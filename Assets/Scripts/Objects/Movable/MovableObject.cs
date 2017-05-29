using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Objects;

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
    }
}