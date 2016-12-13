using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Objects.Inanimate
{
   public abstract partial class InanimateObjectController : MacabreObjectController, ICustomCollider
    {
        protected PolygonCollider2D collisionCircle = null;
        protected EllipseCollider2D proximityCircle = null;
        
        protected SpriteRenderer spriteRenderer
        {
            get { return GetComponentInChildren<SpriteRenderer>(); }
        }
        protected override PolygonCollider2D collisionBox
        {
            get { return CollisionCircle; }
        }
        protected override PolygonCollider2D proximityBox
        {
            get { return (PolygonCollider2D)ProximityCircle; }
        }

        private PolygonCollider2D CollisionCircle
        {
            get
            {
                if (collisionCircle == null) CreateCollisionCircle();
                return collisionCircle;
            }
        }
        private EllipseCollider2D ProximityCircle
        {
            get
            {
                if (proximityCircle == null)
                    CreateProximityCircle();
                return proximityCircle;
            }
        }

        public override void CreateCollisionCircle()
        {
            if (GetComponent<PolygonCollider2D>() == null)
            {
                collisionCircle = gameObject.AddComponent<PolygonCollider2D>();
                if (footprint == null) throw new Exceptions.MacabreException("No Sprite Collider Shape attached to " + name);
            }
        }
        public override void CreateProximityCircle()
        {
            if(proximityCircle == null) proximityCircle = gameObject.AddComponent<EllipseCollider2D>();
            proximityCircle.isTrigger = true;
            float width = spriteRenderer.sprite.rect.width;
            proximityCircle.radiusX = width / 2f;
            proximityCircle.radiusY = width / 2f;
            proximityCircle.smoothness = 4;
        }

        // Find the vector 2 from edges in collider
        protected Vector2[] GetVector2EdgesFromTexture()
        {
            // In here we create the collider circle by finding the points on the sprite
            Sprite sprite = spriteRenderer.sprite;
            Rect rect = sprite.rect;
            
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

            // Find a valid pixel
            for (int p = 0; p < width * height; p++)
            {
                if (colorMap[p].a != 0.0f)
                {
                    int i = p % width;
                    int j = p / width;

                    rightIndex = p;
                    leftIndex = p;
                    bottomIndex = p;
                    topIndex = p;

                    xMax = i;
                    xMin = i;
                    yMax = j;
                    yMin = j;

                    break;
                }
            }

            // Loop through all the pixels and get the 4 directional indices
            for (int p = 0; p < width * height; p++)
            {
                if (colorMap[p].a != 0.0f)
                {
                    int i = p % width;
                    int j = p / width;

                    if (i > xMax)
                    {
                        rightIndex = p;
                        xMax = i;
                    }
                    else if (i < xMin)
                    {
                        leftIndex = p;
                        xMin = i;
                    }
                    if (j > yMax)
                    {
                        bottomIndex = p;
                        yMax = j;
                    }
                    else if (j < yMin)
                    {
                        topIndex = p;
                        yMin = j;
                    }
                }
            }

            // Create the collider2D based on the index, since its one pixel to one index
            Vector2 topVector = new Vector2(topIndex % width, topIndex / width) - sprite.pivot;
            Vector2 bottomVector = new Vector2(bottomIndex % width, bottomIndex / width) - sprite.pivot;
            Vector2 leftVector = new Vector2(leftIndex % width, leftIndex / width) - sprite.pivot;
            Vector2 rightVector = new Vector2(rightIndex % width, rightIndex / width) - sprite.pivot;

            Vector2[] points = new Vector2[5]
            {
                topVector,
                rightVector,
                bottomVector,
                leftVector,
                topVector
            };

            Debug.DrawLine(topVector, leftVector);
            Debug.DrawLine(leftVector, bottomVector);
            Debug.DrawLine(bottomVector, rightVector);
            Debug.DrawLine(rightVector, topVector);

            return points;
        }
    }
}