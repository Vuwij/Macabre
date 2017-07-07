using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Objects.Immovable.Rooms
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

		PolygonCollider2D playerShadow; // The top part of the room where the player its located
		float debounceTriggerTime;
		Material shadowMaterial;

		protected override void Start() {
			shadowMaterial = Resources.Load("Materials/Shadow", typeof(Material)) as Material;
			createShadows();
			base.Start();
		}

		protected void createShadows() {
			if(exteriorWall == null) return;

			// Find the left and right most point
			List<Vector2[]> wallPaths = new List<Vector2[]>();
			for(int p = 0; p < exteriorWall.pathCount; p++) {
				int leftIndex = 0, rightIndex = 0;
				float leftmost = float.MaxValue;
				float rightmost = float.MinValue;

				Vector2[] wallPoints = exteriorWall.GetPath(p);

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

				wallPaths.Add(points.ToArray());
			}

			playerShadow = gameObject.AddComponent<PolygonCollider2D>();
			playerShadow.isTrigger = true;
			playerShadow.pathCount = wallPaths.Count;
			for (int p = 0; p < wallPaths.Count; p++) {
				playerShadow.SetPath(p, wallPaths[p]);
			}
		}

		void OnTriggerEnter2D(Collider2D collider) {
			// Debounces trigger
			if(Time.time - debounceTriggerTime < 0.1) return;
			debounceTriggerTime = Time.time;

			var obj = collider.gameObject.GetComponent<Movable.MovableObject>();
			if (collider.isTrigger) return;
			if (obj != null) {
				SetOpacity(0.3f);
			}

			// Creates the child shadow
			var childShadow = new GameObject();
			childShadow.name = this.name + " Shadow";
			childShadow.transform.parent = transform;
			childShadow.transform.position = transform.position + new Vector3(0, 0, -1);
			var polygon = childShadow.AddComponent<Polygon>();
			polygon.pc2.points = wallPoints;
			polygon.mr.material = shadowMaterial;
			polygon.Refresh();
		}

		void OnTriggerExit2D(Collider2D collider) {
			var obj = collider.gameObject.GetComponent<Movable.MovableObject>();
			if (collider.isTrigger) return;
			if (obj != null) {
				SetOpacity(0.3f);
				foreach(var room in sharedRooms) {
					room.SetOpacity(0.3f);
				}
			}
			GameObject.Destroy(GameObject.Find(this.name + " Shadow"));
		}
	}
}

