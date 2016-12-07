using System;
using UnityEngine;

namespace Objects.Inanimate.Items
{
    public partial class ItemController : InanimateObjectController {

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
        
        private EllipseCollider2D CollisionCircle
        {
            get
            {
                if (collisionCircle == null)
                {
                    collisionCircle = gameObject.AddComponent<EllipseCollider2D>();
                    CreateCollisionCircle();
                }
                return collisionCircle;
            }
        }
        private EllipseCollider2D ProximityCircle
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

        public override void CreateCollisionCircle()
        {
            float width = spriteRenderer.sprite.rect.width;
            CollisionCircle.radiusX = width / 5f;
            CollisionCircle.radiusY = width / 10f;
            CollisionCircle.smoothness = 4;
        }
        public override void CreateProximityCircle()
        {
            ProximityCircle.isTrigger = true;
            float width = spriteRenderer.sprite.rect.width;
            ProximityCircle.radiusX = width / 2f;
            ProximityCircle.radiusY = width / 2f;
            ProximityCircle.smoothness = 4;
        }
    }
}
