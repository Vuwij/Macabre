using Objects.Immovable.Items;
using System;
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

namespace Objects.Inventory
{
	[Serializable]
	public abstract class InventoryItem : IEnumerable
    {
		public Item this[int index]
		{
			get {
				return items[index];
			}
			set {
				items[index] = value;
			}
		}

		public Inventory inventory;
		public Vector2 tableOffset;
		public List<Item> items = new List<Item>();

        public InventoryItem(Item itemController, Inventory inventory)
        {
            this.inventory = inventory;
        }

		#region IEnumerable implementation

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return items.GetEnumerator();
		}

		public void Add(Item i) {
			items.Add(i);
		}

		public void Clear() {
			items.Clear();
		}


		#endregion
    }
}