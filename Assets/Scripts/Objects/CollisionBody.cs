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

		public CollisionBody(Vector2 top, Vector2 left, Vector2 right, Vector2 bottom) {
			this.top = top;
			this.left = left;
			this.right = right;
			this.bottom = bottom;
		}

		public static CollisionBodyComparison CompareTwoCollisionBodies(CollisionBody a, CollisionBody b, float margin = 0.0f) {
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

			bool aTopLeftWithin = PixelCollider.DistanceBetween4pointsOrthographic(aleftWorld, atopWorld, bleftWorld, btopWorld) >= -margin;
			bool aTopRightWithin = PixelCollider.DistanceBetween4pointsOrthographic(atopWorld, arightWorld, btopWorld, brightWorld) >= -margin;
			bool aBottomRightWithin = PixelCollider.DistanceBetween4pointsOrthographic(bleftWorld, btopWorld, aleftWorld, atopWorld) >= -margin;
			bool aBottomLeftWithin = PixelCollider.DistanceBetween4pointsOrthographic(btopWorld, brightWorld, atopWorld, arightWorld) >= -margin;

            // Sides Inclusive
            if (aTopLeft)
                collisionBodyComparision.NWinclusive |= (aleftWorld.x < brightWorld.x && aleftWorld.y < brightWorld.y);

            if (aTopRight)
                collisionBodyComparision.NEinclusive |= (arightWorld.x > bleftWorld.x && arightWorld.y < bleftWorld.y);

            if (aBottomRight)
                collisionBodyComparision.SEinclusive |= (bleftWorld.x < arightWorld.x && bleftWorld.y < arightWorld.y);

            if (aBottomLeft)
                collisionBodyComparision.SWinclusive |= (brightWorld.x > aleftWorld.x && brightWorld.y < aleftWorld.y);

            // Corners Exclusive
            if (aTopLeft && aTopRight) collisionBodyComparision.Nexclusive = true;
            if (aTopLeft && aBottomLeft) collisionBodyComparision.Wexclusive = true;
            if (aBottomLeft && aBottomRight) collisionBodyComparision.Sexclusive = true;
            if (aBottomRight && aTopRight) collisionBodyComparision.Eexclusive = true;

            // Sides Exclusive
            if (aTopLeft && !aTopRight && !aBottomLeft) collisionBodyComparision.NWexclusive = true;
            if (aTopRight && !aTopLeft && !aBottomRight) collisionBodyComparision.NEexclusive = true;
            if (aBottomLeft && !aBottomRight && !aTopLeft) collisionBodyComparision.SEexclusive = true;
            if (aBottomRight && !aBottomLeft && !aTopRight) collisionBodyComparision.SWexclusive = true;

			// Above and Below
			if (arightWorld.x < brightWorld.x && aleftWorld.x > bleftWorld.x && (!aBottomLeft || !aBottomRight))
				collisionBodyComparision.Above = true;

			if (arightWorld.x < brightWorld.x && aleftWorld.x > bleftWorld.x && (!aTopLeft || !aTopRight))
				collisionBodyComparision.Below = true;
                     
            return collisionBodyComparision;
		}

		public CollisionBodyComparison CompareWith(CollisionBody other, float margin = 0.0f) {
			CollisionBodyComparison bodyComparison = CompareTwoCollisionBodies(this, other, margin);
                     
			return bodyComparison;
		}

        // Within Range of a collisionBody
		public bool WithinRange(CollisionBody other, Direction direction, float distance = 0.4f) {
			if (direction == Direction.NW)
			{
				if (PixelCollider.DistanceBetween4pointsOrthographic(left, top, other.bottom, other.right) < distance &&
				    PixelCollider.DistanceBetween4pointsOrthographic(left, top, other.bottom, other.right) > -2.0 &&
					left.x < (other.right.x) && top.x > (other.bottom.x) &&
					left.y < (other.right.y) && top.y > (other.bottom.y))
					return true;
				return false;
			}
			else if (direction == Direction.NE)
			{
				if (PixelCollider.DistanceBetween4pointsOrthographic(top, right, other.left, other.bottom) < distance &&
				    PixelCollider.DistanceBetween4pointsOrthographic(top, right, other.left, other.bottom) > -2.0 &&
				    top.x < (other.bottom.x) && right.x > (other.left.x) &&
				    top.y > (other.bottom.y) && right.y < (other.left.y))
					return true;
				return false;
			}
			else if (direction == Direction.SW)
			{
				if (PixelCollider.DistanceBetween4pointsOrthographic(left, bottom, other.top, other.right) > -distance &&
				    PixelCollider.DistanceBetween4pointsOrthographic(left, bottom, other.top, other.right) < 2.0 &&
					left.x < (other.right.x) && bottom.x > (other.top.x) &&
					left.y > (other.right.y) && bottom.y < (other.top.y))
					return true;
				return false;
			}
			else if (direction == Direction.SE)
			{
				if (PixelCollider.DistanceBetween4pointsOrthographic(bottom, right, other.left, other.top) > -distance &&
				    PixelCollider.DistanceBetween4pointsOrthographic(bottom, right, other.left, other.top) < 2.0 &&
					bottom.x < (other.top.x) && right.x > (other.left.x) &&
					bottom.y < (other.top.y) && right.y > (other.left.y))
					return true;
				return false;
			}
			else if (direction == Direction.All)
			{
				if (PixelCollider.DistanceBetween4pointsOrthographic(left, top, other.bottom, other.right) < distance &&
				    PixelCollider.DistanceBetween4pointsOrthographic(left, top, other.bottom, other.right) > -2.0 &&
                    left.x < (other.right.x) && top.x > (other.bottom.x) &&
                    left.y < (other.right.y) && top.y > (other.bottom.y))
                    return true;
				if (PixelCollider.DistanceBetween4pointsOrthographic(top, right, other.left, other.bottom) < distance &&
				    PixelCollider.DistanceBetween4pointsOrthographic(top, right, other.left, other.bottom) > -2.0 &&
                    top.x < (other.bottom.x) && right.x > (other.left.x) &&
                    top.y > (other.bottom.y) && right.y < (other.left.y))
                    return true;
				if (PixelCollider.DistanceBetween4pointsOrthographic(left, bottom, other.top, other.right) > -distance &&
				    PixelCollider.DistanceBetween4pointsOrthographic(left, bottom, other.top, other.right) < 2.0 &&
                    left.x < (other.right.x) && bottom.x > (other.top.x) &&
                    left.y > (other.right.y) && bottom.y < (other.top.y))
                    return true;
				if (PixelCollider.DistanceBetween4pointsOrthographic(bottom, right, other.left, other.top) > -distance &&
				    PixelCollider.DistanceBetween4pointsOrthographic(bottom, right, other.left, other.top) < 2.0 &&
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

        public bool Ninclusive => Nexclusive || (NEinclusive && !NEexclusive) || (NWinclusive && !NWexclusive);
        public bool Sinclusive => Sexclusive || (SEinclusive && !SEexclusive) || (SWinclusive && !SWexclusive);
        public bool Einclusive => Eexclusive || (NEinclusive && !NEexclusive) || (SEinclusive && !SEexclusive);
        public bool Winclusive => Wexclusive || (NWinclusive && !NWexclusive) || (SWinclusive && !SWexclusive);

		public bool Above;
		public bool Below;

        public int inFront
        {
            get
            {
                if (NEinclusive || NWinclusive)
                    return 1;
                else if (SEinclusive || SWinclusive)
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
