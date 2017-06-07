﻿using UnityEngine;
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
		protected int sortingOffset = 0;

		protected virtual void Start()
        {
			UpdateSortingLayer();
        }

		protected virtual void UpdateSortingLayer() {
			float yPos = (int) ((1000 - transform.position.y) * 10) + sortingOffset;
			spriteRenderer.sortingOrder = (int) yPos;
		}

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
