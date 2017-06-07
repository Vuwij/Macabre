using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

		protected override void createShadows() {

			// Find the left and right most point
			int leftIndex = 0, rightIndex = 0;
			float leftmost = float.MaxValue;
			float rightmost = float.MinValue;
			for(int i = 0; i < wallPoints.Count(); i++) {
				if(wallPoints[i].x < leftmost) {
					leftmost = wallPoints[i].x;
					leftIndex = i;
				}
				if(wallPoints[i].x > rightmost) {
					rightmost = wallPoints[i].x;
					rightIndex = i;
				}
			}

			// Get the top array of vectors
			List<Vector2> points = new List<Vector2>();
			int higherIndex = leftIndex > rightIndex ? leftIndex : rightIndex;
			int lowerIndex = leftIndex < rightIndex ? leftIndex : rightIndex;
			for(int i = 0; i < wallPoints.Count(); i++) {
				if(i >= higherIndex || i <= lowerIndex)
					points.Add(wallPoints[i]);
			}

			// Extend a bit to the left and right
			float extend = 20.0f;
			float height = 200.0f;
			Vector2 SW = wallPoints[leftIndex] + new Vector2(-extend, 0);
			Vector2 SE = wallPoints[rightIndex] + new Vector2(extend, 0);
			Vector2 NW = SW + new Vector2(0, height);
			Vector2 NE = SE + new Vector2(0, height);

			if(higherIndex == leftIndex) {
				points.Add(SW);
				points.Add(NW);
				points.Add(NE);
				points.Add(SE);
				points.Add(wallPoints[rightIndex]);
			}

			if(higherIndex == rightIndex) {
				points.Add(SE);
				points.Add(NE);
				points.Add(NW);
				points.Add(SW);
				points.Add(wallPoints[leftIndex]);
			}

			playerShadow = gameObject.AddComponent<PolygonCollider2D>();
			playerShadow.isTrigger = true;
			playerShadow.points = points.ToArray();
		}
	}
}

