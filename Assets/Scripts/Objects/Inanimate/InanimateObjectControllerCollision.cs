using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Objects.Inanimate
{
   public abstract partial class InanimateObjectController : MacabreObjectController
    {
        private SpriteRenderer spriteRenderer
        {
            get { return GetComponentInChildren<SpriteRenderer>(); }
        }

        // The collision box for colliding with objects
        private PolygonCollider2D polygonCollider;
        protected PolygonCollider2D PolygonCollider
        {
            get
            {
                if (GetComponent<PolygonCollider2D>() != null) polygonCollider = GetComponent<PolygonCollider2D>();
                if (polygonCollider == null)
                {
                    polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
                    CreateCollisionCircle();
                }
                return polygonCollider;
            }
        }
        protected override Collider2D collisionBox
        {
            get { return polygonCollider; }
        }
        protected override Vector2[] SpriteColliderVectices
        {
            get
            {
                return polygonCollider.points;
            }
        }

        protected virtual void CreateCollisionCircle()
        {
            if (footprint == null) throw new Exceptions.MacabreException("No Sprite Collider Shape attached to " + name);
            
            polygonCollider.points = GetVector2EdgesFromTexture();
        }

        protected Vector2[] GetVector2EdgesFromTexture()
        {
            // In here we create the collider circle by finding the points on the sprite
            Sprite sprite = spriteRenderer.sprite;
            Rect rect = sprite.rect;
            footprint.ReadPixels(rect, 0, 0);

            int x = Mathf.FloorToInt(rect.x);
            int y = Mathf.FloorToInt(rect.y);
            int width = Mathf.FloorToInt(rect.width);
            int height = Mathf.FloorToInt(rect.height);

            // Get the color map from the sprite Collider Shape
            Color[] colorMap = footprint.GetPixels(x, y, width, height);

            int topIndex = 0;
            int bottomIndex = 0;
            int leftIndex = 0;
            int rightIndex = 0;

            int xMax = 0;
            int xMin = 0;
            int yMax = 0;
            int yMin = 0;

            // Loop through all the pixels and get the 4 directional indices
            for (int p = 0; p < x * y; p++)
            {
                if (colorMap[p].a != 0.0f)
                {
                    int i = p % x;
                    int j = p / x;

                    if (j > yMax) rightIndex = p;
                    if (j < yMin) leftIndex = p;
                    if (i > xMax) bottomIndex = p;
                    if (i < xMin) topIndex = p;
                }
            }

            // Create the collider2D based on the index, since its one pixel to one index
            Vector2 topVector = new Vector2(topIndex % x, topIndex / x) - sprite.pivot;
            Vector2 bottomVector = new Vector2(bottomIndex % x, bottomIndex / x) - sprite.pivot;
            Vector2 leftVector = new Vector2(leftIndex % x, leftIndex / x) - sprite.pivot;
            Vector2 rightVector = new Vector2(rightIndex % x, rightIndex / x) - sprite.pivot;

            Vector2[] points = new Vector2[5]
            {
                topVector,
                rightVector,
                bottomVector,
                leftVector,
                topVector
            };

            return points;
        }

        // The proximity circle for detecting if objects are nearby
        private EllipseCollider2D proximityCircle;
        protected virtual EllipseCollider2D ProximityCircle
        {
            get
            {
                if (proximityCircle == null)
                {
                    proximityCircle = gameObject.AddComponent<EllipseCollider2D>();
                    CreateProximityCircle();
                }

                return proximityCircle;
            }
        }
        protected override Collider2D proximityBox
        {
            get { return proximityCircle; }
        }
        protected override Vector2[] SpriteProximityVertices
        {
            get
            {
                return ProximityCircle.points;
            }
        }

        public virtual void CreateProximityCircle()
        {
            ProximityCircle.isTrigger = true;
            float width = spriteRenderer.sprite.rect.width;
            ProximityCircle.radiusX = width / 2f;
            ProximityCircle.radiusY = width / 2f;
            ProximityCircle.smoothness = 4;
        }
    }
}
