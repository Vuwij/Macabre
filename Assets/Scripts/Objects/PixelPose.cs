using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
	public enum Direction
    {
        NE, SE, NW, SW, All
    }

	public class PixelPose : MonoBehaviour
	{
		public PixelRoom pixelRoom;
        public Direction direction;
        public WayPoint wayPoint;
        public PixelPose() { }
        public PixelPose(Direction direction, WayPoint wayPoint)
        {
            this.direction = direction;
            this.wayPoint = wayPoint;
        }
	}
}