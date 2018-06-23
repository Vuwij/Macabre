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

		protected virtual void Start()
        {
			InvokeRepeating("KillHoverText", 0.0f, 0.1f);
        }

        // Topologically sorts all neighbouring colliders with the current object
        public virtual void UpdateSortingLayer() {
            PixelCollider pixelCollider = GetComponentInChildren<PixelCollider>();
            Debug.Assert(pixelCollider != null);
            pixelCollider.TopologicalSortNearbySortingLayers();
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
              
		public void BorderHighlight(Color c, int thickness) {
			if(spriteRenderer == null) return;
			var sprite = spriteRenderer.sprite;
			if(sprite == null) return;

			var texture = sprite.texture;
			var rect = sprite.rect;

		}

		public void ShowHoverText(float duration = 0.2f) {
			Debug.Log("Hovering over " + this.name);
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
