using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Objects
{
	public class PixelCollider : MonoBehaviour, IComparable<PixelCollider>
	{
		public struct CollisionBody
		{
			public Vector2 top;
			public Vector2 left;
			public Vector2 right;
			public Vector2 bottom;
		}

		public class MovementRestriction
		{
			public bool restrictNW = false;
			public bool restrictNE = false;
			public bool restrictSW = false;
			public bool restrictSE = false;
		}

		public Vector2 topLeft => (top + left) / 2;
		public Vector2 topRight => (top + right) / 2;
		public Vector2 bottomLeft => (bottom + left) / 2;
		public Vector2 bottomRight => (bottom + right) / 2;
		public Vector2 center => (top + left + right + bottom) / 4;

		public Vector2 topLeftWorld => topLeft + (Vector2)transform.position;
		public Vector2 topRightWorld => topRight + (Vector2)transform.position;
		public Vector2 bottomLeftWorld => bottomLeft + (Vector2)transform.position;
		public Vector2 bottomRightWorld => bottomRight + (Vector2)transform.position;
		public Vector2 centerWorld => center + (Vector2)transform.position;
		public Vector2 topWorld => top + (Vector2)transform.position;
		public Vector2 bottomWorld => bottom + (Vector2)transform.position;
		public Vector2 leftWorld => left + (Vector2)transform.position;
		public Vector2 rightWorld => right + (Vector2)transform.position;
		public Vector2 topPWorld => topP + (Vector2)transform.position;
		public Vector2 bottomPWorld => bottomP + (Vector2)transform.position;
		public Vector2 leftPWorld => leftP + (Vector2)transform.position;
		public Vector2 rightPWorld => rightP + (Vector2)transform.position;

		protected new PolygonCollider2D collider2D;

		Vector2 top, bottom, left, right;
		Vector2[] colliderPoints;

		public bool noSorting;
		public bool noCollision;
		public float visibilityInFront = 0.25f;
		Color originalColor;
		Color originalObjColor;
		public bool inspectChildObjects = false;

		protected int pixelProximity = 4; // 3 pixels away from the object

		Vector2 topP => top + new Vector2(0, pixelProximity);
		Vector2 bottomP => bottom + new Vector2(0, -pixelProximity);
		Vector2 leftP => left + new Vector2(-2 * pixelProximity, 0);
		Vector2 rightP => right + new Vector2(2 * pixelProximity, 0);

		protected virtual void Awake()
		{
			if (noCollision) return;

			collider2D = GetComponent<PolygonCollider2D>();

			Debug.Assert(collider2D != null);
			Debug.Assert(collider2D.points.Length == 4);
			Debug.Assert(transform.parent.GetComponent<PixelRoom>() == null);
			Debug.Assert(transform.parent.GetComponent<PolygonCollider2D>() == null);

			colliderPoints = collider2D.points;

			top = colliderPoints[0];
			bottom = colliderPoints[0];
			left = colliderPoints[0];
			right = colliderPoints[0];

			for (int i = 0; i < 4; ++i)
			{
				if (colliderPoints[i].y > top.y)
					top = colliderPoints[i];
				if (colliderPoints[i].y < bottom.y)
					bottom = colliderPoints[i];
				if (colliderPoints[i].x < left.x)
					left = colliderPoints[i];
				if (colliderPoints[i].x > right.x)
					right = colliderPoints[i];
			}

			top += collider2D.offset;
			bottom += collider2D.offset;
			left += collider2D.offset;
			right += collider2D.offset;
		}

		public void TopologicalSortNearbySortingLayers()
		{
			Vector3 castStart = transform.position;
			castStart.z = -10.0f;

			RaycastHit2D[] castStar = Physics2D.CircleCastAll(castStart, GameSettings.inspectRadius * 23, Vector2.zero);
			List<PixelCollider> pixelColliders = new List<PixelCollider>();

			foreach (RaycastHit2D raycastHit in castStar)
			{
				PixelCollider otherPixelCollider = raycastHit.collider.GetComponent<PixelCollider>();
				if (otherPixelCollider == null) continue;
				if (otherPixelCollider.ParentIsContainer()) continue;
				if (otherPixelCollider.noSorting) continue;
				if (otherPixelCollider.noCollision) continue;
				if (!OtherPixelColliderSameParent(otherPixelCollider)) continue;
				pixelColliders.Add(otherPixelCollider);
			}

			pixelColliders = TopologicalSort(pixelColliders);
			pixelColliders.Reverse();

			Transform previousTransform = null;

			for (int i = 0; i < pixelColliders.Count; ++i)
			{
				SpriteRenderer sr = pixelColliders[i].transform.parent.GetComponentInChildren<SpriteRenderer>();
				if (sr == null) continue;
				sr.sortingOrder = i * 2;

				// Child objects
				SpriteRenderer[] childobjects = pixelColliders[i].transform.parent.GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer co in childobjects)
				{
					if (co == sr) continue;
					co.sortingOrder = i * 2 + 1;
				}

				//if (previousTransform != null && sr.gameObject.transform != previousTransform) {
				//    Debug.DrawLine(previousTransform.position, sr.gameObject.transform.position, Color.red, 10.0f);
				//}

				previousTransform = sr.gameObject.transform;
				//Debug.Log(sr.gameObject.name + " " + sr.sortingOrder.ToString());
			}

			// Place the player in front of colliding objects
			PixelCollider player = pixelColliders.Find((obj) => obj.transform.parent.tag == "Player");
			if (player != null)
			{
				for (int i = 0; i < pixelColliders.Count; ++i)
				{
					int comparison = player.CompareTo(pixelColliders[i]);
					if (pixelColliders[i] == player) continue;
					if (comparison == -1)
					{
						SpriteRenderer sr = pixelColliders[i].transform.parent.GetComponentInChildren<SpriteRenderer>();
						if (sr == null) continue;
						Color color = sr.color;
						color.a = pixelColliders[i].visibilityInFront;
						sr.color = color;

						// Child objects
						SpriteRenderer[] childobjects = pixelColliders[i].transform.parent.GetComponentsInChildren<SpriteRenderer>();
						foreach (SpriteRenderer co in childobjects)
						{
							if (co == sr) continue;
							Color cocolor = co.color;
							if (co.name == "Footprint")
							{
								if (co.transform.parent.GetComponent<PixelExterior>() != null)
									cocolor.a = 0.8f;
							}
							else
								cocolor.a = pixelColliders[i].visibilityInFront;
							co.color = cocolor;
						}

						// If the object is a room, hide the insides of the room
						PixelExterior exterior = pixelColliders[i].transform.parent.GetComponent<PixelExterior>();
						if (exterior != null)
							exterior.HideAllRooms();

					}
					else
					{
						SpriteRenderer sr = pixelColliders[i].transform.parent.GetComponentInChildren<SpriteRenderer>();
						if (sr == null) continue;
						Color color = sr.color;
						color.a = 1;
						sr.color = color;

						// Child objects
						SpriteRenderer[] childobjects = pixelColliders[i].transform.parent.GetComponentsInChildren<SpriteRenderer>();
						foreach (SpriteRenderer co in childobjects)
						{
							if (co == sr) continue;
							Color cocolor = co.color;
							if (co.name == "Footprint")
							{
								if (co.transform.parent.GetComponent<PixelExterior>() != null)
									cocolor.a = 0.0f;
								else
									cocolor.a = 0.3f;
							}
							else
								cocolor.a = 1;
							co.color = cocolor;
						}

						// If the object is a room, hide the insides of the room
						PixelExterior exterior = pixelColliders[i].transform.parent.GetComponent<PixelExterior>();
						if (exterior != null)
							exterior.ShowAllRooms();
					}
				}
			}
		}

		public List<PixelCollider> TopologicalSort(List<PixelCollider> pixelColliders)
		{

			// Find adjacency list
			List<KeyValuePair<PixelCollider, PixelCollider>> adjacencyList = new List<KeyValuePair<PixelCollider, PixelCollider>>();

			for (int i = 0; i < pixelColliders.Count; ++i)
			{
				for (int j = 0; j < pixelColliders.Count; ++j)
				{
					if (i >= j) continue;
					int comparison = pixelColliders[i].CompareTo(pixelColliders[j]);
					if (comparison == 1)
					{
						adjacencyList.Add(new KeyValuePair<PixelCollider, PixelCollider>(pixelColliders[i], pixelColliders[j]));
					}
					else if (comparison == -1)
					{
						adjacencyList.Add(new KeyValuePair<PixelCollider, PixelCollider>(pixelColliders[j], pixelColliders[i]));
					}
				}
			}

			//foreach(var adj in adjacencyList) {
			//    Debug.Log(adj.Key.transform.parent.name + " -> " + adj.Value.transform.parent.name);            
			//}

			// Kahn's Algorithm
			List<PixelCollider> sortedPixelColliders = new List<PixelCollider>();
			List<PixelCollider> noIncomingEdgeColliders = new List<PixelCollider>();

			for (int i = 0; i < pixelColliders.Count; ++i)
			{
				if (!adjacencyList.Any((x) => x.Value == pixelColliders[i]))
					noIncomingEdgeColliders.Add(pixelColliders[i]);
			}

			while (noIncomingEdgeColliders.Count > 0)
			{
				PixelCollider first = noIncomingEdgeColliders[0];
				noIncomingEdgeColliders.Remove(first);
				sortedPixelColliders.Add(first);

				List<KeyValuePair<PixelCollider, PixelCollider>> objs = adjacencyList.Where((x) => x.Key == first).ToList();
				for (int i = 0; i < objs.Count(); ++i)
				{
					adjacencyList.Remove(objs[i]);
					if (!adjacencyList.Any((x) => x.Value == objs[i].Value))
					{
						noIncomingEdgeColliders.Add(objs[i].Value);
					}
				}
			}

			return sortedPixelColliders;
		}

		// Finds nearest other pixel collider, same as check for collision, but returns a list of objects instead
		public List<PixelCollision> CheckForInspection()
		{
			List<PixelCollision> pixelCollisions = new List<PixelCollision>();

			Vector3 castStart = transform.position;
			castStart.z = -10.0f;

			RaycastHit2D[] castStar = Physics2D.CircleCastAll(castStart, GameSettings.inspectRadius, Vector2.zero);

			foreach (RaycastHit2D raycastHit in castStar)
			{
				PixelCollider otherPixelCollider = raycastHit.collider.GetComponent<PixelCollider>();
				if (otherPixelCollider == null) continue;
				if (otherPixelCollider.noCollision) continue;
				if (otherPixelCollider.ParentIsContainer()) continue;
				if (!OtherPixelColliderSameParent(otherPixelCollider)) continue;
				if (otherPixelCollider is MultiBodyPixelCollider) continue;

				Transform otherTransform = otherPixelCollider.gameObject.transform;

				Debug.Assert(otherPixelCollider.colliderPoints.Length == 4);

				Vector2 othertopWorld = otherPixelCollider.topWorld;
				Vector2 otherbottomWorld = otherPixelCollider.bottomWorld;
				Vector2 otherleftWorld = otherPixelCollider.leftWorld;
				Vector2 otherrightWorld = otherPixelCollider.rightWorld;

				Debug.DrawLine(othertopWorld, otherleftWorld, Color.yellow, 1.0f);
				Debug.DrawLine(otherleftWorld, otherbottomWorld, Color.yellow, 1.0f);
				Debug.DrawLine(otherbottomWorld, otherrightWorld, Color.yellow, 1.0f);
				Debug.DrawLine(otherrightWorld, othertopWorld, Color.yellow, 1.0f);

				Direction direction = Direction.All;
				List<PixelCollider> pixelColliders = new List<PixelCollider>();

				if (DistanceBetween4points(leftWorld, topWorld, otherbottomWorld, otherrightWorld) < 2 &&
					DistanceBetween4points(leftWorld, topWorld, otherbottomWorld, otherrightWorld) > -10.0 &&
					leftWorld.x < (otherrightWorld.x) && topWorld.x > (otherbottomWorld.x) &&
					leftWorld.y < (otherrightWorld.y) && topWorld.y > (otherbottomWorld.y))
				{
					direction = Direction.NW;
					pixelColliders.Add(otherPixelCollider);
					pixelColliders.AddRange(otherPixelCollider.GetChildColliders());
				}
				else if (DistanceBetween4points(topWorld, rightWorld, otherleftWorld, otherbottomWorld) < 2 &&
						 DistanceBetween4points(topWorld, rightWorld, otherleftWorld, otherbottomWorld) > -10.0 &&
						 topWorld.x < (otherbottomWorld.x) && rightWorld.x > (otherleftWorld.x) &&
						 topWorld.y > (otherbottomWorld.y) && rightWorld.y < (otherleftWorld.y))
				{
					direction = Direction.NE;
					pixelColliders.Add(otherPixelCollider);
					pixelColliders.AddRange(otherPixelCollider.GetChildColliders());
				}
				else if (DistanceBetween4points(leftWorld, bottomWorld, othertopWorld, otherrightWorld) > -2 &&
						 DistanceBetween4points(leftWorld, bottomWorld, othertopWorld, otherrightWorld) < 10.0 &&
						 leftWorld.x < (otherrightWorld.x) && bottomWorld.x > (othertopWorld.x) &&
						 leftWorld.y > (otherrightWorld.y) && bottomWorld.y < (othertopWorld.y))
				{
					direction = Direction.SW;
					pixelColliders.Add(otherPixelCollider);
					pixelColliders.AddRange(otherPixelCollider.GetChildColliders());
				}
				else if (DistanceBetween4points(bottomWorld, rightWorld, otherleftWorld, othertopWorld) > -2 &&
						 DistanceBetween4points(bottomWorld, rightWorld, otherleftWorld, othertopWorld) < 10.0 &&
						 bottomWorld.x < (othertopWorld.x) && rightWorld.x > (otherleftWorld.x) &&
						 bottomWorld.y < (othertopWorld.y) && rightWorld.y > (otherleftWorld.y))
				{
					direction = Direction.SE;
					pixelColliders.Add(otherPixelCollider);
					pixelColliders.AddRange(otherPixelCollider.GetChildColliders());
				}

				foreach (PixelCollider pc in pixelColliders)
				{
					PixelCollision pixelCollision;
					pixelCollision.direction = direction;
					pixelCollision.pixelCollider = pc;
					pixelCollisions.Add(pixelCollision);

				}
			}

			return pixelCollisions;
		}

		public virtual MovementRestriction CheckForCollision()
		{
			Debug.Assert(!(this is MultiBodyPixelCollider));

			Vector3 castStart = transform.position;
			castStart.z = -10.0f;

			RaycastHit2D[] castStar = Physics2D.CircleCastAll(castStart, GameSettings.inspectRadius, Vector2.zero);

			bool restrictNW = false;
			bool restrictNE = false;
			bool restrictSW = false;
			bool restrictSE = false;

			// Collided with other object
			foreach (RaycastHit2D raycastHit in castStar)
			{
				PixelCollider otherPixelCollider = raycastHit.collider.GetComponent<PixelCollider>();
				if (otherPixelCollider == null) continue;
				if (otherPixelCollider.noCollision) continue;
				if (otherPixelCollider.ParentIsContainer()) continue;
				if (!OtherPixelColliderSameParent(otherPixelCollider)) continue;

				Transform otherTransform = otherPixelCollider.gameObject.transform;

				if (!(otherPixelCollider is MultiBodyPixelCollider))
				{
					Debug.Assert(otherPixelCollider.colliderPoints.Length == 4);

					Vector2 othertopWorld = otherPixelCollider.topWorld;
					Vector2 otherbottomWorld = otherPixelCollider.bottomWorld;
					Vector2 otherleftWorld = otherPixelCollider.leftWorld;
					Vector2 otherrightWorld = otherPixelCollider.rightWorld;

					Debug.DrawLine(othertopWorld, otherleftWorld, Color.white, 1.0f);
					Debug.DrawLine(otherleftWorld, otherbottomWorld, Color.white, 1.0f);
					Debug.DrawLine(otherbottomWorld, otherrightWorld, Color.white, 1.0f);
					Debug.DrawLine(otherrightWorld, othertopWorld, Color.white, 1.0f);

					if (DistanceBetween4points(leftWorld, topWorld, otherbottomWorld, otherrightWorld) < 1.4 &&
						DistanceBetween4points(leftWorld, topWorld, otherbottomWorld, otherrightWorld) > -2.0 &&
						leftWorld.x < (otherrightWorld.x) && topWorld.x > (otherbottomWorld.x) &&
						leftWorld.y < (otherrightWorld.y) && topWorld.y > (otherbottomWorld.y))
						restrictNW = true;

					if (DistanceBetween4points(topWorld, rightWorld, otherleftWorld, otherbottomWorld) < 0.4 &&
						DistanceBetween4points(topWorld, rightWorld, otherleftWorld, otherbottomWorld) > -2.0 &&
						topWorld.x < (otherbottomWorld.x) && rightWorld.x > (otherleftWorld.x) &&
						topWorld.y > (otherbottomWorld.y) && rightWorld.y < (otherleftWorld.y))
						restrictNE = true;

					if (DistanceBetween4points(leftWorld, bottomWorld, othertopWorld, otherrightWorld) > -0.4 &&
						DistanceBetween4points(leftWorld, bottomWorld, othertopWorld, otherrightWorld) < 2.0 &&
						leftWorld.x < (otherrightWorld.x) && bottomWorld.x > (othertopWorld.x) &&
						leftWorld.y > (otherrightWorld.y) && bottomWorld.y < (othertopWorld.y))
						restrictSW = true;

					if (DistanceBetween4points(bottomWorld, rightWorld, otherleftWorld, othertopWorld) > -0.4 &&
						DistanceBetween4points(bottomWorld, rightWorld, otherleftWorld, othertopWorld) < 2.0 &&
						bottomWorld.x < (othertopWorld.x) && rightWorld.x > (otherleftWorld.x) &&
						bottomWorld.y < (othertopWorld.y) && rightWorld.y > (otherleftWorld.y))
						restrictSE = true;
				}
				else
				{
					MultiBodyPixelCollider multi = otherPixelCollider as MultiBodyPixelCollider;
					foreach (CollisionBody cbody in multi.collisionBodies)
					{

						Vector2 othertopWorld = cbody.top + (Vector2)otherTransform.position;
						Vector2 otherbottomWorld = cbody.bottom + (Vector2)otherTransform.position;
						Vector2 otherleftWorld = cbody.left + (Vector2)otherTransform.position;
						Vector2 otherrightWorld = cbody.right + (Vector2)otherTransform.position;

						Debug.DrawLine(othertopWorld, otherleftWorld);
						Debug.DrawLine(otherleftWorld, otherbottomWorld);
						Debug.DrawLine(otherbottomWorld, otherrightWorld);
						Debug.DrawLine(otherrightWorld, othertopWorld);

						if (DistanceBetween4points(leftWorld, topWorld, otherbottomWorld, otherrightWorld) < 0.4 &&
							DistanceBetween4points(leftWorld, topWorld, otherbottomWorld, otherrightWorld) > -2.0 &&
							leftWorld.x < (otherrightWorld.x) && topWorld.x > (otherbottomWorld.x) &&
							leftWorld.y < (otherrightWorld.y) && topWorld.y > (otherbottomWorld.y))
							restrictNW = true;

						if (DistanceBetween4points(topWorld, rightWorld, otherleftWorld, otherbottomWorld) < 0.4 &&
							DistanceBetween4points(topWorld, rightWorld, otherleftWorld, otherbottomWorld) > -2.0 &&
							topWorld.x < (otherbottomWorld.x) && rightWorld.x > (otherleftWorld.x) &&
							topWorld.y > (otherbottomWorld.y) && rightWorld.y < (otherleftWorld.y))
							restrictNE = true;

						if (DistanceBetween4points(leftWorld, bottomWorld, othertopWorld, otherrightWorld) > -0.4 &&
							DistanceBetween4points(leftWorld, bottomWorld, othertopWorld, otherrightWorld) < 2.0 &&
							leftWorld.x < (otherrightWorld.x) && bottomWorld.x > (othertopWorld.x) &&
							leftWorld.y > (otherrightWorld.y) && bottomWorld.y < (othertopWorld.y))
							restrictSW = true;

						if (DistanceBetween4points(bottomWorld, rightWorld, otherleftWorld, othertopWorld) > -0.4 &&
							DistanceBetween4points(bottomWorld, rightWorld, otherleftWorld, othertopWorld) < 2.0 &&
							bottomWorld.x < (othertopWorld.x) && rightWorld.x > (otherleftWorld.x) &&
							bottomWorld.y < (othertopWorld.y) && rightWorld.y > (otherleftWorld.y))
							restrictSE = true;
					}
				}
			}

			// Collided with floor
			PixelRoom floor = transform.parent.parent.GetComponent<PixelRoom>();
			Debug.Assert(floor != null);
			Debug.Assert(floor.colliderPoints.Length == 4);

			Vector2 floortopWorld = floor.top + (Vector2)floor.transform.position;
			Vector2 floorbottomWorld = floor.bottom + (Vector2)floor.transform.position;
			Vector2 floorleftWorld = floor.left + (Vector2)floor.transform.position;
			Vector2 floorrightWorld = floor.right + (Vector2)floor.transform.position;

			if (DistanceBetween4points(leftWorld, topWorld, floorleftWorld, floortopWorld) < 0.4)
				restrictNW = true;

			if (DistanceBetween4points(topWorld, rightWorld, floortopWorld, floorrightWorld) < 0.4)
				restrictNE = true;

			if (DistanceBetween4points(leftWorld, bottomWorld, floorleftWorld, floorbottomWorld) > -0.4)
				restrictSW = true;

			if (DistanceBetween4points(bottomWorld, rightWorld, floorbottomWorld, floorrightWorld) > -0.4)
				restrictSE = true;

			// Send off movement restriction
			MovementRestriction movementRestriction = new MovementRestriction();

			movementRestriction.restrictNE = restrictNE;
			movementRestriction.restrictNW = restrictNW;
			movementRestriction.restrictSE = restrictSE;
			movementRestriction.restrictSW = restrictSW;

			return movementRestriction;
		}

		public bool CheckForWithinCollider(Vector2 position, float distance = 0.4f)
		{
			if (this is MultiBodyPixelCollider && (this as MultiBodyPixelCollider).collisionBodies.Count() != 0)
			{
				MultiBodyPixelCollider multibody = (this as MultiBodyPixelCollider);

				bool inside = false;
				foreach (CollisionBody body in multibody.collisionBodies)
				{
					if (DistanceBetween4points(leftWorld, topWorld, position, position) > distance)
						continue;

					if (DistanceBetween4points(topWorld, rightWorld, position, position) > distance)
						continue;

					if (DistanceBetween4points(leftWorld, bottomWorld, position, position) < -distance)
						continue;

					if (DistanceBetween4points(bottomWorld, rightWorld, position, position) < -distance)
						continue;

					// We are inside one of the blocks
					inside = true;
					break;
				}
				if (inside)
					return true;
				else
					return false;
			}
			else
			{
				if (this.colliderPoints.Length != 4)
				{
					Debug.Log(this.transform.parent.name);
				}
				Debug.Assert(this.colliderPoints.Length == 4);

				if (DistanceBetween4points(leftWorld, topWorld, position, position) > distance)
					return false;

				if (DistanceBetween4points(topWorld, rightWorld, position, position) > distance)
					return false;

				if (DistanceBetween4points(leftWorld, bottomWorld, position, position) < -distance)
					return false;

				if (DistanceBetween4points(bottomWorld, rightWorld, position, position) < -distance)
					return false;
			}

			return true;
		}

		public static float DistanceBetween4points(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
		{
			float m = (a2.y - a1.y) / (a2.x - a1.x); // Slope of parallel lines
			float i1 = a1.y - a1.x * m; // Intercept 1
			float i2 = b1.y - b1.x * m; // Intercept 2
			float dist = (i2 - i1) / Mathf.Sqrt(m * m + 1);
			return dist;
		}

		public static float DistanceBetween4pointsOrthographic(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
		{
			float m = (a2.y - a1.y) / (a2.x - a1.x); // Slope of parallel lines
			float i1 = a1.y - a1.x * m; // Intercept 1
			float i2 = b1.y - b1.x * m; // Intercept 2
			float dist = (i2 - i1) / 2 / Mathf.Abs(m / Mathf.Sqrt(m * m + 1));
			return dist;
		}

		public static float DistanceBetween4pointsAbs(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
		{
			return Mathf.Abs(DistanceBetween4points(a1, a2, b1, b2));
		}

		public static float DistanceBetween2pointsOrthographic(Vector2 a, Vector2 b, Direction direction)
		{
			float m = 0.0f;
			Debug.Assert(direction != Direction.All);
			if (direction == Direction.NE)
				m = 0.5f;
			else if (direction == Direction.NW)
				m = -0.5f;
			else if (direction == Direction.SE)
				m = -0.5f;
			else if (direction == Direction.SW)
				m = 0.5f;

			float i1 = a.y - a.x * m; // Intercept 1
			float i2 = b.y - b.x * m; // Intercept 2
			float dist = (i2 - i1) / 2 / Mathf.Abs(m / Mathf.Sqrt(m * m + 1));
			return dist;
		}

		public static float DistanceBetween2pointsOrthographicAbs(Vector2 a, Vector2 b, Direction direction)
		{
			return Mathf.Abs(DistanceBetween2pointsOrthographicAbs(a, b, direction));
		}

		// Returns 1 if in front of the other, returns 1 if object is in front of other
		public int CompareTo(PixelCollider other)
		{
			int comparison = 0;

			if (other == this)
				return 0;

			if (other is MultiBodyPixelCollider && this is MultiBodyPixelCollider)
			{
				MultiBodyPixelCollider a = this as MultiBodyPixelCollider;
				MultiBodyPixelCollider b = other as MultiBodyPixelCollider;

				Debug.Assert(a.collisionBodies.Count() > 0);
				Debug.Assert(b.collisionBodies.Count() > 0);

				// Find the front most box of all the boxes

				// TODO: Top sort for more complicated buildings
				for (int i = 0; i < a.collisionBodies.Count(); ++i)
				{
					for (int j = 0; j < b.collisionBodies.Count(); ++j)
					{
						int comp = CompareTwoCollisionBoxes(a.collisionBodies[i], (Vector2)a.transform.position, b.collisionBodies[j], (Vector2)b.transform.position);
						if (comp == 1)
							comparison = 1;
						if (comp == -1)
							comparison = -1;
					}
				}
			}
			else if (other is MultiBodyPixelCollider || this is MultiBodyPixelCollider)
			{
				MultiBodyPixelCollider multi;
				PixelCollider single;
				if (other is MultiBodyPixelCollider)
				{
					multi = other as MultiBodyPixelCollider;
					single = this;
				}
				else
				{
					multi = this as MultiBodyPixelCollider;
					single = other;
				}

				CollisionBody singleBody;
				singleBody.top = single.top;
				singleBody.bottom = single.bottom;
				singleBody.left = single.left;
				singleBody.right = single.right;

				// If any of the multi are in front of the single, multi wins
				int multiInFront = 0;
				for (int i = 0; i < multi.collisionBodies.Count(); ++i)
				{
					int comp = CompareTwoCollisionBoxes(multi.collisionBodies[i], (Vector2)multi.transform.position, singleBody, single.transform.position);
					if (comp == 1) multiInFront = 1;
					if (comp == -1) multiInFront = -1;
				}

				if (single != this)
				{
					comparison = multiInFront;
				}
				else
				{
					if (multiInFront == 1)
						comparison = -1;
					if (multiInFront == -1)
						comparison = 1;
				}
			}
			else
			{
				Debug.Assert(this.colliderPoints.Length == 4);
				Debug.Assert(other.colliderPoints.Length == 4);

				CollisionBody a;
				a.top = this.top;
				a.bottom = this.bottom;
				a.left = this.left;
				a.right = this.right;

				CollisionBody b;
				b.top = other.top;
				b.bottom = other.bottom;
				b.left = other.left;
				b.right = other.right;

				return CompareTwoCollisionBoxes(a, (Vector2)this.transform.position, b, (Vector2)other.transform.position);
				//Debug.Log("Comparison: " + transform.parent.name + " - " + other.transform.parent.name + ": " + comparison);
			}
			return comparison;
		}

		int CompareTwoCollisionBoxes(CollisionBody a, Vector2 aPosition, CollisionBody b, Vector2 bPosition)
		{
			int comparison = 0;

			Vector2 atopWorld = a.top + aPosition;
			Vector2 abottomWorld = a.bottom + aPosition;
			Vector2 aleftWorld = a.left + aPosition;
			Vector2 arightWorld = a.right + aPosition;

			Vector2 btopWorld = b.top + bPosition;
			Vector2 bbottomWorld = b.bottom + bPosition;
			Vector2 bleftWorld = b.left + bPosition;
			Vector2 brightWorld = b.right + bPosition;

			if (DistanceBetween4points(aleftWorld, atopWorld, bbottomWorld, brightWorld) >= -2.5)
				if (aleftWorld.x < brightWorld.x && aleftWorld.y < brightWorld.y)
					comparison = 1;

			if (DistanceBetween4points(atopWorld, arightWorld, bleftWorld, bbottomWorld) >= -2.5)
				if (arightWorld.x > bleftWorld.x && arightWorld.y < bleftWorld.y)
					comparison = 1;

			if (DistanceBetween4points(bleftWorld, btopWorld, abottomWorld, arightWorld) >= -2.5)
				if (bleftWorld.x < arightWorld.x && bleftWorld.y < arightWorld.y)
					comparison = -1;

			if (DistanceBetween4points(btopWorld, brightWorld, aleftWorld, abottomWorld) >= -2.5)
				if (brightWorld.x > aleftWorld.x && brightWorld.y < aleftWorld.y)
					comparison = -1;

			return comparison;
		}

		public bool ParentIsContainer()
		{
			for (int i = 0; i < transform.parent.parent.childCount; ++i)
			{
				Transform t = transform.parent.parent.GetChild(i);
				if (t.GetComponent<PixelCollider>() == true && !t.GetComponent<PixelCollider>().inspectChildObjects)
					return true;
			}
			return false;
		}

		public List<PixelCollider> GetChildColliders()
		{
			List<PixelCollider> pixelColliders = new List<PixelCollider>();

			for (int i = 0; i < transform.parent.childCount; ++i)
			{
				Transform t = transform.parent.GetChild(i);
				for (int j = 0; j < t.childCount; ++j)
				{
					Transform t2 = t.GetChild(j);
					PixelCollider pc = t2.GetComponent<PixelCollider>();
					if (pc != null)
					{
						pixelColliders.Add(pc);
					}
				}
			}

			return pixelColliders;
		}

		public List<SpriteRenderer> GetChildSpriteRenderers()
		{
			List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

			for (int i = 0; i < transform.parent.childCount; ++i)
			{
				Transform t = transform.parent.GetChild(i);
				for (int j = 0; j < t.childCount; ++j)
				{
					Transform t2 = t.GetChild(j);
					SpriteRenderer pc = t2.GetComponent<SpriteRenderer>();
					if (pc != null)
					{
						spriteRenderers.Add(pc);
					}
				}
			}

			return spriteRenderers;
		}

		bool OtherPixelColliderSameParent(PixelCollider pc)
		{
			return pc.GetPixelRoom() == this.GetPixelRoom();
		}

		public PixelRoom GetPixelRoom()
		{
			PixelRoom pixelRoom = transform.parent.parent.GetComponent<PixelRoom>();
			if (pixelRoom == null)
				pixelRoom = transform.parent.GetComponent<PixelRoom>();
			if (pixelRoom == null)
			{
				Debug.Assert(transform.parent.parent.parent != null);
				pixelRoom = transform.parent.parent.parent.GetComponent<PixelRoom>();
			}
			return pixelRoom;
		}

		public void HighlightObject()
		{
			SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
			if (spriteRenderer != null)
			{
				originalColor = spriteRenderer.color;
				Color color = new Color(255f, 255f, 255f, 0.2f);
				spriteRenderer.color = color;
			}
			SpriteRenderer objRenderer = this.transform.parent.GetComponent<SpriteRenderer>();
			if (objRenderer != null)
			{
				originalObjColor = objRenderer.color;
				Color color = new Color(255f, 255f, 0, 1.0f);
				objRenderer.color = color;
			}
		}

		public void UnHighlightObject()
		{
			SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
			if (spriteRenderer != null)
				spriteRenderer.color = originalColor;

			SpriteRenderer objRenderer = this.transform.parent.GetComponent<SpriteRenderer>();
			if (objRenderer != null)
			{
				objRenderer.color = originalObjColor;
			}
		}

		public WayPoint FindWayPointInDirection(Direction direction)
		{
			PixelRoom pixelRoom = GetPixelRoom();
			HashSet<WayPoint> navigationMesh = pixelRoom.navigationMesh;
			Debug.Assert(navigationMesh.Count != 0);
			if (direction == Direction.NE)
			{
				WayPoint closestNE = navigationMesh.Aggregate((arg1, arg2) => Vector2.Distance(arg1.position, topLeftWorld) < Vector2.Distance(arg2.position, topLeftWorld) ? arg1 : arg2);
				return closestNE;
			}
			else if (direction == Direction.NW)
			{
				WayPoint closestNW = navigationMesh.Aggregate((arg1, arg2) => Vector2.Distance(arg1.position, topRightWorld) < Vector2.Distance(arg2.position, topRightWorld) ? arg1 : arg2);
				return closestNW;
			}
			else if (direction == Direction.SE)
			{
				WayPoint closestNW = navigationMesh.Aggregate((arg1, arg2) => Vector2.Distance(arg1.position, bottomLeftWorld) < Vector2.Distance(arg2.position, bottomLeftWorld) ? arg1 : arg2);
				return closestNW;
			}
			else if (direction == Direction.SW)
			{
				WayPoint closestNW = navigationMesh.Aggregate((arg1, arg2) => Vector2.Distance(arg1.position, bottomRightWorld) < Vector2.Distance(arg2.position, bottomRightWorld) ? arg1 : arg2);
				return closestNW;
			}
			else
			{
				return null;
			}
		}

		public KeyValuePair<PixelPose, float> FindBestWayPoint()
		{
			WayPoint NE = FindWayPointInDirection(Direction.NE);
			WayPoint NW = FindWayPointInDirection(Direction.NW);
			WayPoint SE = FindWayPointInDirection(Direction.SE);
			WayPoint SW = FindWayPointInDirection(Direction.SW);

			Debug.DrawLine(topRightWorld, NE.position, Color.white, 3.0f);
			Debug.DrawLine(topLeftWorld, NW.position, Color.white, 3.0f);
			Debug.DrawLine(bottomRightWorld, SE.position, Color.white, 3.0f);
			Debug.DrawLine(bottomLeftWorld, SW.position, Color.white, 3.0f);

			float deltaNE = Vector2.Distance(NE.position, topLeft);
			float deltaNW = Vector2.Distance(NW.position, topRight);
			float deltaSE = Vector2.Distance(SE.position, bottomLeft);
			float deltaSW = Vector2.Distance(SW.position, bottomRight);

			Dictionary<PixelPose, float> dirDic = new Dictionary<PixelPose, float>();
			PixelPose NEpose = new PixelPose(GetPixelRoom(), Direction.SW, NE.position);
			PixelPose NWpose = new PixelPose(GetPixelRoom(), Direction.SE, NW.position);
			PixelPose SEpose = new PixelPose(GetPixelRoom(), Direction.NW, SE.position);
			PixelPose SWpose = new PixelPose(GetPixelRoom(), Direction.NE, SW.position);
			dirDic.Add(NEpose, deltaNE);
			dirDic.Add(NWpose, deltaNW);
			dirDic.Add(SEpose, deltaSE);
			dirDic.Add(SWpose, deltaSW);

			return dirDic.Aggregate((arg1, arg2) => arg1.Value < arg2.Value ? arg1 : arg2);
		}

		public PixelPose FindBestWayPointPosition(Vector2 starting)
		{
			WayPoint NE = FindWayPointInDirection(Direction.NE);
			WayPoint NW = FindWayPointInDirection(Direction.NW);
			WayPoint SE = FindWayPointInDirection(Direction.SE);
			WayPoint SW = FindWayPointInDirection(Direction.SW);

			List<PixelPose> dirList = new List<PixelPose>();
			PixelPose NEpose = new PixelPose(GetPixelRoom(), Direction.SW, NE.position);
			PixelPose NWpose = new PixelPose(GetPixelRoom(), Direction.SE, NW.position);
			PixelPose SEpose = new PixelPose(GetPixelRoom(), Direction.NW, SE.position);
			PixelPose SWpose = new PixelPose(GetPixelRoom(), Direction.NE, SW.position);
			dirList.Add(NEpose);
			dirList.Add(NWpose);
			dirList.Add(SEpose);
			dirList.Add(SWpose);

			return dirList.Aggregate((arg1, arg2) => Vector2.Distance(arg1.position, starting) < Vector2.Distance(arg2.position, starting) ? arg1 : arg2);
		}

		public void OnDrawGizmos()
        {
            Gizmos.DrawSphere(topRightWorld, 0.3f);
            Gizmos.DrawSphere(topLeftWorld, 0.3f);
			Gizmos.DrawSphere(bottomRightWorld, 0.3f);
			Gizmos.DrawSphere(bottomLeftWorld, 0.3f);
			Gizmos.DrawSphere(centerWorld, 0.3f);
			Gizmos.DrawSphere(topWorld, 0.3f);
			Gizmos.DrawSphere(bottomWorld, 0.3f);
			Gizmos.DrawSphere(leftWorld, 0.3f);
			Gizmos.DrawSphere(rightWorld, 0.3f);
        }
	}
 
    public struct PixelCollision {
        public Direction direction;
        public PixelCollider pixelCollider;
    }
}