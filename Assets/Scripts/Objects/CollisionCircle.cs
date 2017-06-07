using System;
using UnityEngine;
using System.Collections.Generic;

namespace Objects
{
	public class CollisionCircle
	{
		Vector2[] collisionVertices
		{
			get { return collisionCircle.points; }
		}
		Vector2[] proximityVertices
		{
			get { return proximityCircle.points; }
		}
		public float radius {
			get { return collisionCircle.radiusX; }
			set { 
				collisionCircle.radiusX = value;
				collisionCircle.radiusY = value/2;

				proximityCircle.radiusX = value * proximityRatio;
				proximityCircle.radiusY = value/2 * proximityRatio;
			} 
		}
		public int smoothness {
			get { return collisionCircle.smoothness; }
			set { collisionCircle.smoothness = value; } 
		}
		PhysicsMaterial2D ice {
			get {
				if(ice_ == null) ice_ = Resources.Load("Materials/Ice", typeof(PhysicsMaterial2D)) as PhysicsMaterial2D;
				return ice_;
			}
		}

		static PhysicsMaterial2D ice_;
		float proximityRatio = 12;
		GameObject gameObject;
		EllipseCollider2D collisionCircle;
		EllipseCollider2D proximityCircle;

		public CollisionCircle (GameObject gameObject, float radius = 4)
		{
			this.gameObject = gameObject;

			// Creating Collision and Proximity Circle
			collisionCircle = this.gameObject.AddComponent<EllipseCollider2D>();
			proximityCircle = this.gameObject.AddComponent<EllipseCollider2D>();
			proximityCircle.isTrigger = true;
			this.radius = radius;

			// Bounciness to circle
			collisionCircle.polygonCollider.sharedMaterial = ice;
		}
	}
}

