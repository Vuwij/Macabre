using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Objects.Movable;
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

		public Vector2 topWorld => top + (Vector2) transform.position;
		public Vector2 bottomWorld => bottom + (Vector2)transform.position;
		public Vector2 leftWorld => left + (Vector2)transform.position;
		public Vector2 rightWorld => right + (Vector2)transform.position;
		public PixelBox collisionbody => new PixelBox(top, left, right, bottom);
		public PixelBox collisionbodyWorld => new PixelBox(topWorld, leftWorld, rightWorld, bottomWorld);
  		public Vector2 topLeft => (top + left) / 2;
        public Vector2 topRight => (top + right) / 2;
        public Vector2 bottomLeft => (bottom + left) / 2;
        public Vector2 bottomRight => (bottom + right) / 2;
        public Vector2 center => (top + left + right + bottom) / 4;
              
        
		public OtherVisibleRoom[] otherVisibleRooms;
		public int RoomWalkingSpeed = 10;
		public int stepSize = 1; // How much steps for the navigation mesh

		public HashSet<WayPoint> navigationMesh = new HashSet<WayPoint>();
		[HideInInspector]
		public PixelCollider navigationMeshObject = null;

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

		public void Awake()
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
      
        
		public HashSet<WayPoint> GetNavigationalMesh(PixelCollider pixelCollider = default(PixelCollider), Vector2 startPosition = default(Vector2)) {
   			float navigationMargin = 0.0f;
			if (pixelCollider == default(PixelCollider)) {
                Character player = GameObject.Find("Player").GetComponent<Character>();
				pixelCollider = player.GetComponentInChildren<PixelCollider>();
			}

			navigationMargin = pixelCollider.navigationMargin;

			if (startPosition == default(Vector2))
                startPosition = pixelCollider.transform.position;

			Debug.Assert(startPosition != default(Vector2));
			GetNavigationalMesh(startPosition, 0, navigationMargin);
            Debug.Assert(navigationMesh.Count != 0);

			// Stamp all moving objects except this one
            HashSet<WayPoint> navMeshCopy = new HashSet<WayPoint>(navigationMesh);
            
			for (int c = 0; c < transform.childCount; ++c)
            {
                Transform t = transform.GetChild(c);

				// Remove all movable objects
				if (!t.GetComponent<MovableObject>()) continue;
                
				PixelCollider movingCollider = t.GetComponentInChildren<PixelCollider>();
				if (movingCollider == pixelCollider) continue;

				if(movingCollider != null) {
					StampPixelCollider(navMeshCopy, movingCollider, pixelCollider.navigationMargin);
				}
            }

			foreach (WayPoint w in navMeshCopy)
            {
                foreach (WayPoint n in w.neighbours)
                {
                    Debug.DrawLine(w.position, n.position, Color.green, 10.0f);
                }
            }

			return navMeshCopy;
		}

		public HashSet<WayPoint> GetNavigationalMesh(Vector2 startPosition, int stepSize = 0, float margin = 0.0f) {

			if (stepSize == 0)
				stepSize = this.stepSize;

			if (navigationMesh.Count != 0) {
				foreach (WayPoint w in navigationMesh) {
					w.distance = float.MaxValue;
					w.previous = null;
				}
				return new HashSet<WayPoint>(navigationMesh);
			}

			if (System.Math.Abs(margin) < 0.01f) {
				Character player = GameObject.Find("Player").GetComponent<Character>();
				margin = player.GetComponentInChildren<PixelCollider>().navigationMargin;
			}

			if (top == Vector2.zero) {
				this.gameObject.SetActive(true);
				this.gameObject.SetActive(false);
			}

 			Debug.Assert(top != Vector2.zero);
			Debug.Assert(bottom != Vector2.zero);
			Debug.Assert(left != Vector2.zero);
			Debug.Assert(right != Vector2.zero);
			Vector2 topWorld = top + (Vector2) transform.position;
			Vector2 bottomWorld = bottom + (Vector2)transform.position;
			Vector2 leftWorld = left + (Vector2)transform.position;
			Vector2 rightWorld = right + (Vector2) transform.position;

            // Should all be positive
			float topLeftDist = -PixelCollider.DistanceBetween4pointsOrthographic(leftWorld, topWorld, startPosition, startPosition);
			float topRightDist = -PixelCollider.DistanceBetween4pointsOrthographic(topWorld, rightWorld, startPosition, startPosition);
			float bottomLeftDist = PixelCollider.DistanceBetween4pointsOrthographic(leftWorld, bottomWorld, startPosition, startPosition);
			float bottomRightDist = PixelCollider.DistanceBetween4pointsOrthographic(bottomWorld, rightWorld, startPosition, startPosition);
            
			Debug.DrawLine(topWorld, leftWorld, Color.blue, 10.0f);
			Debug.DrawLine(leftWorld, bottomWorld, Color.blue, 10.0f);
			Debug.DrawLine(bottomWorld, rightWorld, Color.blue, 10.0f);
			Debug.DrawLine(rightWorld, topWorld, Color.blue, 10.0f);

			Vector2 topLeftPoint = startPosition + new Vector2(-topLeftDist / 2.23606f * 2, topLeftDist / 2.23606f);
			Vector2 topRightPoint = startPosition + new Vector2(topRightDist / 2.23606f * 2, topRightDist / 2.23606f);
			Vector2 bottomLeftPoint = startPosition + new Vector2(-bottomLeftDist / 2.23606f * 2, -bottomLeftDist / 2.23606f);
			Vector2 bottomRightPoint = startPosition + new Vector2(bottomRightDist / 2.23606f * 2, -bottomRightDist / 2.23606f);
            
			Debug.Assert(topLeftDist >= 0);
			Debug.Assert(topRightDist >= 0);
			Debug.Assert(bottomLeftDist >= 0);
			Debug.Assert(bottomRightDist >= 0);

			float stepSizeLength = (new Vector2(stepSize * 2, stepSize)).magnitude;

			int topLeftSteps = Mathf.FloorToInt ((topLeftDist - margin) / stepSizeLength);
			int topRightSteps = Mathf.FloorToInt ((topRightDist - margin) / stepSizeLength);
			int bottomLeftSteps = Mathf.FloorToInt ((bottomLeftDist - margin) / stepSizeLength);
			int bottomRightSteps = Mathf.FloorToInt ((bottomRightDist - margin) / stepSizeLength);

			topLeftSteps = topLeftSteps >= 0 ? topLeftSteps : 0;
			topRightSteps = topRightSteps >= 0 ? topRightSteps : 0;
			bottomLeftSteps = bottomLeftSteps >= 0 ? bottomLeftSteps : 0;
			bottomRightSteps = bottomRightSteps >= 0 ? bottomRightSteps : 0;

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

				// Remove all movable objects
				if (t.GetComponent<MovableObject>())
					continue;

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
                                
								if (pixelCollider.CheckForWithinCollider(wayPointArray[i + bottomLeftSteps, j + bottomRightSteps].position, margin))
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
                        
			return new HashSet<WayPoint>(navigationMesh);
		}

		public void StampPixelCollider(HashSet<WayPoint> navMesh, PixelCollider pixelCollider, float dist = 8.0f) {
			Debug.Assert(navMesh.Count != 0);

			Debug.DrawLine(pixelCollider.topWorld, pixelCollider.leftWorld, Color.magenta, 3.0f);
			Debug.DrawLine(pixelCollider.leftWorld, pixelCollider.bottomWorld, Color.magenta, 3.0f);
			Debug.DrawLine(pixelCollider.bottomWorld, pixelCollider.rightWorld, Color.magenta, 3.0f);
			Debug.DrawLine(pixelCollider.rightWorld, pixelCollider.topWorld, Color.magenta, 3.0f);
            
			Vector2 topWorldExtra = pixelCollider.topWorld + new Vector2(0.0f, 0.5f * 0.5773502692f * dist);
			Vector2 bottomWorldExtra = pixelCollider.bottomWorld + new Vector2(0.0f, -0.5f * 0.5773502692f * dist);
			Vector2 leftWorldExtra = pixelCollider.leftWorld + new Vector2(-0.5773502692f * dist, 0.0f);
			Vector2 rightWorldExtra = pixelCollider.rightWorld + new Vector2(0.5773502692f * dist, 0.0f);

			Debug.DrawLine(topWorldExtra, leftWorldExtra, Color.yellow, 3.0f);
			Debug.DrawLine(leftWorldExtra, bottomWorldExtra, Color.yellow, 3.0f);
			Debug.DrawLine(bottomWorldExtra, rightWorldExtra, Color.yellow, 3.0f);
			Debug.DrawLine(rightWorldExtra, topWorldExtra, Color.yellow, 3.0f);

			if (pixelCollider != null && pixelCollider.isActiveAndEnabled)
			{
				List<WayPoint> toRemove = new List<WayPoint>();
				foreach (WayPoint w in navMesh)
				{
					bool collidedWayPoint = pixelCollider.CheckForWithinCollider(w.position, dist);
					if (collidedWayPoint)
						toRemove.Add(w);
				}
				foreach(WayPoint w in toRemove)
					navMesh.Remove(w);
			}
			Debug.Assert(navMesh.Count != 0);
		}
	}
}