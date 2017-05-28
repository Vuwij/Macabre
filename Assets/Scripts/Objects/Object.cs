using UnityEngine;
using System.Collections.Generic;

namespace Objects
{
	public abstract class Object : MonoBehaviour
	{
		protected SpriteRenderer spriteRenderer
		{
			get { return GetComponentInChildren<SpriteRenderer>(); }
		}
		protected Rigidbody2D rigidbody2D {
			get { return GetComponentInChildren<Rigidbody2D>(); }
		}

		Object objectInFront;
		CollisionBox collisionbox;
		public Texture2D footprint;

		protected virtual void Start()
        {
			collisionbox = new CollisionBox(gameObject);
        }

		#region Layer Sorting

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
		protected void EncounterObjectSortLayer(Object obj)
		{
			if (ObjectIsInFront(obj, this))
			{
				if (objectInFront == null) objectInFront = obj;
				ReorganizeSortingLayer(obj);
				IncrementObjectInFrontIfSortingLayerIsSame();

				if (objectInFront == obj) return;

				if (!ObjectIsInFront(obj, objectInFront)) {
					// Chain the next object
					Object objInFrontCopy = objectInFront;
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
		private void ReorganizeSortingLayer(Object obj)
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
		private static bool ObjectIsInFront(Object obj1, Object obj2)
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
			if (collision.gameObject.GetComponent<Object>() != null)
			{
				Object objController = collision.gameObject.GetComponent<Object>();
				EncounterObjectSortLayer(objController);
			}
		}

		#endregion

		protected T FindNearestObject<T>()
			where T : Object
		{
			RaycastHit2D[] castStar = Physics2D.CircleCastAll(transform.position, GameSettings.inspectRadius, Vector2.zero);

			foreach (RaycastHit2D raycastHit in castStar)
			{
				T hit = raycastHit.collider.GetComponentInChildren<T>();
				if (hit != null) return hit;
			}

			return null;
		}
    }
}
