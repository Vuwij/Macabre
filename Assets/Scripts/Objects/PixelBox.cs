using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Objects
{
	public class PixelBox
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

		public PixelLine lineNE => new PixelLine(top, right);
		public PixelLine lineNW => new PixelLine(left, top);
		public PixelLine lineSE => new PixelLine(bottom, right);
		public PixelLine lineSW => new PixelLine(left, bottom);

		public PixelBox() {}
		public PixelBox(PixelBox body) {
			this.top = body.top;
			this.left = body.left;
			this.right = body.right;
			this.bottom = body.bottom;
		}
		public PixelBox(Vector2 top, Vector2 left, Vector2 right, Vector2 bottom) {
			this.top = top;
			this.left = left;
			this.right = right;
			this.bottom = bottom;
		}

		public static PixelBoxComparison CompareTwoCollisionBodies(PixelBox a, PixelBox b, float margin = 0.0f, bool debug = false) {
			Vector2 atopWorld = a.top;
            Vector2 abottomWorld = a.bottom;
            Vector2 aleftWorld = a.left;
            Vector2 arightWorld = a.right;

            Vector2 btopWorld = b.top;
            Vector2 bbottomWorld = b.bottom;
            Vector2 bleftWorld = b.left;
            Vector2 brightWorld = b.right;

            PixelBoxComparison collisionBodyComparision = new PixelBoxComparison();

			bool aTopLeft = PixelLine.DistanceOrthographic(a.lineNW, b.lineSE) >= -margin;
			bool aTopRight = PixelLine.DistanceOrthographic(a.lineNE, b.lineSW) >= -margin;
			bool aBottomRight = PixelLine.DistanceOrthographic(b.lineNW, a.lineSE) >= -margin;
			bool aBottomLeft = PixelLine.DistanceOrthographic(b.lineNE, a.lineSW) >= -margin;

			bool aTopRightWithin = PixelLine.DistanceOrthographic(b.lineNE, a.lineNE) <= margin;
			bool aTopLeftWithin = PixelLine.DistanceOrthographic(b.lineNW, a.lineNW) <= margin;
			bool aBottomLeftWithin = PixelLine.DistanceOrthographic(a.lineSW, b.lineSW) <= margin;
			bool aBottomRightWithin = PixelLine.DistanceOrthographic(a.lineSE, b.lineSE) <= margin;
            
			collisionBodyComparision.NEinside = aTopRightWithin;
			collisionBodyComparision.NWinside = aTopLeftWithin;
			collisionBodyComparision.SEinside = aBottomRightWithin;
			collisionBodyComparision.SWinside = aBottomLeftWithin;

			collisionBodyComparision.NEoutside = aBottomLeft;
			collisionBodyComparision.NWoutside = aBottomRight;
			collisionBodyComparision.SEoutside = aTopLeft;
			collisionBodyComparision.SWoutside = aTopRight;


			// Debugging Tools
			if (debug) {
				if (aTopRightWithin) b.lineNE.Draw(Color.blue, 1.0f);
				if (aTopLeftWithin) b.lineNW.Draw(Color.blue, 1.0f);
				if (aBottomLeftWithin) b.lineSW.Draw(Color.blue, 1.0f);
				if (aBottomRightWithin) b.lineSE.Draw(Color.blue, 1.0f);
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
			if (aTopLeft && (aBottomLeftWithin || aTopRightWithin)) collisionBodyComparision.NWinclusive = true;
			if (aBottomRight && (aBottomLeftWithin || aTopRightWithin)) collisionBodyComparision.SEinclusive = true;
			if (aTopRight && (aTopLeftWithin || aBottomRightWithin)) collisionBodyComparision.NEinclusive = true;
			if (aBottomLeft && (aBottomRightWithin || aTopLeftWithin)) collisionBodyComparision.SWinclusive = true;

            // Sides Exclusive
            if (aTopLeft && !aTopRight && !aBottomLeft) collisionBodyComparision.NWexclusive = true;
            if (aTopRight && !aTopLeft && !aBottomRight) collisionBodyComparision.NEexclusive = true;
            if (aBottomLeft && !aBottomRight && !aTopLeft) collisionBodyComparision.SEexclusive = true;
            if (aBottomRight && !aBottomLeft && !aTopRight) collisionBodyComparision.SWexclusive = true;

			// Above and Below
			if (arightWorld.x < brightWorld.x && aleftWorld.x > bleftWorld.x && (aBottomLeftWithin && aBottomRightWithin))
				collisionBodyComparision.aAbove = true;

			if (arightWorld.x < brightWorld.x && aleftWorld.x > bleftWorld.x && (aTopLeftWithin && aTopRightWithin))
				collisionBodyComparision.aBelow = true;
                     
            return collisionBodyComparision;
		}

		public PixelBoxComparison CompareWith(PixelBox other, float margin = 0.0f) {
			PixelBoxComparison bodyComparison = CompareTwoCollisionBodies(this, other, margin);
                     
			return bodyComparison;
		}

        // Within Range of a collisionBody
		public bool WithinRange(PixelBox other, Direction direction, float distance = 0.4f, float negDistance = 2.0f) {
			PixelBoxComparison comparison = this.CompareWith(other);
            
			if (direction == Direction.NW)
			{
				if (PixelLine.DistanceOrthographic(lineNW, other.lineSE) < distance &&
				    PixelLine.DistanceOrthographic(lineNW, other.lineSE) > -negDistance &&
                    left.x < (other.right.x) && top.x > (other.bottom.x) &&
					left.y < (other.right.y) && top.y > (other.bottom.y))
					return true;
				return false;
			}
			else if (direction == Direction.NE)
			{
				if (PixelLine.DistanceOrthographic(lineNE, other.lineSW) < distance &&
				    PixelLine.DistanceOrthographic(lineNE, other.lineSW) > -negDistance &&
				    top.x < (other.bottom.x) && right.x > (other.left.x) &&
				    top.y > (other.bottom.y) && right.y < (other.left.y))
					return true;
				return false;
			}
			else if (direction == Direction.SW)
			{
				if (PixelLine.DistanceOrthographic(lineSW, other.lineNE) > -distance &&
				    PixelLine.DistanceOrthographic(lineSW, other.lineNE) < negDistance &&
					left.x < (other.right.x) && bottom.x > (other.top.x) &&
					left.y > (other.right.y) && bottom.y < (other.top.y))
					return true;
				return false;
			}
			else if (direction == Direction.SE)
			{
				if (PixelLine.DistanceOrthographic(lineSE, other.lineNW) > -distance &&
				    PixelLine.DistanceOrthographic(lineSE, other.lineNW) < negDistance &&
					bottom.x < (other.top.x) && right.x > (other.left.x) &&
					bottom.y < (other.top.y) && right.y > (other.left.y))
					return true;
				return false;
			}
			else if (direction == Direction.All)
			{
				if (PixelLine.DistanceOrthographic(lineNW, other.lineSE) < distance &&
				    PixelLine.DistanceOrthographic(lineNW, other.lineSE) > -negDistance &&
                    left.x < (other.right.x) && top.x > (other.bottom.x) &&
                    left.y < (other.right.y) && top.y > (other.bottom.y))
                    return true;
				if (PixelLine.DistanceOrthographic(lineNE, other.lineSW) < distance &&
				    PixelLine.DistanceOrthographic(lineNE, other.lineSW) > -negDistance &&
                    top.x < (other.bottom.x) && right.x > (other.left.x) &&
                    top.y > (other.bottom.y) && right.y < (other.left.y))
                    return true;
				if (PixelLine.DistanceOrthographic(lineSW, other.lineNE) > -distance &&
				    PixelLine.DistanceOrthographic(lineSW, other.lineNE) < negDistance &&
                    left.x < (other.right.x) && bottom.x > (other.top.x) &&
                    left.y > (other.right.y) && bottom.y < (other.top.y))
                    return true;
				if (PixelLine.DistanceOrthographic(lineSE, other.lineNW) > -distance &&
				    PixelLine.DistanceOrthographic(lineSE, other.lineNW) < negDistance &&
                    bottom.x < (other.top.x) && right.x > (other.left.x) &&
                    bottom.y < (other.top.y) && right.y > (other.left.y))
                    return true;
				return false;
			}
			return false;
		}

        // Within the body
		public bool WithinCollisionBody(Vector2 position, float margin = 0.0f) {
			
			if (PixelLine.DistanceOrthographic(lineNW, position) >= margin)
                return false;

            if (PixelLine.DistanceOrthographic(lineNE, position) >= margin)
                return false;

            if (PixelLine.DistanceOrthographic(lineSW, position) <= -margin)
                return false;

            if (PixelLine.DistanceOrthographic(lineSE, position) <= -margin)
                return false;

			return true;
		}

        // Extend and Stretch and increase size of collision Body
		public PixelBox Extend(Direction direction, float distance) {
			PixelBox copy = new PixelBox(this);
			return copy;
		}

        // Draw the body
		public void Draw(Color color, float duration) {
			Debug.DrawLine(top, left, color, duration);
			Debug.DrawLine(left, bottom, color, duration);
			Debug.DrawLine(bottom, right, color, duration);
			Debug.DrawLine(right, top, color, duration);
		}
    }
}
