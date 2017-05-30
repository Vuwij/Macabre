using System;
using UnityEngine;

namespace Objects.Unmovable
{
	public class Exterior : Room
	{
		PolygonCollider2D exteriorWall {
			get {
				return this.GetComponent<PolygonCollider2D>();
			}
		}
		protected override Vector2[] wallPoints {
			get {
				return exteriorWall.points;
			}
		}
	}
}

