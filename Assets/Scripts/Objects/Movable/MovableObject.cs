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
		protected bool isMoving
		{
			get { return (rigidbody2D.velocity.sqrMagnitude >= float.Epsilon); }
		}
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

		protected CollisionCircle collisionbox;
		protected Vector2? destinationPosition;

		protected override void Start() {
			collisionbox = new CollisionCircle(gameObject);
			base.Start();
		}

		protected virtual void Update() {
			if(isMoving)
				UpdateSortingLayer();
		}

		protected Vector2 FindHitFromRaycast(Vector2 dest) {
			Vector2 hitposition = new Vector2();
			var rcasthits = Physics2D.LinecastAll(transform.position, dest);
			Debug.DrawRay(transform.position, mousePosition - (Vector2) transform.position, Color.blue, 10.0f);
			foreach(var hit in rcasthits) {
				if(hit.transform.gameObject == this.gameObject) continue;
				if(hit.collider.isTrigger) continue;
				if(hit.transform.GetComponent<Item>() != null) continue;

//				Debug.Log(hit.transform.name);
//				Debug.Log(hit.point);
				Debug.DrawLine((Vector2) transform.position, (Vector2) hit.point, Color.red, 10.0f);
				hitposition = hit.point;
				break;
			}
			return hitposition;
		}

		protected virtual void OnTriggerStay2D(Collider2D collider)
		{
			var obj = collider.GetComponent<ImmovableObject>();
			if(obj == null) return;
			var c = obj.GetComponent<PolygonCollider2D>();
			if(c == null) return;

			Vector2 left = new Vector2(float.MaxValue, 0);
			Vector2 right = new Vector2(float.MinValue, 0);

			for(int i = 0; i < c.points.Length; i++) {
				if(c.points[i].x < left.x) left = c.points[i];
				if(c.points[i].x > right.x) right = c.points[i];
			}
			left += (Vector2) obj.transform.position;
			right += (Vector2) obj.transform.position;

			Vector2 ltor = right - left;
			Vector2 ltop = (Vector2) transform.position - left;
			Vector3 cross = Vector3.Cross(ltor, ltop);
			Vector2 center = left + ltor/2;

//			Debug.DrawLine(left, right, Color.blue, 1.0f);
//			Debug.DrawLine(left, (Vector2) transform.position, Color.red, 1.0f);

			bool rabove = cross.z > 0;
			bool cAbove = transform.position.y > obj.colliderCenter.y;

			int objSortOrder = (int) ((1000 - obj.colliderCenter.y) * 10);
			int thisSortOrder = (int) ((1000 - transform.position.y) * 10);

			if(rabove && !cAbove) {
				sortingOffset = objSortOrder - thisSortOrder - 1;
//				Debug.Log("Real above, virtual below");
			}
			else if(!rabove && cAbove) {
				sortingOffset = objSortOrder - thisSortOrder + 1;
//				Debug.Log("Real above, virtual below");
			}
			else
				sortingOffset = 0;
		}

		protected void OnTriggerExit2D(Collider2D collider) {
			var obj = collider.GetComponent<ImmovableObject>();
			if(obj == null) return;
			var c = obj.GetComponent<PolygonCollider2D>();
			if(c == null) return;

			sortingOffset = 0;
		}
    }
}