using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class PixelItem : MonoBehaviour
    {
        [System.Serializable]
        public class Combination {
            public PixelItem with;
            public PixelItem result;
        }

        public List<Combination> combinations;
        public List<PixelItem> breakapart;
        public bool breakable = true;

        [HideInInspector] 
        public int id;
        public string description;
        public string[] properties;

        public bool isLargeItem {
            get {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                Debug.Assert(spriteRenderer != null);
                if ((int)spriteRenderer.sprite.rect.width == 16 && (int) spriteRenderer.sprite.rect.height == 16)
                    return false;
                return true;
            }
        }

		public void Start()
		{
            UpdateFromPrefab();
		}

        void UpdateFromPrefab()
        {
            PixelItem prefab = Resources.Load<PixelItem>("Items/" + name);
            id = prefab.id;
            description = prefab.description;
            properties = prefab.properties;
            combinations = prefab.combinations;
        }

		public static PixelItem Combine(PixelItem a, PixelItem b)
        {
            Debug.Assert(a != b);
            foreach (Combination c in a.combinations)
            {
                if (c.with.name == b.name)
                {
                    return c.result;
                }
            }
            foreach (Combination c in b.combinations)
            {
                if (c.with.name == a.name)
                {
                    return c.result;
                }
            }
            return null;
        }
    }
}
