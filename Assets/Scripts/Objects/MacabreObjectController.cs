using UnityEngine;
using System.Collections.Generic;

namespace Objects
{
	public abstract class MacabreObjectController : MonoBehaviour
	{
        // This is the field that allows editing
        protected abstract MacabreObject model { get; }

        protected T GetNearestMacabreObject<T>()
            where T : MacabreObjectController
        {
            RaycastHit2D[] castStar = Physics2D.CircleCastAll(transform.position, GameSettings.inspectRadius, Vector2.zero);

            foreach (RaycastHit2D raycastHit in castStar)
            {
                T hit = raycastHit.collider.GetComponentInChildren<T>();
                if (hit != null) return hit;
            }

            Debug.Log("No objects within radius");
            return null;
        }

        protected virtual void Start()
        {
            CreateCollisionCircle();
            CreateProximityCircle();
            SetupBackEdgeCollider();
        }

		#region Collision

		// The SpriteRenderer of the object
		private SpriteRenderer spriteRenderer
		{
			get { return GetComponentInChildren<SpriteRenderer>(); }
		}

		// This property is visible in inspector (used to create the collision box)
		public Texture2D footprint;

		// Abstract creates of the collision and proximity
		public abstract void CreateCollisionCircle();
		public abstract void CreateProximityCircle();

		// The Collider and Proximity Boxfor the object
		protected abstract PolygonCollider2D collisionBox { get; }
		protected abstract PolygonCollider2D proximityBox { get; }

		private Vector2[] collisionVertices
		{
			get { return collisionBox.points; }
		}
		private Vector2[] proximityVertices
		{
			get { return proximityBox.points; }
		}

		#endregion

		#region Layer Sorting

		private MacabreObjectController objectInFront = null;

		private int orderInLayer
		{
			get { return spriteRenderer.sortingOrder; }
			set { spriteRenderer.sortingOrder = value; }
		}

		/// <summary>
		/// Encounters an object, does a chain algorithm to see which one is in the really front
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>Whether or not the object is in front</returns>
		protected void EncounterObjectSortLayer(MacabreObjectController obj)
		{
			if (ObjectIsInFront(obj, this))
			{
				if (objectInFront == null) objectInFront = obj;
				ReorganizeSortingLayer(obj);
				IncrementObjectInFrontIfSortingLayerIsSame();

				if (objectInFront == obj) return;

				if (!ObjectIsInFront(obj, objectInFront)) {
					// Chain the next object
					MacabreObjectController objInFrontCopy = objectInFront;
					objectInFront = obj;
					obj.EncounterObjectSortLayer(objInFrontCopy);
				}
				else
				{
					// Chain the next object
					if (!ObjectIsInFront(objectInFront, this)) objectInFront = null;
					else objectInFront.EncounterObjectSortLayer(obj);
				}
			}
		}

		// Organize the sorting layer
		private void ReorganizeSortingLayer(MacabreObjectController obj)
		{
			if (obj.orderInLayer > orderInLayer) { } // Do nothing
			if (obj.orderInLayer == orderInLayer) objectInFront.orderInLayer++;
			if (obj.orderInLayer < orderInLayer)
			{
				int a = orderInLayer;
				orderInLayer = obj.orderInLayer;
				obj.orderInLayer = a;
			}
		}

		// Increment sorting layer if there is an error
		private void IncrementObjectInFrontIfSortingLayerIsSame()
		{
			//if (objectInFront == null) return;
			//if (orderInLayer > objectInFront.orderInLayer) return;
			//objectInFront.orderInLayer++;
			//objectInFront.IncrementObjectInFrontIfSortingLayerIsSame();
		}

		/// <returns>True if Object1 is in front of Object2</returns>
		private static bool ObjectIsInFront(MacabreObjectController obj1, MacabreObjectController obj2)
		{
			float obj1Y = obj1.transform.position.y;
			float obj2Y = obj2.transform.position.y;
			return (obj1Y < obj2Y);
		}

		// When the character hits the backedge collider (works for both colliders
		protected virtual void OnTriggerEnter2D(Collider2D collision)
		{
			DetermineSortingLayer2D(collision);
		}

		protected virtual void OnTriggerExit2D(Collider2D collision)
		{
			DetermineSortingLayer2D(collision);
		}

		private void DetermineSortingLayer2D(Collider2D collision)
		{
			if (!(collision is PolygonCollider2D) || collision.isTrigger) return;
			//Debug.Log(name + " encountered " + collision.gameObject.name + " via " + collision.GetType());
			if (collision.gameObject.GetComponent<MacabreObjectController>() != null)
			{
				MacabreObjectController objController = collision.gameObject.GetComponent<MacabreObjectController>();
				EncounterObjectSortLayer(objController);
			}
		}

		#endregion

		#region Layer Sorting Layer

		protected virtual void SetupBackEdgeCollider()
		{
			if (GetComponentInChildren<EdgeCollider2D>() != null) return;
			CreateBackEdgeCollider();
		}

		// The Default BackEdgeCollider
		protected EdgeCollider2D CreateBackEdgeCollider()
		{
			List<Vector2> backEdgePointsOfObject = new List<Vector2>();

			// Detect Edge Points here
			int leftIndex = 0;
			int rightIndex = 0;

			// Determine the indexes of the left and the right most point
			float leftMostPoint = 0;
			float rightMostPoint = 0;

			GetMaximaPoints(collisionVertices, out leftIndex, out rightIndex, out leftMostPoint, out rightMostPoint);

			// Determine the clockwise direction TODO: Get the correct direction
			bool clockWise = collisionVertices[leftIndex].y < collisionVertices[(leftIndex + 1) % collisionVertices.Length].y;

			// Add backEdgePoints based on direction
			int index = leftIndex;
			while (index != rightIndex)
			{
				backEdgePointsOfObject.Add(collisionVertices[index]);
				index = (clockWise ? index + 1 : index - 1) % collisionVertices.Length;
			}

			// Add the left line and the right line
			AddLeftAndRightEdges(ref backEdgePointsOfObject);

			// Use the Edge Points to create a back edge
			EdgeCollider2D backOfObject = gameObject.AddComponent<EdgeCollider2D>();
			backOfObject.points = backEdgePointsOfObject.ToArray();
			backOfObject.isTrigger = true;
			return backOfObject;
		}

		private void GetMaximaPoints(Vector2[] points, out int leftIndex, out int rightIndex, out float leftMostPoint, out float rightMostPoint)
		{
			leftIndex = 0;
			rightIndex = 0;
			leftMostPoint = points[0].x;
			rightMostPoint = points[0].x;
			for (int i = 0; i < points.Length; i++)
			{
				if (points[i].x < leftMostPoint)
				{
					leftIndex = i;
					leftMostPoint = points[i].x;
				}
				if (points[i].x > rightMostPoint)
				{
					rightIndex = i;
					rightMostPoint = points[i].x;
				}
			}
		}

		private void AddLeftAndRightEdges(ref List<Vector2> backEdgePointsOfObject)
		{
			int leftIndex, rightIndex;
			float leftMostPoint, rightMostPoint;

			GetMaximaPoints(proximityVertices, out leftIndex, out rightIndex, out leftMostPoint, out rightMostPoint);
			backEdgePointsOfObject.Insert(0, proximityVertices[leftIndex]);
			backEdgePointsOfObject.Add(proximityVertices[rightIndex]);
		}

		#endregion
    }
}
