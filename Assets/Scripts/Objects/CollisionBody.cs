using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Objects
{
	public class CollisionBody
    {
        public Vector2 top;
        public Vector2 left;
        public Vector2 right;
        public Vector2 bottom;
		public Vector2 center => (top + left + right + bottom) / 4;
		public Vector2 topLeft => (top + left) / 2;
		public Vector2 topRight => (top + right) / 2;
		public Vector2 bottomLeft => (bottom + left) / 2;
		public Vector2 bottomRight => (bottom + right) / 2;
		public float distLeft => Vector2.Distance(topLeft, bottomRight);
		public float distRight => Vector2.Distance(topRight, bottomLeft);

		public CollisionBody() {}
		public CollisionBody(Vector2 top, Vector2 left, Vector2 right, Vector2 bottom) {
			this.top = top;
			this.left = left;
			this.right = right;
			this.bottom = bottom;
		}

		public static CollisionBodyComparison CompareTwoCollisionBodies(CollisionBody a, CollisionBody b, float margin = 0.0f, bool debug = false) {
			Vector2 atopWorld = a.top;
            Vector2 abottomWorld = a.bottom;
            Vector2 aleftWorld = a.left;
            Vector2 arightWorld = a.right;

            Vector2 btopWorld = b.top;
            Vector2 bbottomWorld = b.bottom;
            Vector2 bleftWorld = b.left;
            Vector2 brightWorld = b.right;

            CollisionBodyComparison collisionBodyComparision = new CollisionBodyComparison();

			bool aTopLeft = PixelCollider.DistanceBetween4pointsOrthographic(aleftWorld, atopWorld, bbottomWorld, brightWorld) >= -margin;
			bool aTopRight = PixelCollider.DistanceBetween4pointsOrthographic(atopWorld, arightWorld, bleftWorld, bbottomWorld) >= -margin;
			bool aBottomRight = PixelCollider.DistanceBetween4pointsOrthographic(bleftWorld, btopWorld, abottomWorld, arightWorld) >= -margin;
			bool aBottomLeft = PixelCollider.DistanceBetween4pointsOrthographic(btopWorld, brightWorld, aleftWorld, abottomWorld) >= -margin;
            
			bool bTopRightWithin = PixelCollider.DistanceBetween4pointsOrthographic(bleftWorld, bbottomWorld, aleftWorld, abottomWorld) >= -margin;
			bool bTopLeftWithin = PixelCollider.DistanceBetween4pointsOrthographic(bbottomWorld, brightWorld, abottomWorld, arightWorld) >= -margin;
			bool bBottomLeftWithin = PixelCollider.DistanceBetween4pointsOrthographic(atopWorld, arightWorld, btopWorld, brightWorld) >= -margin;
			bool bBottomRightWithin = PixelCollider.DistanceBetween4pointsOrthographic(aleftWorld, atopWorld, bleftWorld, btopWorld) >= -margin;

			collisionBodyComparision.NEinside = bTopRightWithin;
			collisionBodyComparision.NWinside = bTopLeftWithin;
			collisionBodyComparision.SEinside = bBottomRightWithin;
			collisionBodyComparision.SWinside = bBottomLeftWithin;

			// Debugging Tools
			if (debug) {
				if (bTopRightWithin) Debug.DrawLine(bbottomWorld, brightWorld, Color.blue, 1.0f);
				if (bTopLeftWithin) Debug.DrawLine(bleftWorld, bbottomWorld, Color.blue, 1.0f);
				if (bBottomLeftWithin) Debug.DrawLine(bleftWorld, btopWorld, Color.blue, 1.0f);
				if (bBottomRightWithin) Debug.DrawLine(btopWorld, brightWorld, Color.blue, 1.0f);
                
				//if (aTopLeft) Debug.DrawLine(bbottomWorld, brightWorld, Color.red, 0.3f);
				//if (aTopRight) Debug.DrawLine(bleftWorld, bbottomWorld, Color.red, 0.3f);
				//if (aBottomRight) Debug.DrawLine(bleftWorld, btopWorld, Color.red, 0.3f);
				//if (aBottomLeft) Debug.DrawLine(btopWorld, brightWorld, Color.red, 0.3f);            
			}

			// Sides Vertical
            if (aTopLeft)
                collisionBodyComparision.NWvertical |= (aleftWorld.x < brightWorld.x && aleftWorld.y < brightWorld.y);

            if (aTopRight)
                collisionBodyComparision.NEvertical |= (arightWorld.x > bleftWorld.x && arightWorld.y < bleftWorld.y);

            if (aBottomRight)
                collisionBodyComparision.SEvertical |= (bleftWorld.x < arightWorld.x && bleftWorld.y < arightWorld.y);

            if (aBottomLeft)
                collisionBodyComparision.SWvertical |= (brightWorld.x > aleftWorld.x && brightWorld.y < aleftWorld.y);
                
            // Corners Exclusive
            if (aTopLeft && aTopRight) collisionBodyComparision.Nexclusive = true;
            if (aTopLeft && aBottomLeft) collisionBodyComparision.Wexclusive = true;
            if (aBottomLeft && aBottomRight) collisionBodyComparision.Sexclusive = true;
            if (aBottomRight && aTopRight) collisionBodyComparision.Eexclusive = true;

			// Sides Inclusive
			if (aTopLeft && (bBottomLeftWithin || bTopRightWithin)) collisionBodyComparision.NWinclusive = true;
			if (aBottomRight && (bBottomLeftWithin || bTopRightWithin)) collisionBodyComparision.SEinclusive = true;
			if (aTopRight && (bTopLeftWithin || bBottomRightWithin)) collisionBodyComparision.NEinclusive = true;
			if (aBottomLeft && (bBottomRightWithin || bTopLeftWithin)) collisionBodyComparision.SWinclusive = true;

            // Sides Exclusive
            if (aTopLeft && !aTopRight && !aBottomLeft) collisionBodyComparision.NWexclusive = true;
            if (aTopRight && !aTopLeft && !aBottomRight) collisionBodyComparision.NEexclusive = true;
            if (aBottomLeft && !aBottomRight && !aTopLeft) collisionBodyComparision.SEexclusive = true;
            if (aBottomRight && !aBottomLeft && !aTopRight) collisionBodyComparision.SWexclusive = true;

			// Above and Below
			if (arightWorld.x < brightWorld.x && aleftWorld.x > bleftWorld.x && (bTopLeftWithin && bTopRightWithin))
				collisionBodyComparision.aAbove = true;

			if (arightWorld.x < brightWorld.x && aleftWorld.x > bleftWorld.x && (bBottomLeftWithin && bBottomRightWithin))
				collisionBodyComparision.aBelow = true;
            

            return collisionBodyComparision;
		}

		public CollisionBodyComparison CompareWith(CollisionBody other, float margin = 0.0f) {
			CollisionBodyComparison bodyComparison = CompareTwoCollisionBodies(this, other, margin);
                     
			return bodyComparison;
		}

        // Within Range of a collisionBody
		public bool WithinRange(CollisionBody other, Direction direction, float distance = 0.4f, float negDistance = 2.0f) {
			CollisionBodyComparison comparison = this.CompareWith(other);
            
			if (direction == Direction.NW)
			{
				if (PixelCollider.DistanceBetween4pointsOrthographic(left, top, other.bottom, other.right) < distance &&
				    PixelCollider.DistanceBetween4pointsOrthographic(left, top, other.bottom, other.right) > -negDistance &&
                    left.x < (other.right.x) && top.x > (other.bottom.x) &&
					left.y < (other.right.y) && top.y > (other.bottom.y))
					return true;
				return false;
			}
			else if (direction == Direction.NE)
			{
				if (PixelCollider.DistanceBetween4pointsOrthographic(top, right, other.left, other.bottom) < distance &&
				    PixelCollider.DistanceBetween4pointsOrthographic(top, right, other.left, other.bottom) > -negDistance &&
				    top.x < (other.bottom.x) && right.x > (other.left.x) &&
				    top.y > (other.bottom.y) && right.y < (other.left.y))
					return true;
				return false;
			}
			else if (direction == Direction.SW)
			{
				if (PixelCollider.DistanceBetween4pointsOrthographic(left, bottom, other.top, other.right) > -distance &&
				    PixelCollider.DistanceBetween4pointsOrthographic(left, bottom, other.top, other.right) < negDistance &&
					left.x < (other.right.x) && bottom.x > (other.top.x) &&
					left.y > (other.right.y) && bottom.y < (other.top.y))
					return true;
				return false;
			}
			else if (direction == Direction.SE)
			{
				if (PixelCollider.DistanceBetween4pointsOrthographic(bottom, right, other.left, other.top) > -distance &&
				    PixelCollider.DistanceBetween4pointsOrthographic(bottom, right, other.left, other.top) < negDistance &&
					bottom.x < (other.top.x) && right.x > (other.left.x) &&
					bottom.y < (other.top.y) && right.y > (other.left.y))
					return true;
				return false;
			}
			else if (direction == Direction.All)
			{
				if (PixelCollider.DistanceBetween4pointsOrthographic(left, top, other.bottom, other.right) < distance &&
				    PixelCollider.DistanceBetween4pointsOrthographic(left, top, other.bottom, other.right) > -negDistance &&
                    left.x < (other.right.x) && top.x > (other.bottom.x) &&
                    left.y < (other.right.y) && top.y > (other.bottom.y))
                    return true;
				if (PixelCollider.DistanceBetween4pointsOrthographic(top, right, other.left, other.bottom) < distance &&
				    PixelCollider.DistanceBetween4pointsOrthographic(top, right, other.left, other.bottom) > -negDistance &&
                    top.x < (other.bottom.x) && right.x > (other.left.x) &&
                    top.y > (other.bottom.y) && right.y < (other.left.y))
                    return true;
				if (PixelCollider.DistanceBetween4pointsOrthographic(left, bottom, other.top, other.right) > -distance &&
				    PixelCollider.DistanceBetween4pointsOrthographic(left, bottom, other.top, other.right) < negDistance &&
                    left.x < (other.right.x) && bottom.x > (other.top.x) &&
                    left.y > (other.right.y) && bottom.y < (other.top.y))
                    return true;
				if (PixelCollider.DistanceBetween4pointsOrthographic(bottom, right, other.left, other.top) > -distance &&
				    PixelCollider.DistanceBetween4pointsOrthographic(bottom, right, other.left, other.top) < negDistance &&
                    bottom.x < (other.top.x) && right.x > (other.left.x) &&
                    bottom.y < (other.top.y) && right.y > (other.left.y))
                    return true;
				return false;
			}
			return false;
		}

        // Within the body
		public bool WithinCollisionBody(Vector2 position, float margin = 0.0f) {
			
			if (PixelCollider.DistanceBetween4pointsOrthographic(left, top, position, position) >= margin)
                return false;

			if (PixelCollider.DistanceBetween4pointsOrthographic(top, right, position, position) >= margin)
                return false;

			if (PixelCollider.DistanceBetween4pointsOrthographic(left, bottom, position, position) <= -margin)
                return false;

			if (PixelCollider.DistanceBetween4pointsOrthographic(bottom, right, position, position) <= -margin)
                return false;

			return true;
		}

        // Draw the body
		public void Draw(Color color, float duration) {
			Debug.DrawLine(top, left, color, duration);
			Debug.DrawLine(left, bottom, color, duration);
			Debug.DrawLine(bottom, right, color, duration);
			Debug.DrawLine(right, top, color, duration);
		}
    }

    public class CollisionBodyComparison
    {
        // Vertical For Sorting
        public bool NEvertical;
        public bool NWvertical;
        public bool SEvertical;
        public bool SWvertical;

		// Inclusive means partial overlap is allowed, Exclusive means no partial overlap allowed
		public bool NEinclusive;
		public bool NWinclusive;
		public bool SEinclusive;
		public bool SWinclusive;
        
        public bool NEexclusive;
        public bool NWexclusive;
        public bool SEexclusive;
        public bool SWexclusive;

        public bool Nexclusive;
        public bool Sexclusive;
        public bool Eexclusive;
        public bool Wexclusive;

		public bool Ninclusive => Nexclusive || (NEvertical && !NEexclusive) || (NWvertical && !NWexclusive);
        public bool Sinclusive => Sexclusive || (SEvertical && !SEexclusive) || (SWvertical && !SWexclusive);
        public bool Einclusive => Eexclusive || (NEvertical && !NEexclusive) || (SEvertical && !SEexclusive);
        public bool Winclusive => Wexclusive || (NWvertical && !NWexclusive) || (SWvertical && !SWexclusive);

        // Inside means on the inside border
		public bool NEinside;
		public bool NWinside;
		public bool SEinside;
		public bool SWinside;

		public bool NEoutside => !NEinside;
		public bool NWoutside => !NWinside;
		public bool SEoutside => !SEinside;
		public bool SWoutside => !SWinside;

		public bool NEandSWinside => NEinside && SWinside;
        public bool NWandSEinside => NWinside && SEinside;

		public bool NEandSWoutside => NEoutside && SWoutside;
		public bool NWandSEoutside => NWoutside && SEoutside;

        // Above and below, for ramps
		public bool aAbove;
		public bool aBelow;

		public bool Within => aAbove && aBelow;

        public int inFront
        {
            get
            {
                if (NEvertical || NWvertical)
                    return 1;
                else if (SEvertical || SWvertical)
                    return -1;
                else return 0;
            }
        }
    }

	public struct PixelCollision
    {
        public Direction direction;
        public PixelCollider pixelCollider;
    }
}
