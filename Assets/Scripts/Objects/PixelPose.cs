using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
	public enum Direction
    {
        NE, SE, NW, SW, All
    }

	public class PixelPose
	{
		public PixelRoom pixelRoom;
        public Direction direction;
		public Vector2 position;
        public PixelPose() { }
		public PixelPose(PixelPose pose) {
			this.pixelRoom = pose.pixelRoom;
			this.direction = pose.direction;
			this.position = pose.position;
		}
		public PixelPose(PixelRoom pixelRoom, Direction direction, Vector2 position)
        {
			this.pixelRoom = pixelRoom;
			this.direction = direction;
            this.position = position;
        }

		public PixelPose Flip() {
			PixelPose p = new PixelPose(this);
			if (p.direction == Direction.NE)
				p.direction = Direction.SW;
			if (p.direction == Direction.SW)
                p.direction = Direction.NE;
			if (p.direction == Direction.NW)
                p.direction = Direction.SE;
			if (p.direction == Direction.SE)
				p.direction = Direction.NW;
			return p;
		}

		public PixelPose TranslatePose(float distance, Direction transDirection = Direction.All) {
			if (transDirection == Direction.All)
				transDirection = this.direction;
			
			PixelPose p = new PixelPose(this);
			if(transDirection == Direction.NE)
				p.position += new Vector2(distance / Mathf.Sqrt(3), distance / Mathf.Sqrt(3) / 2);
			if(transDirection == Direction.NW)
                p.position += new Vector2(-distance / Mathf.Sqrt(3), distance / Mathf.Sqrt(3) / 2);
			if(transDirection == Direction.SE)
                p.position += new Vector2(distance / Mathf.Sqrt(3), -distance / Mathf.Sqrt(3) / 2);
			if(transDirection == Direction.SW)
                p.position += new Vector2(-distance / Mathf.Sqrt(3), -distance / Mathf.Sqrt(3) / 2);
			return p;
		}
	}
}