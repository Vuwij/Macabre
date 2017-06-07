using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Objects;
using Objects.Immovable;

namespace Objects.Movable
{
    public abstract class MovableObject : Object
    {
		protected bool isMoving
		{
			get { return (rigidbody2D.velocity.sqrMagnitude >= float.Epsilon); }
		}

		protected bool movementLocked;
		CollisionCircle collisionbox;

		protected override void Start() {
			collisionbox = new CollisionCircle(gameObject);
			base.Start();
		}

		protected virtual void Update() {
			if(isMoving)
				UpdateSortingLayer();
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

			bool rabove = cross.z > 0;
			bool cAbove = transform.position.y > obj.colliderCenter.y;

			int objSortOrder = (int) ((1000 - obj.colliderCenter.y) * 10);
			int thisSortOrder = (int) ((1000 - transform.position.y) * 10);

			if(rabove && !cAbove)
				sortingOffset = thisSortOrder - objSortOrder;

			if(!rabove && cAbove)
				sortingOffset = objSortOrder - thisSortOrder;
			
			print(sortingOffset);
		}
    }
}