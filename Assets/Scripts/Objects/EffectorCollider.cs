using UnityEngine;
using System.Collections.Generic;

namespace Objects
{
	public class EffectorCollider : PixelCollider
    {
		public List<GameObject> enabledWhenInside = new List<GameObject>();
		public List<GameObject> disabledWhenInside = new List<GameObject>();

		public Sprite effectorSprite; // Sprite that shows up when you are within the object
        public Sprite originalSprite;

		protected override void Awake()
		{
			base.Awake();

			SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                originalSprite = sr.sprite;
		}

		public void OnEffectorEnter()
		{
			foreach (GameObject g in enabledWhenInside)
				g.SetActive(true);
			
			foreach (GameObject g in disabledWhenInside)
				g.SetActive(false);

			SpriteRenderer sr = this.transform.parent.GetComponent<SpriteRenderer>();
			sr.sprite = effectorSprite;
		}

        public void OnEffectorExit()
		{
			foreach (GameObject g in enabledWhenInside)
				g.SetActive(false);

            foreach (GameObject g in disabledWhenInside)
				g.SetActive(true);

			SpriteRenderer sr = this.transform.parent.GetComponent<SpriteRenderer>();
			sr.sprite = originalSprite;
		}
	}
}
