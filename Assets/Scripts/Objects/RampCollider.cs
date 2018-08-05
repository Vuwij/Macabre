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
        
		public override void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(topWorldElevated, leftWorldElevated);
			Gizmos.DrawLine(leftWorldElevated, bottomWorldElevated);
			Gizmos.DrawLine(bottomWorldElevated, rightWorldElevated);
			Gizmos.DrawLine(rightWorldElevated, topWorldElevated);

			base.OnDrawGizmos();
		}
	}
}