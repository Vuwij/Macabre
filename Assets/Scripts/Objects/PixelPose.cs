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
		public PixelPose(PixelRoom pixelRoom, Direction direction, Vector2 position)
        {
			this.pixelRoom = pixelRoom;
			this.direction = direction;
            this.position = position;
        }
	}
}