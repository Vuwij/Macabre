using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Objects;
using Objects.Immovable;
using Objects.Immovable.Items;

namespace Objects.Movable
{
    public abstract class MovableObject : Object
    {
		protected Vector2 mousePosition
		{
			get {
				Vector2 click = Input.mousePosition;
				var offset = click - new Vector2(320.0f, 180.0f);
				offset.Scale(new Vector2(0.5f, 0.5f));
				click = offset + new Vector2(320.0f, 180.0f);
				return Camera.main.ScreenToWorldPoint(click);
			}
		}

		protected Vector2? destinationPosition;

		protected override void Start() {
			base.Start();
		}

        protected override void Update()
        {
            base.Update();
        }

		protected Vector2 FindHitFromRaycast(Vector2 dest) {
			Vector2 hitposition = new Vector2();
			var rcasthits = Physics2D.LinecastAll(transform.position, dest);
			//Debug.DrawRay(transform.position, mousePosition - (Vector2) transform.position, Color.blue, 10.0f);
			bool hitSomething = false;
			foreach(var hit in rcasthits) {
				if(hit.transform.gameObject == this.gameObject) continue;
				if(hit.collider.isTrigger) continue;
				if(hit.transform.GetComponent<Item>() != null) continue;

				Debug.DrawLine((Vector2) transform.position, (Vector2) hit.point, Color.red, 10.0f);
				hitposition = hit.point;
				hitSomething = true;
				break;
			}
			if(hitSomething) return hitposition;
			else return dest;
		}
		protected void OnTriggerExit2D(Collider2D collider) {
			var obj = collider.GetComponent<ImmovableObject>();
			if(obj == null) return;
			var c = obj.GetComponent<PolygonCollider2D>();
			if(c == null) return;
   		}
    }
}