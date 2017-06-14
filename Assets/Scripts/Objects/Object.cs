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
		new protected Rigidbody2D rigidbody2D {
			get { return GetComponentInChildren<Rigidbody2D>(); }
		}

		Object objectInFront;
		public Texture2D footprint;
		public string interactionText = "Press T to Interact";
		protected int sortingOffset = 0;

		protected virtual void Start()
        {
			UpdateSortingLayer();
        }

		public virtual void UpdateSortingLayer() {
			float yPos = (int) ((1000 - transform.position.y) * 10) + sortingOffset;
			spriteRenderer.sortingOrder = (int) yPos;
		}

		protected T FindNearestComponent<T>()
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

		protected Object FindNearestObject<T>()
		{
			RaycastHit2D[] castStar = Physics2D.CircleCastAll(transform.position, GameSettings.inspectRadius, Vector2.zero);

			foreach (RaycastHit2D raycastHit in castStar)
			{
				T hit = raycastHit.collider.GetComponentInChildren<T>();
				if (hit != null) {
					if(raycastHit.collider.gameObject != gameObject) {
						return raycastHit.collider.gameObject.GetComponent<Object>();
					}
				}
			}
			return null;
		}
    }
}
