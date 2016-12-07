using System;
using UnityEngine;

namespace Objects.Movable
{
    public abstract partial class MovingObjectController : MacabreObjectController
    {
        private EllipseCollider2D collisionCircle = null;
        private EllipseCollider2D proximityCircle = null;

        private SpriteRenderer spriteRenderer
        {
            get { return GetComponentInChildren<SpriteRenderer>(); }
        }

        protected override PolygonCollider2D collisionBox
        {
            get { return (PolygonCollider2D)collisionCircle; }
        }
        protected override PolygonCollider2D proximityBox
        {
            get { return (PolygonCollider2D)proximityCircle; }
        }
        
        protected virtual EllipseCollider2D CollisionCircle
        {
            get
            {
                if (collisionCircle == null)
                    CreateCollisionCircle();
                return collisionCircle;
            }
        }
        protected virtual EllipseCollider2D ProximityCircle
        {
            get
            {
                if (proximityCircle == null) CreateProximityCircle();
                return proximityCircle;
            }
        }
        
        protected virtual void CreateCollisionCircle()
        {
            if(collisionCircle == null) collisionCircle = gameObject.AddComponent<EllipseCollider2D>();
            float width = spriteRenderer.sprite.rect.width;
            collisionCircle.radiusX = width * 0.25f;
            collisionCircle.radiusY = width * 0.125f;
        }
        protected virtual void CreateProximityCircle()
        {
            if(proximityCircle == null)
                proximityCircle = gameObject.AddComponent<EllipseCollider2D>();

            proximityCircle.isTrigger = true;
            float width = spriteRenderer.sprite.rect.width;
            proximityCircle.radiusX = width * 3f;
            proximityCircle.radiusY = width * 1.5f;
        }
    }
}
