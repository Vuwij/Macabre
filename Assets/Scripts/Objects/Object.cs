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
		protected bool highlighted {
			set {
				if(spriteRenderer == null) return;
				var sprite = spriteRenderer.sprite;
				if(sprite == null) return;

				var c = spriteRenderer.color;
				if(value) c.b = c.b / 3.0f;
				else c.b = c.b * 3.0f;
				spriteRenderer.color = c;
			}
		}

		Object objectInFront;
		static GameObject hoverTemplate = null;
		GameObject hoverText = null;
		float hoverTextTimer = 0.0f;
		public Texture2D footprint;
		public Sprite highlight;
		public string interactionText = "Press T to Interact";
		protected int sortingOffset = 0;

		protected virtual void Start()
        {
			UpdateSortingLayer();
			InvokeRepeating("KillHoverText", 0.0f, 0.1f);
        }

		public virtual void UpdateSortingLayer(int? offset = null) {
			if(offset != null) sortingOffset = (int) offset;
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

		protected Object FindNearestObject<T>(float radius = 10.0f)
		{
			RaycastHit2D[] castStar = Physics2D.CircleCastAll(transform.position, radius, Vector2.zero);

			foreach (RaycastHit2D raycastHit in castStar)
			{
				if(raycastHit.collider.isTrigger) continue;
				T hit = raycastHit.collider.GetComponentInChildren<T>();
				if (hit != null) {
					if(raycastHit.collider.gameObject != gameObject) {
						return raycastHit.collider.gameObject.GetComponent<Object>();
					}
				}
			}
			return null;
		}

		protected Object FindInspectableAroundPosition(Vector2 position) {
			Vector3 castStart = (Vector3) position;
			castStart.z = -10.0f;
			RaycastHit[] hits = Physics.RaycastAll(position, Vector3.back);

			foreach(var hit in hits) {
				if(hit.collider.gameObject.GetComponent<IInspectable>() != null) {
					return hit.collider.gameObject.GetComponent<Object>();
				}
			}
			return null;
		}

		protected Object FindInspectablePixelAroundPosition(Vector2 position) {
			Vector3 castStart = (Vector3) position;
			castStart.z = -10.0f;

			RaycastHit2D[] castStar = Physics2D.CircleCastAll(castStart, GameSettings.inspectRadius * 2, Vector2.zero);

			Objects.Object closestObject = null;
			int closestObjectSortingOrder = 0;
			int closestObjectSortingLayer = 0;
			foreach (RaycastHit2D raycastHit in castStar)
			{
				if(raycastHit.collider.gameObject == this.gameObject) continue;
				if(raycastHit.collider.gameObject.GetComponentInChildren<IInspectable>() == null) continue;
				var spriteRenderer = raycastHit.collider.gameObject.GetComponentInChildren<SpriteRenderer>();
				if(spriteRenderer == null) continue;

				Sprite sprite = spriteRenderer.sprite;
				Rect rect = sprite.rect;
				int x = Mathf.FloorToInt(rect.x);
				int y = Mathf.FloorToInt(rect.y);
				int width = Mathf.FloorToInt(rect.width);
				int height = Mathf.FloorToInt(rect.height);

				Vector2 objectToMouse = position - ((Vector2) raycastHit.collider.gameObject.transform.position - sprite.pivot);
				//Debug.DrawLine(position, ((Vector2) raycastHit.collider.gameObject.transform.position - sprite.pivot), Color.red, 10.0f);

				if(objectToMouse.x < 0 || objectToMouse.y < 0) continue;
				if(objectToMouse.x > width) continue;
				if(objectToMouse.y > height) continue;

				Color c = sprite.texture.GetPixel(x + Mathf.FloorToInt(objectToMouse.x), y + Mathf.FloorToInt(objectToMouse.y));
				if(c.a == 0) continue;

				if(closestObject == null
					|| spriteRenderer.sortingLayerID < closestObjectSortingLayer
					|| spriteRenderer.sortingOrder < closestObjectSortingOrder) {
					closestObject = raycastHit.collider.gameObject.GetComponent<Objects.Object>();
					closestObjectSortingLayer = spriteRenderer.sortingLayerID;
					closestObjectSortingOrder = spriteRenderer.sortingOrder;
				}
			}
			return closestObject;
		}

		public void BorderHighlight(Color c, int thickness) {
			if(spriteRenderer == null) return;
			var sprite = spriteRenderer.sprite;
			if(sprite == null) return;

			var texture = sprite.texture;
			var rect = sprite.rect;

		}

		public void ShowHoverText(float duration = 0.2f) {
			Debug.Log("Hovering over" + this.name);
			if(hoverTemplate == null) {
				hoverTemplate = Resources.Load<GameObject>("UI/Dialogue/ItemDialogue");
			}
			hoverTextTimer = duration;
			if(hoverText != null) return;
			hoverText = Instantiate(hoverTemplate, transform);
			hoverText.GetComponent<MeshRenderer>().sortingLayerName = "GameUI";
			hoverText.GetComponent<TextMesh>().text = name;
		}

		void KillHoverText() {
			if(hoverText != null) {
				hoverTextTimer = hoverTextTimer - 0.1f;
				if(hoverTextTimer <= 0)
					Destroy(hoverText);
			}
		}
    }
}
