using UnityEngine;
using System.Collections.Generic;

namespace Objects.Immovable
{
    public abstract class ImmovableObject : PixelObject {
		public virtual Vector2 colliderCenter {
			get {
				if(colliderCenter_ != Vector2.zero) return colliderCenter_;
				var c = GetComponent<PolygonCollider2D>();
				if(c == null) return Vector2.zero;

				Vector2 left = new Vector2(float.MaxValue, 0);
				Vector2 right = new Vector2(float.MinValue, 0);

				for(int i = 0; i < c.points.Length; i++) {
					if(c.points[i].x < left.x) left = c.points[i];
					if(c.points[i].x > right.x) right = c.points[i];
				}
				left += (Vector2) transform.position;
				right += (Vector2) transform.position;

				Vector2 ltor = right - left;
				colliderCenter_ = left + ltor/2;
				return colliderCenter_;
			}
		}

		Vector2 colliderCenter_;
		protected List<PixelObject> itemsToShowWhenEntered = new List<PixelObject>();
		protected List<PixelObject> itemsToHideWhenEntered = new List<PixelObject>();

		void OnTriggerStay2D(Collider2D other) {}

		protected override void Start() {
			base.Start();
		}
    }
}
