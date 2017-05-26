using UnityEngine;
using System.Collections.Generic;

namespace Objects.Unmovable
{
    public abstract partial class UnmovableObject : Object {
		public Object MacabreObject;

		#region Collision

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
			// Needs to be custom created
		}
		public override void CreateProximityCircle()
		{
			//if(proximityCircle == null) proximityCircle = gameObject.AddComponent<EllipseCollider2D>();
			//proximityCircle.isTrigger = true;
			//float width = spriteRenderer.sprite.rect.width;
			//proximityCircle.radiusX = width / 2f;
			//proximityCircle.radiusY = width / 2f;
			//proximityCircle.smoothness = 4;
		}

		#endregion

		#region Layer Sorting

		// TODO Automatically Reposition the object
		partial void OnTriggerStay2D(Collider2D other);

		//partial void OnTriggerExit2D(Collider2D other);

		#endregion

		#region Opacity

		protected List<Object> itemsToShowWhenEntered = new List<Object>();
		protected List<Object> itemsToHideWhenEntered = new List<Object>();

		// TODO, set objects to hide and show when you enter the object

		#endregion
    }
}
