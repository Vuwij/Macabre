using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Objects.Movable.Characters;
using Objects.Movable.Characters.Individuals;

namespace Objects
{
	public enum Layer
	{
		Front,
		World,
		Back
	}

	[System.Serializable]
	public struct OtherVisibleRoom
	{
		public PixelRoom room;
		public Layer layer;
	}
    
	public class WayPoint
    {
        public Vector2 position;
        public float distance = float.MaxValue;
        public WayPoint previous = null;
		public List<WayPoint> neighbours = new List<WayPoint>();

        public WayPoint() { }
        public WayPoint(Vector2 position)
        {
            this.position = position;
        }

        public static float Distance(WayPoint a, WayPoint b)
        {
            return Vector2.Distance(a.position, b.position);
        }
    }
    
	public class PixelRoom : MonoBehaviour
	{
        new PolygonCollider2D collider2D;

		[HideInInspector]
		public Vector2 top, bottom, left, right;
		[HideInInspector]
		public Vector2[] colliderPoints;

		public OtherVisibleRoom[] otherVisibleRooms;
		public int RoomWalkingSpeed = 10;
		public int stepSize = 3; // How much steps for the navigation mesh

		HashSet<WayPoint> navigationMesh = new HashSet<WayPoint>();

		public List<PixelDoor> pixelDoors {
			get {
				List <PixelDoor> doors = new List<PixelDoor>();

				for (int i = 0; i < transform.childCount; ++i) {
					Transform t = transform.GetChild(i);
					PixelDoor door = t.GetComponent<PixelDoor>();
					if(door != null) {
						doors.Add(door);
					}
				}

				foreach(PixelExterior exterior in pixelExteriors) {
					doors.AddRange(exterior.pixelDoors);
				}

				return doors;
			}
		}

		public List<PixelExterior> pixelExteriors {
            get {
				List<PixelExterior> exteriors = new List<PixelExterior>();

                for (int i = 0; i < transform.childCount; ++i)  {
                    Transform t = transform.GetChild(i);
					PixelExterior exterior = t.GetComponent<PixelExterior>();
                    if (exterior != null)
						exteriors.Add(exterior);
                }

                return exteriors;
            }
        }

