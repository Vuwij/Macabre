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
        
		public Vector2 topWorldElevated => (rampDirection == Direction.NE || rampDirection == Direction.NW) ? topWorld + new Vector2(0.0f, height) : topWorld;
		public Vector2 bottomWorldElevated => (rampDirection == Direction.SE || rampDirection == Direction.SW) ? bottomWorld + new Vector2(0.0f, height) : bottomWorld;
		public Vector2 leftWorldElevated => (rampDirection == Direction.NE || rampDirection == Direction.SE) ? leftWorld + new Vector2(0.0f, height) : leftWorld;
		public Vector2 rightWorldElevated => (rampDirection == Direction.NW || rampDirection == Direction.NW) ? rightWorld + new Vector2(0.0f, height) : rightWorld;
        
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