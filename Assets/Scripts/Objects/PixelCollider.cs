using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class PixelCollider : MonoBehaviour, IComparable<PixelCollider>
    {
        public struct MovementRestriction {
            public bool restrictNW;
            public bool restrictNE;
            public bool restrictSW;
            public bool restrictSE;
        }

        new PolygonCollider2D collider2D;

        Vector2 top, bottom, left, right;
        Vector2[] colliderPoints;

        public int sortingOffset;

        void Awake()
        {
            collider2D = GetComponent<PolygonCollider2D>();

            Debug.Assert(collider2D != null);
            Debug.Assert(collider2D.points.Length == 4);
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
                sr.sortingOrder = i;
                Debug.Log(sr.gameObject.name + " " + sr.sortingOrder.ToString());
            }
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

            foreach (RaycastHit2D raycastHit in castStar)
            {
                PixelCollider otherPixelCollider = raycastHit.collider.GetComponent<PixelCollider>();
                if (otherPixelCollider == null) continue;

                Transform otherTransform = otherPixelCollider.gameObject.transform;

                Debug.Assert(otherPixelCollider.colliderPoints.Length == 4);

                Vector2 othertopWorld = otherPixelCollider.top + (Vector2) otherTransform.position;
                Vector2 otherbottomWorld = otherPixelCollider.bottom + (Vector2) otherTransform.position;
                Vector2 otherleftWorld = otherPixelCollider.left + (Vector2) otherTransform.position;
                Vector2 otherrightWorld = otherPixelCollider.right + (Vector2) otherTransform.position;

                //Debug.DrawLine(othertopWorld, otherbottomWorld);
                //Debug.DrawLine(otherleftWorld, otherrightWorld);

                if (DistanceBetween4pointsAbs(leftWorld, topWorld, otherbottomWorld, otherrightWorld) < 1.1 &&
                    leftWorld.x < otherrightWorld.x && topWorld.x > otherbottomWorld.x &&
                    leftWorld.y < otherrightWorld.y && topWorld.y > otherbottomWorld.y)
                    restrictNW = true;
                
                if (DistanceBetween4pointsAbs(topWorld, rightWorld, otherleftWorld, otherbottomWorld) < 1.1 &&
                    topWorld.x < otherbottomWorld.x && rightWorld.x > otherleftWorld.x &&
                    topWorld.y > otherbottomWorld.y && rightWorld.y < otherleftWorld.y)
                    restrictNE = true;
                
                if (DistanceBetween4pointsAbs(leftWorld, bottomWorld, othertopWorld, otherrightWorld) < 1.1 &&
                    leftWorld.x < otherrightWorld.x && bottomWorld.x > othertopWorld.x &&
                    leftWorld.y > otherrightWorld.y && bottomWorld.y < othertopWorld.y)
                    restrictSW = true;

                if (DistanceBetween4pointsAbs(bottomWorld, rightWorld, otherleftWorld, othertopWorld) < 1.1 &&
                    bottomWorld.x < othertopWorld.x && rightWorld.x > otherleftWorld.x &&
                    bottomWorld.y < othertopWorld.y && rightWorld.y > otherleftWorld.y)
                    restrictSE = true;
            }

            MovementRestriction movementRestriction;
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

            if(DistanceBetween4points(aleftWorld, atopWorld, bbottomWorld, brightWorld) >= -0.5)
                if(aleftWorld.x < brightWorld.x && aleftWorld.y < brightWorld.y)
                    return 1;

            if (DistanceBetween4points(atopWorld, arightWorld, bleftWorld, bbottomWorld) >= -0.5)
                if (arightWorld.x > bleftWorld.x && arightWorld.y < bleftWorld.y)
                    return 1;

            if (DistanceBetween4points(bleftWorld, btopWorld, abottomWorld, arightWorld) >= -0.5)
                if (bleftWorld.x < arightWorld.x && bleftWorld.y < arightWorld.y)
                    return -1;

            if (DistanceBetween4points(btopWorld, brightWorld, aleftWorld, abottomWorld) >= -0.5)
                if (brightWorld.x > aleftWorld.x && brightWorld.y < aleftWorld.y)
                    return -1;

            return 0;
        }
    }
}