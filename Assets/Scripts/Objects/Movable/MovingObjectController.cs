using UnityEngine;
using Objects;

namespace Objects.Movable
{
    public abstract partial class MovingObjectController : MacabreObjectController {
        protected Rigidbody2D rb2D
        {
			get { return gameObject.GetComponent<Rigidbody2D>(); }
        }

        protected override void Start()
        {
            base.Start();
            CreateCollisionCircle();
            CreateProximityCircle();
        }

		#region Collision

		protected EllipseCollider2D collisionCircle = null;
		protected EllipseCollider2D proximityCircle = null;

		private SpriteRenderer spriteRenderer
		{
			get { return GetComponentInChildren<SpriteRenderer>(); }
		}

		protected override PolygonCollider2D collisionBox
		{
			get { return (PolygonCollider2D)CollisionCircle; }
		}
		protected override PolygonCollider2D proximityBox
		{
			get { return (PolygonCollider2D)ProximityCircle; }
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

		public override void CreateCollisionCircle()
		{
			if(collisionCircle == null) collisionCircle = gameObject.AddComponent<EllipseCollider2D>();
			float width = spriteRenderer.sprite.rect.width;
			collisionCircle.radiusX = width * 0.25f;
			collisionCircle.radiusY = width * 0.125f;
		}
		public override void CreateProximityCircle()
		{
			if(proximityCircle == null)
				proximityCircle = gameObject.AddComponent<EllipseCollider2D>();

			proximityCircle.isTrigger = true;
			float width = spriteRenderer.sprite.rect.width;
			proximityCircle.radiusX = width * 3f;
			proximityCircle.radiusY = width * 1.5f;
		}

		#endregion

		#region Layer Sorting

		#endregion

    }
}
