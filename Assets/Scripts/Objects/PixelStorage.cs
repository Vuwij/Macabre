using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
	public class PixelStorage : MonoBehaviour
	{
		public List<Vector2Int> displayLocations = new List<Vector2Int>();
		List<int> occupiedLocations = new List<int>();

		public void AddObject(GameObject obj)
		{
			obj.transform.parent = this.transform;

			for (int i = 0; i < displayLocations.Count; ++i)
			{
				if (!occupiedLocations.Contains(i))
				{
					occupiedLocations.Add(i);

					obj.transform.position = (Vector2)this.transform.position + displayLocations[i];
					obj.SetActive(true);
					SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
					SpriteRenderer parent = gameObject.GetComponent<SpriteRenderer>();

					Debug.Assert(spriteRenderer != null && parent != null);
					spriteRenderer.sortingOrder = parent.sortingOrder + 1;
					return;
				}
			}

			// Filled up, no more space
			obj.SetActive(false);
		}

		public void TakeObject(GameObject obj)
		{

		}

		public void OnDrawGizmos()
		{
			foreach (Vector2 location in displayLocations)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(location + (Vector2)transform.position, 1.0f);
			}
		}
	}
}