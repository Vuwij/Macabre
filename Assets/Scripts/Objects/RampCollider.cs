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

		public Vector2 topLeftWorldElevated => (topWorldElevated + leftWorldElevated) / 2;
		public Vector2 topRightWorldElevated => (topWorldElevated + rightWorldElevated) / 2;
		public Vector2 bottomLeftWorldElevated => (bottomWorldElevated + leftWorldElevated) / 2;
		public Vector2 bottomRightWorldElevated => (bottomWorldElevated + rightWorldElevated) / 2;
        public Vector2 centerElevated => (topWorldElevated + bottomWorldElevated + leftWorldElevated + rightWorldElevated) / 4;

		public PixelLine centerSegment {
			get {
				if (rampDirection == Direction.NE || rampDirection == Direction.SW)
					return new PixelLine(topLeftWorldElevated, bottomRightWorldElevated);
				else
					return new PixelLine(bottomLeftWorldElevated, topRightWorldElevated);
			}
		}

		public PixelLine bottomSegment{
            get {
				if (rampDirection == Direction.NE)
					return collisionBodyWorld.lineSW;
				else if (rampDirection == Direction.NW)
					return collisionBodyWorld.lineSE;
				else if (rampDirection == Direction.SW)
					return collisionBodyWorld.lineNE;
				else if (rampDirection == Direction.SE)
					return collisionBodyWorld.lineNW;
				else return null;
            }
        }

        public PixelBox collisionBodyRampedWorld => new PixelBox(topWorldElevated, leftWorldElevated, rightWorldElevated, bottomWorldElevated);
              
		public PixelBox proximityBodyWorld {
			get {
				GameObject player = GameObject.Find("Player");
				float navMargin = 20.0f;
				if (player != null)
				{
					PixelCollider playerCollider = GameObject.Find("Player").GetComponentInChildren<PixelCollider>();
					navMargin = playerCollider.navigationMargin * 5;
				}

				if (rampDirection == Direction.NE || rampDirection == Direction.SW) {
					PixelLine pl1 = bottomSegment.Shift(Direction.NE, navMargin);
					PixelLine pl2 = bottomSegment.Shift(Direction.SW, navMargin);
					return new PixelBox(pl1.p1, pl2.p1, pl1.p2, pl2.p2);
				}
				else if (rampDirection == Direction.NW || rampDirection == Direction.SE) {
					PixelLine pl1 = bottomSegment.Shift(Direction.NW, navMargin);
					PixelLine pl2 = bottomSegment.Shift(Direction.SE, navMargin);
                    return new PixelBox(pl1.p2, pl1.p1, pl2.p2, pl2.p1);
				}
				return null;
			}
		}

		public bool OnFarSide(PixelBox body)
		{
			bool onTop = centerSegment.AboveSegment(body);
			if (Math.Abs(slope) < float.Epsilon) return false;
			if ((rampDirection == Direction.NE || rampDirection == Direction.NW) && onTop)
				return true;
			if ((rampDirection == Direction.SE || rampDirection == Direction.SW) && !onTop)
				return true;
			return false;
		}
        
		public override void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(topWorldElevated, leftWorldElevated);
			Gizmos.DrawLine(leftWorldElevated, bottomWorldElevated);
			Gizmos.DrawLine(bottomWorldElevated, rightWorldElevated);
			Gizmos.DrawLine(rightWorldElevated, topWorldElevated);
   
			Gizmos.DrawLine(centerSegment.p1, centerSegment.p2);

			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(proximityBodyWorld.top, proximityBodyWorld.right);
			Gizmos.DrawLine(proximityBodyWorld.right, proximityBodyWorld.bottom);
			Gizmos.DrawLine(proximityBodyWorld.bottom, proximityBodyWorld.left);
			Gizmos.DrawLine(proximityBodyWorld.left, proximityBodyWorld.top);

			base.OnDrawGizmos();
		}

		public PixelBox MatchCollisionBody(PixelBox input, bool center = true) {
			float h = 0.0f;
			if (rampDirection == Direction.NE || rampDirection == Direction.SW)
				h = slope * Vector2.Distance(input.bottom, input.right);
            if (rampDirection == Direction.SE || rampDirection == Direction.NW)
				h = slope * Vector2.Distance(input.bottom, input.left);
            
			Vector2 inputTopWorldElevated = (rampDirection == Direction.NW || rampDirection == Direction.NE) ? input.top + new Vector2(0.0f, h) : input.top;
			Vector2 inputbottomWorldElevated = (rampDirection == Direction.SW || rampDirection == Direction.SE) ? input.bottom + new Vector2(0.0f, h) : input.bottom;
			Vector2 inputleftWorldElevated = (rampDirection == Direction.NW || rampDirection == Direction.SW) ? input.left + new Vector2(0.0f, h) : input.left;
			Vector2 inputrightWorldElevated = (rampDirection == Direction.NE || rampDirection == Direction.SE) ? input.right + new Vector2(0.0f, h) : input.right;

			if (center) {
				inputTopWorldElevated.y = inputTopWorldElevated.y - h / 2;
				inputbottomWorldElevated.y = inputbottomWorldElevated.y - h / 2;
				inputleftWorldElevated.y = inputleftWorldElevated.y - h / 2;
				inputrightWorldElevated.y = inputrightWorldElevated.y - h / 2;
			}

			PixelBox cbody = new PixelBox(inputTopWorldElevated, inputleftWorldElevated, inputrightWorldElevated, inputbottomWorldElevated);
			return cbody;
		}

		public bool WithinRampCollider(PixelBox body) {
			return false;
		}
	}
}