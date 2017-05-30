using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Objects.Unmovable.Building;
using Objects.Unmovable.Furniture;
using Objects.Unmovable.Path;
using UnityEngine;

namespace Objects.Unmovable
{
    public class Room : UnmovableObject
    {
		public Building.Building buildingController
        {
            get
            {
				var room = GetComponentInParent<Building.Building>();
				if (room == null) throw new Exception("Room not specified for the furniture: " + name);
                return room;
            }
        }
		public VirtualPath[] paths
		{
			get { return GetComponentsInChildren<VirtualPath>(); }
		}
		protected List<AbstractFurniture> furniture
		{
			get
			{
				return gameObject.GetComponentsInChildren<AbstractFurniture>().ToList();
			}
		}
		EdgeCollider2D wall {
			get {
				return this.GetComponent<EdgeCollider2D>();
			}
		}
		protected virtual Vector2[] wallPoints {
			get {
				return wall.points;
			}
		}

		PolygonCollider2D playerShadow; // The top part of the room where the player its located

		protected override void Start() {
			createPlayerShadow();

			base.Start();
		}

		void createPlayerShadow() {

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

		void OnTriggerEnter2D(Collider2D collider) {
			var obj = collider.gameObject.GetComponent<Movable.MovableObject>();
			if (collider.isTrigger) return;
			if (obj != null) {
				var c = spriteRenderer.color;
				c.a = 0.0f;
				spriteRenderer.color = c;
			}
		}

		void OnTriggerExit2D(Collider2D collider) {
			var obj = collider.gameObject.GetComponent<Movable.MovableObject>();
			if (collider.isTrigger) return;
			if (obj != null) {
				var c = spriteRenderer.color;
				c.a = 1.0f;
				spriteRenderer.color = c;
			}
		}
    }
}
