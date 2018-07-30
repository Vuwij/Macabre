using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Objects
{
	public class PixelStorage : MonoBehaviour
	{
		public List<Vector2Int> displayLocations = new List<Vector2Int>();
		protected Dictionary<int, GameObject> occupiedLocations = new Dictionary<int, GameObject>();

		public void AddObject(GameObject obj)
		{
			obj.transform.parent = this.transform;

			for (int i = 0; i < displayLocations.Count; ++i)
			{
				if (!occupiedLocations.ContainsKey(i))
				{
					occupiedLocations.Add(i, obj);

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
        
		public GameObject TakeObject(string name)
		{
			for (int i = 0; i < transform.childCount; ++i) {
				Transform t = transform.GetChild(i);
				if(t.name == name) {
					KeyValuePair<int, GameObject>? keyValueExist = occupiedLocations.Where(x => x.Value == t.gameObject).FirstOrDefault();
					if(keyValueExist != null) {
						occupiedLocations.Remove(keyValueExist.Value.Key);
						return t.gameObject;
					}
				}
			}
			return null;
		}
        
		public bool HasObject(string obj, int number = 1)
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                if (transform.GetChild(i).name == obj)
                    number--;
            }
            return number <= 0;
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