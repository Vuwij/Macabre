using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Objects
{
    // Basically the same as a pixel collider, but with an additional choice
	public class RampCollider : PixelCollider
	{
        // The direction that is going up
		public Direction rampDirection;
		public int height = 0;

		public float lengthE => Vector2.Distance(bottomWorld, rightWorld);
		public float lengthW => Vector2.Distance(bottomWorld, leftWorld);

		public float slope {
			get {
				float dist = (rampDirection == Direction.NE || rampDirection == Direction.SW) ? lengthE : lengthW;
				return height / dist;	
			}
		}
        
		public Vector2 topWorldElevated => (rampDirection == Direction.NW || rampDirection == Direction.NE) ? topWorld + new Vector2(0.0f, height) : topWorld;
		public Vector2 bottomWorldElevated => (rampDirection == Direction.SW || rampDirection == Direction.SE) ? bottomWorld + new Vector2(0.0f, height) : bottomWorld;
		public Vector2 leftWorldElevated => (rampDirection == Direction.NW || rampDirection == Direction.SW) ? leftWorld + new Vector2(0.0f, height) : leftWorld;
		public Vector2 rightWorldElevated => (rampDirection == Direction.NE || rampDirection == Direction.SE) ? rightWorld + new Vector2(0.0f, height) : rightWorld;
        
		public CollisionBody collisionBodyRampedWorld => new CollisionBody(topWorldElevated, leftWorldElevated, rightWorldElevated, bottomWorldElevated);
        
		public override void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(topWorldElevated, leftWorldElevated);
			Gizmos.DrawLine(leftWorldElevated, bottomWorldElevated);
			Gizmos.DrawLine(bottomWorldElevated, rightWorldElevated);
			Gizmos.DrawLine(rightWorldElevated, topWorldElevated);

			base.OnDrawGizmos();
		}

		public CollisionBody MatchCollisionBody(CollisionBody input) {
			float h = 0.0f;
			if (rampDirection == Direction.NE || rampDirection == Direction.SW)
				h = slope * Vector2.Distance(input.bottom, input.right);
            if (rampDirection == Direction.SE || rampDirection == Direction.NW)
				h = slope * Vector2.Distance(input.bottom, input.left);
            
			Vector2 inputTopWorldElevated = (rampDirection == Direction.NW || rampDirection == Direction.NE) ? input.top + new Vector2(0.0f, h) : input.top;
			Vector2 inputbottomWorldElevated = (rampDirection == Direction.SW || rampDirection == Direction.SE) ? input.bottom + new Vector2(0.0f, h) : input.bottom;
			Vector2 inputleftWorldElevated = (rampDirection == Direction.NW || rampDirection == Direction.SW) ? input.left + new Vector2(0.0f, h) : input.left;
			Vector2 inputrightWorldElevated = (rampDirection == Direction.NE || rampDirection == Direction.SE) ? input.right + new Vector2(0.0f, h) : input.right;

			CollisionBody cbody = new CollisionBody(inputTopWorldElevated, inputleftWorldElevated, inputrightWorldElevated, inputbottomWorldElevated);
			return cbody;
		}
	}
}