		void Awake()
		{
			collider2D = GetComponent<PolygonCollider2D>();

			Debug.Assert(collider2D != null);
			Debug.Assert(collider2D.points.Length == 4);
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

		public void OnEnable()
		{
			SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
			foreach (SpriteRenderer sr in spriteRenderers)
			{
				SetSortingLayer(Layer.World, sr);
				if (sr.sortingLayerName.Contains("Foreground"))
					sr.gameObject.SetActive(true);
			}

			foreach (OtherVisibleRoom room in GetAllConnectedRooms())
			{
				room.room.gameObject.SetActive(true);
				SpriteRenderer[] srs = room.room.GetComponentsInChildren<SpriteRenderer>(true);
				foreach (SpriteRenderer sr in srs)
				{
					SetSortingLayer(room.layer, sr);
					if (sr.sortingLayerName.Contains("Foreground"))
						sr.gameObject.SetActive(false);
				}
			}
		}

		public void OnDisable()
		{
			OtherVisibleRoom[] allOtherVisibleRooms = GetAllConnectedRooms();
			foreach (OtherVisibleRoom room in allOtherVisibleRooms)
			{
				if(room.room != null)
				    room.room.gameObject.SetActive(false);
			}
		}

		OtherVisibleRoom[] GetAllConnectedRooms()
		{
			List<OtherVisibleRoom> rooms = new List<OtherVisibleRoom>();

			for (int i = 0; i < transform.childCount; ++i)
			{
				Transform child = transform.GetChild(i);
				PixelExterior pixelExterior = child.GetComponent<PixelExterior>();
				if(pixelExterior != null) {
					rooms.AddRange(pixelExterior.otherVisibleRooms.ToList());
				}
		    }
			rooms.AddRange(otherVisibleRooms.ToList());

			return rooms.ToArray();
		}


		void SetSortingLayer(Layer layer, SpriteRenderer sr)
		{
			if (layer == Layer.World)
			{
				if (sr.sortingLayerName == "Front - World")
					sr.sortingLayerName = "World";
				else if (sr.sortingLayerName == "Back - World")
					sr.sortingLayerName = "World";
				else if (sr.sortingLayerName == "Front - Foreground")
					sr.sortingLayerName = "Foreground";
				else if (sr.sortingLayerName == "Back - Foreground")
					sr.sortingLayerName = "Foreground";
				else if (sr.sortingLayerName == "Front - Background")
					sr.sortingLayerName = "Background";
				else if (sr.sortingLayerName == "Back - Background")
					sr.sortingLayerName = "Background";
			}
			else if (layer == Layer.Back)
			{
				if (sr.sortingLayerName == "Front - World")
					sr.sortingLayerName = "Back - World";
				else if (sr.sortingLayerName == "World")
					sr.sortingLayerName = "Back - World";
				else if (sr.sortingLayerName == "Foreground")
					sr.sortingLayerName = "Back - Foreground";
				else if (sr.sortingLayerName == "Back - Foreground")
					sr.sortingLayerName = "Back - Foreground";
				else if (sr.sortingLayerName == "Front - Background")
					sr.sortingLayerName = "Back - Background";
				else if (sr.sortingLayerName == "Background")
					sr.sortingLayerName = "Back - Background";
			}
			else if (layer == Layer.Front)
			{
				if (sr.sortingLayerName == "World")
					sr.sortingLayerName = "Front - World";
				else if (sr.sortingLayerName == "Back - World")
					sr.sortingLayerName = "Front - World";
				else if (sr.sortingLayerName == "Foreground")
					sr.sortingLayerName = "Front - Foreground";
				else if (sr.sortingLayerName == "Back - Foreground")
					sr.sortingLayerName = "Front - Foreground";
				else if (sr.sortingLayerName == "Background")
					sr.sortingLayerName = "Front - Background";
				else if (sr.sortingLayerName == "Back - Background")
					sr.sortingLayerName = "Front - Background";
			}
		}

		public HashSet<WayPoint> GetNavigationalMesh(Vector2 startPosition, int stepSize) {

			if (navigationMesh.Count != 0) {
				foreach (WayPoint w in navigationMesh) {
					w.distance = float.MaxValue;
					w.previous = null;
				}
				return new HashSet<WayPoint>(navigationMesh);
			}
			         
			Vector2 topWorld = top + (Vector2) transform.position;
			Vector2 bottomWorld = bottom + (Vector2)transform.position;
			Vector2 leftWorld = left + (Vector2)transform.position;
			Vector2 rightWorld = right + (Vector2) transform.position;

            // Should all be positive
			float topLeft = -PixelCollider.DistanceBetween4pointsOrthographic(leftWorld, topWorld, startPosition, startPosition);
			float topRight = -PixelCollider.DistanceBetween4pointsOrthographic(topWorld, rightWorld, startPosition, startPosition);
			float bottomLeft = PixelCollider.DistanceBetween4pointsOrthographic(leftWorld, bottomWorld, startPosition, startPosition);
			float bottomRight = PixelCollider.DistanceBetween4pointsOrthographic(bottomWorld, rightWorld, startPosition, startPosition);
            
			Debug.DrawLine(topWorld, leftWorld, Color.blue, 10.0f);
			Debug.DrawLine(leftWorld, bottomWorld, Color.blue, 10.0f);
			Debug.DrawLine(bottomWorld, rightWorld, Color.blue, 10.0f);
			Debug.DrawLine(rightWorld, topWorld, Color.blue, 10.0f);

			Vector2 topLeftPoint = startPosition + new Vector2(-topLeft / 2.23606f * 2, topLeft / 2.23606f);
			Vector2 topRightPoint = startPosition + new Vector2(topRight / 2.23606f * 2, topRight / 2.23606f);
			Vector2 bottomLeftPoint = startPosition + new Vector2(-bottomLeft / 2.23606f * 2, -bottomLeft / 2.23606f);
			Vector2 bottomRightPoint = startPosition + new Vector2(bottomRight / 2.23606f * 2, -bottomRight / 2.23606f);
            
			Debug.Assert(topLeft > 0);
			Debug.Assert(topRight > 0);
			Debug.Assert(bottomLeft > 0);
			Debug.Assert(bottomRight > 0);

			float stepSizeLength = (new Vector2(stepSize * 2, stepSize)).magnitude;

			int topLeftSteps = Mathf.FloorToInt (topLeft / stepSizeLength);
			int topRightSteps = Mathf.FloorToInt (topRight / stepSizeLength);
			int bottomLeftSteps = Mathf.FloorToInt (bottomLeft / stepSizeLength);
			int bottomRightSteps = Mathf.FloorToInt (bottomRight / stepSizeLength);

			WayPoint[,] wayPointArray = new WayPoint[bottomLeftSteps + topRightSteps + 1, bottomRightSteps + topLeftSteps + 1];

			for (int i = -bottomLeftSteps; i <= topRightSteps; ++i) {
				for (int j = -bottomRightSteps; j <= topLeftSteps; ++j) {
					Vector2 point = startPosition + new Vector2(stepSize * 2, stepSize) * i + new Vector2(-stepSize * 2, stepSize) * j;
					WayPoint wayPoint = new WayPoint(point);
                    wayPointArray[i + bottomLeftSteps, j + bottomRightSteps] = wayPoint;
				}
			}

			for (int i = -bottomLeftSteps; i <= topRightSteps; ++i) {
				for (int j = -bottomRightSteps; j <= topLeftSteps; ++j) {

					if (i != -bottomLeftSteps) 
						wayPointArray[i - 1 + bottomLeftSteps, j + bottomRightSteps].neighbours.Add(wayPointArray[i + bottomLeftSteps, j + bottomRightSteps]);

					if (i != topRightSteps)
						wayPointArray[i + 1 + bottomLeftSteps, j + bottomRightSteps].neighbours.Add(wayPointArray[i + bottomLeftSteps, j + bottomRightSteps]);

					if (j != -bottomRightSteps)
						wayPointArray[i + bottomLeftSteps, j - 1 + bottomRightSteps].neighbours.Add(wayPointArray[i + bottomLeftSteps, j + bottomRightSteps]);

					if (j != topLeftSteps)
						wayPointArray[i + bottomLeftSteps, j + 1 + bottomRightSteps].neighbours.Add(wayPointArray[i + bottomLeftSteps, j + bottomRightSteps]);
                }
            }

			// Remove all waypoints with pixel colliders
			for (int c = 0; c < transform.childCount; ++c) {
				Transform t = transform.GetChild(c);
				if (t.GetComponent<Player>() != null) continue;

				// Get the child pixel collider
				PixelCollider[] pixelColliders = t.GetComponentsInChildren<PixelCollider>();
                
				foreach (PixelCollider pixelCollider in pixelColliders)
				{
					if (pixelCollider != null && pixelCollider.isActiveAndEnabled)
					{
						List<WayPoint> badWayPoints = new List<WayPoint>();
						for (int i = -bottomLeftSteps; i <= topRightSteps; ++i)
						{
							for (int j = -bottomRightSteps; j <= topLeftSteps; ++j)
							{
								if (wayPointArray[i + bottomLeftSteps, j + bottomRightSteps] == null) continue;

								if (pixelCollider.CheckForWithinCollider(wayPointArray[i + bottomLeftSteps, j + bottomRightSteps].position, 0.8f))
								{
									foreach (WayPoint n in wayPointArray[i + bottomLeftSteps, j + bottomRightSteps].neighbours)
									{
										n.neighbours.Remove(wayPointArray[i + bottomLeftSteps, j + bottomRightSteps]);
									}
									wayPointArray[i + bottomLeftSteps, j + bottomRightSteps] = null;
								}
							}
						}
					}
				}
			}
            
			for (int i = -bottomLeftSteps; i <= topRightSteps; ++i) {
                for (int j = -bottomRightSteps; j <= topLeftSteps; ++j) {
					if (wayPointArray[i + bottomLeftSteps, j + bottomRightSteps] != null)
						navigationMesh.Add(wayPointArray[i + bottomLeftSteps, j + bottomRightSteps]);
                }
            }
            
			foreach(WayPoint w in navigationMesh){
				foreach(WayPoint n in w.neighbours) {
					Debug.DrawLine(w.position, n.position, Color.green, 10.0f);
				}
			}

			//Debug.DrawLine(startPosition, topLeftPoint, Color.cyan, 10.0f);
			//Debug.DrawLine(startPosition, topRightPoint, Color.cyan, 10.0f);
			//Debug.DrawLine(startPosition, bottomLeftPoint, Color.cyan, 10.0f);
			//Debug.DrawLine(startPosition, bottomRightPoint, Color.cyan, 10.0f);
                        
			return new HashSet<WayPoint>(navigationMesh);
		}
	}
}