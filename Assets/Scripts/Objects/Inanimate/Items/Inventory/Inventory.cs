using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Objects.Inanimate.Items.Inventory {

	public abstract class Inventory {
		public int classALimit, classBLimit;
		protected List<InventoryItemClassA> classAItems = new List<InventoryItemClassA>();
		protected List<InventoryItemClassB> classBItems = new List<InventoryItemClassB>();

        public Inventory(int classALimit = 0, int classBLimit = 0)
        {
            this.classALimit = classALimit;
            this.classBLimit = classBLimit;
        }

        // Automatically add an item to inventory
		public virtual void Add(Item item) {
            if(item.type == ItemType.InventoryItemClassB)
            {
                InventoryItemClassB itemB = new InventoryItemClassB(item);
                if (classBItems.Count >= classBLimit) classBItems.Add(itemB);
            }
            if (item.type == ItemType.InventoryItemClassA)
            {
                // Check if the limit has been reached
                if (classAItems.Count >= classALimit) return;

                // Add the item into a new slot
                InventoryItemClassA itemA = new InventoryItemClassA(item);
                classAItems.Add(itemA);
            }
		}
	}
}