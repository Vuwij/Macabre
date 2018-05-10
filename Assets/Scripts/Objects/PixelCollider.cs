﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class PixelCollider : MonoBehaviour, IComparable<PixelCollider>
    {
        public class MovementRestriction {
            public bool restrictNW = false;
            public bool restrictNE = false;
            public bool restrictSW = false;
            public bool restrictSE = false;
        }

        new PolygonCollider2D collider2D;

        Vector2 top, bottom, left, right;
        Vector2[] colliderPoints;

        public bool isTrigger;

        int pixelProximity = 4; // 3 pixels away from the object
        Vector2 topP, bottomP, leftP, rightP;

        void Awake()
        {
            collider2D = GetComponent<PolygonCollider2D>();

            Debug.Assert(collider2D != null);
            Debug.Assert(collider2D.points.Length == 4);
            Debug.Assert(transform.parent.GetComponent<PixelRoom>() == null);
            Debug.Assert(transform.parent.GetComponent<PolygonCollider2D>() == null);

            colliderPoints = collider2D.points;

            top = colliderPoints[0];
            bottom = colliderPoints[0];
            left = colliderPoints[0];
            right = colliderPoints[0];

            for (int i = 0; i < 4; ++i)
            {
                if (colliderPoints[i].y > top.y)
                    top = colliderPoints[i];
                if (colliderPoints[i].y < bottom.y)
                    bottom = colliderPoints[i];
                if (colliderPoints[i].x < left.x)
                    left = colliderPoints[i];
                if (colliderPoints[i].x > right.x)
                    right = colliderPoints[i];
            }

            topP = top + new Vector2(0, pixelProximity);
            bottomP = bottom + new Vector2(0, -pixelProximity);
            leftP = left + new Vector2(-2 * pixelProximity, 0);
            rightP = right + new Vector2(2 * pixelProximity, 0);
        }

        public void TopologicalSortNearbySortingLayers() {
            Vector3 castStart = transform.position;
            castStart.z = -10.0f;

            RaycastHit2D[] castStar = Physics2D.CircleCastAll(castStart, GameSettings.inspectRadius * 20, Vector2.zero);
            List<PixelCollider> pixelColliders = new List<PixelCollider>();

            foreach (RaycastHit2D raycastHit in castStar)
            {
                PixelCollider otherPixelCollider = raycastHit.collider.GetComponent<PixelCollider>();
                if (otherPixelCollider == null) continue;
                if (otherPixelCollider.ParentIsContainer()) continue;
                pixelColliders.Add(otherPixelCollider);
            }

            // Bubble Sort
            for (int i = 0; i < pixelColliders.Count; ++i) {
                for (int j = 0; j < pixelColliders.Count; ++j) {
                    if (i == j) continue;
                    if(pixelColliders[i].CompareTo(pixelColliders[j]) == -1) {
                        PixelCollider temp = pixelColliders[i];
                        pixelColliders[i] = pixelColliders[j];
                        pixelColliders[j] = temp;
                    }
                }
            }

            for (int i = 0; i < pixelColliders.Count; ++i) {
                SpriteRenderer sr = pixelColliders[i].transform.parent.GetComponentInChildren<SpriteRenderer>();
                Debug.Assert(sr != null);
                sr.sortingOrder = i * 2;

                // Child objects
                PixelCollider[] childobjects = pixelColliders[i].transform.parent.GetComponentsInChildren<PixelCollider>();
                foreach(PixelCollider co in childobjects) {
                    SpriteRenderer srchild = co.transform.parent.GetComponentInChildren<SpriteRenderer>();
                    Debug.Assert(srchild != null);
                    srchild.sortingOrder = i * 2 + 1;
                }

                //Debug.Log(sr.gameObject.name + " " + sr.sortingOrder.ToString());
            }
        }

        // Finds nearest other pixel collider, same as check for collision, but returns a list of objects instead
        public List<PixelCollider> CheckForInspection()
        {
            List<PixelCollider> pixelColliders = new List<PixelCollider>();

            Vector3 castStart = transform.position;
            castStart.z = -10.0f;

            Vector2 topWorld = topP + (Vector2)transform.position;
            Vector2 bottomWorld = bottomP + (Vector2)transform.position;
            Vector2 leftWorld = leftP + (Vector2)transform.position;
            Vector2 rightWorld = rightP + (Vector2)transform.position;

            RaycastHit2D[] castStar = Physics2D.CircleCastAll(castStart, GameSettings.inspectRadius * 3, Vector2.zero);

            foreach (RaycastHit2D raycastHit in castStar)
            {
                PixelCollider otherPixelCollider = raycastHit.collider.GetComponent<PixelCollider>();
                if (otherPixelCollider == null) continue;
                if (otherPixelCollider.ParentIsContainer()) continue;

                Transform otherTransform = otherPixelCollider.gameObject.transform;

                Debug.Assert(otherPixelCollider.colliderPoints.Length == 4);

                Vector2 othertopWorld = otherPixelCollider.top + (Vector2)otherTransform.position;
                Vector2 otherbottomWorld = otherPixelCollider.bottom + (Vector2)otherTransform.position;
                Vector2 otherleftWorld = otherPixelCollider.left + (Vector2)otherTransform.position;
                Vector2 otherrightWorld = otherPixelCollider.right + (Vector2)otherTransform.position;

                //Debug.DrawLine(othertopWorld, otherbottomWorld);
                //Debug.DrawLine(otherleftWorld, otherrightWorld);

                if (DistanceBetween4points(leftWorld, topWorld, otherbottomWorld, otherrightWorld) < 0.8 &&
                    leftWorld.x < (otherrightWorld.x) && topWorld.x > (otherbottomWorld.x) &&
                    leftWorld.y < (otherrightWorld.y) && topWorld.y > (otherbottomWorld.y))
                {
                    pixelColliders.Add(otherPixelCollider);
                    pixelColliders.AddRange(otherPixelCollider.GetChildColliders());
                }
                else if (DistanceBetween4points(topWorld, rightWorld, otherleftWorld, otherbottomWorld) < 0.8 &&
                    topWorld.x < (otherbottomWorld.x) && rightWorld.x > (otherleftWorld.x) &&
                         topWorld.y > (otherbottomWorld.y) && rightWorld.y < (otherleftWorld.y))
                {
                    pixelColliders.Add(otherPixelCollider);
                    pixelColliders.AddRange(otherPixelCollider.GetChildColliders());
                }
                else if (DistanceBetween4points(leftWorld, bottomWorld, othertopWorld, otherrightWorld) > -0.8 &&
                    leftWorld.x < (otherrightWorld.x) && bottomWorld.x > (othertopWorld.x) &&
                         leftWorld.y > (otherrightWorld.y) && bottomWorld.y < (othertopWorld.y))
                {
                    pixelColliders.Add(otherPixelCollider);
                    pixelColliders.AddRange(otherPixelCollider.GetChildColliders());
                }
                else if (DistanceBetween4points(bottomWorld, rightWorld, otherleftWorld, othertopWorld) > -0.8 &&
                    bottomWorld.x < (othertopWorld.x) && rightWorld.x > (otherleftWorld.x) &&
                         bottomWorld.y < (othertopWorld.y) && rightWorld.y > (otherleftWorld.y))
                {
                    pixelColliders.Add(otherPixelCollider);
                    pixelColliders.AddRange(otherPixelCollider.GetChildColliders());
                }
                
            }

            return pixelColliders;
        }

        public MovementRestriction CheckForCollision()
        {
            Vector3 castStart = transform.position;
            castStart.z = -10.0f;

            Vector2 topWorld = top + (Vector2) transform.position;
            Vector2 bottomWorld = bottom + (Vector2)  transform.position;
            Vector2 leftWorld = left + (Vector2)  transform.position;
            Vector2 rightWorld = right + (Vector2)  transform.position;

            RaycastHit2D[] castStar = Physics2D.CircleCastAll(castStart, GameSettings.inspectRadius * 2, Vector2.zero);

            bool restrictNW = false;
            bool restrictNE = false;
            bool restrictSW = false;
            bool restrictSE = false;

            // Collided with other object
            foreach (RaycastHit2D raycastHit in castStar)
            {
                PixelCollider otherPixelCollider = raycastHit.collider.GetComponent<PixelCollider>();
                if (otherPixelCollider == null) continue;
                if (otherPixelCollider.ParentIsContainer()) continue;

                Transform otherTransform = otherPixelCollider.gameObject.transform;

                Debug.Assert(otherPixelCollider.colliderPoints.Length == 4);

                Vector2 othertopWorld = otherPixelCollider.top + (Vector2) otherTransform.position;
                Vector2 otherbottomWorld = otherPixelCollider.bottom + (Vector2) otherTransform.position;
                Vector2 otherleftWorld = otherPixelCollider.left + (Vector2) otherTransform.position;
                Vector2 otherrightWorld = otherPixelCollider.right + (Vector2) otherTransform.position;

                //Debug.DrawLine(othertopWorld, otherbottomWorld);
                //Debug.DrawLine(otherleftWorld, otherrightWorld);

                if (DistanceBetween4points(leftWorld, topWorld, otherbottomWorld, otherrightWorld) < 0.8 &&
                    leftWorld.x < (otherrightWorld.x) && topWorld.x > (otherbottomWorld.x) &&
                    leftWorld.y < (otherrightWorld.y) && topWorld.y > (otherbottomWorld.y))
                    restrictNW = true;
                
                if (DistanceBetween4points(topWorld, rightWorld, otherleftWorld, otherbottomWorld) < 0.8 &&
                    topWorld.x < (otherbottomWorld.x) && rightWorld.x > (otherleftWorld.x) &&
                    topWorld.y > (otherbottomWorld.y) && rightWorld.y < (otherleftWorld.y))
                    restrictNE = true;
                
                if (DistanceBetween4points(leftWorld, bottomWorld, othertopWorld, otherrightWorld) > -0.8 &&
                    leftWorld.x < (otherrightWorld.x) && bottomWorld.x > (othertopWorld.x) &&
                    leftWorld.y > (otherrightWorld.y) && bottomWorld.y < (othertopWorld.y))
                    restrictSW = true;

                if (DistanceBetween4points(bottomWorld, rightWorld, otherleftWorld, othertopWorld) > -0.8 &&
                    bottomWorld.x < (othertopWorld.x) && rightWorld.x > (otherleftWorld.x) &&
                    bottomWorld.y < (othertopWorld.y) && rightWorld.y > (otherleftWorld.y))
                    restrictSE = true;
            }

            // Collided with floor
            PixelRoom floor = transform.parent.parent.GetComponent<PixelRoom>();
            Debug.Assert(floor != null);
            Debug.Assert(floor.colliderPoints.Length == 4);

            Vector2 floortopWorld = floor.top + (Vector2)floor.transform.position;
            Vector2 floorbottomWorld = floor.bottom + (Vector2)floor.transform.position;
            Vector2 floorleftWorld = floor.left + (Vector2)floor.transform.position;
            Vector2 floorrightWorld = floor.right + (Vector2)floor.transform.position;

            if (DistanceBetween4points(leftWorld, topWorld, floorleftWorld, floortopWorld) < 1.1)
                restrictNW = true;

            if (DistanceBetween4points(topWorld, rightWorld, floortopWorld, floorrightWorld) < 1.1)
                restrictNE = true;

            if (DistanceBetween4points(leftWorld, bottomWorld, floorleftWorld, floorbottomWorld) > -1.1)
                restrictSW = true;

            if (DistanceBetween4points(bottomWorld, rightWorld, floorbottomWorld, floorrightWorld) > -1.1)
                restrictSE = true;

            // Send off movement restriction
            MovementRestriction movementRestriction = new MovementRestriction();

            movementRestriction.restrictNE = restrictNE;
            movementRestriction.restrictNW = restrictNW;
            movementRestriction.restrictSE = restrictSE;
            movementRestriction.restrictSW = restrictSW;

            return movementRestriction;
        }

        float DistanceBetween4points(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2) {
            float m = (a2.y - a1.y) / (a2.x - a1.x); // Slope of parallel lines
            float i1 = a1.y - a1.x * m; // Intercept 1
            float i2 = b1.y - b1.x * m; // Intercept 2
            float dist = (i2 - i1) / Mathf.Sqrt(m * m + 1);
            return dist;
        }

        float DistanceBetween4pointsAbs(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            float m = (a2.y - a1.y) / (a2.x - a1.x); // Slope of parallel lines
            float i1 = a1.y - a1.x * m; // Intercept 1
            float i2 = b1.y - b1.x * m; // Intercept 2
            float dist = Mathf.Abs(i2 - i1) / Mathf.Sqrt(m * m + 1);
            return dist;
        }

        // Returns 1 if in front of the other
        public int CompareTo(PixelCollider other)
        {
            Debug.Assert(this.colliderPoints.Length == 4);
            Debug.Assert(other.colliderPoints.Length == 4);

            Vector2 atopWorld = this.top + (Vector2) this.transform.position;
            Vector2 abottomWorld = this.bottom + (Vector2) this.transform.position;
            Vector2 aleftWorld = this.left + (Vector2) this.transform.position;
            Vector2 arightWorld = this.right + (Vector2) this.transform.position;

            Vector2 btopWorld = other.top + (Vector2) other.transform.position;
            Vector2 bbottomWorld = other.bottom + (Vector2) other.transform.position;
            Vector2 bleftWorld = other.left + (Vector2) other.transform.position;
            Vector2 brightWorld = other.right + (Vector2) other.transform.position;

            if (other == this)
                return 0;

            if(DistanceBetween4points(aleftWorld, atopWorld, bbottomWorld, brightWorld) >= -1.5)
                if(aleftWorld.x < brightWorld.x && aleftWorld.y < brightWorld.y)
                    return 1;

            if (DistanceBetween4points(atopWorld, arightWorld, bleftWorld, bbottomWorld) >= -1.5)
                if (arightWorld.x > bleftWorld.x && arightWorld.y < bleftWorld.y)
                    return 1;

            if (DistanceBetween4points(bleftWorld, btopWorld, abottomWorld, arightWorld) >= -1.5)
                if (bleftWorld.x < arightWorld.x && bleftWorld.y < arightWorld.y)
                    return -1;

            if (DistanceBetween4points(btopWorld, brightWorld, aleftWorld, abottomWorld) >= -1.5)
                if (brightWorld.x > aleftWorld.x && brightWorld.y < aleftWorld.y)
                    return -1;

            return 0;
        }

        public bool ParentIsContainer() {
            for (int i = 0; i < transform.parent.parent.childCount; ++i)
            {
                Transform t = transform.parent.parent.GetChild(i);
                if (t.GetComponent<PixelCollider>() == true)
                    return true;
            }
            return false;
        }

        public List<PixelCollider> GetChildColliders() {
            List<PixelCollider> pixelColliders = new List<PixelCollider>();

            for (int i = 0; i < transform.parent.childCount; ++i)
            {
                Transform t = transform.parent.GetChild(i);
                for (int j = 0; j < t.childCount; ++j)
                {
                    Transform t2 = t.GetChild(j);
                    PixelCollider pc = t2.GetComponent<PixelCollider>();
                    if(pc != null) {
                        pixelColliders.Add(pc);
                    }
                }
            }

            return pixelColliders;
        }
    }
}