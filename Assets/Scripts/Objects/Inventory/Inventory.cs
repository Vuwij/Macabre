using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Objects.Inanimate.Items;

namespace Objects.Inventory {

	public abstract class Inventory {
		public int classALimit = 6, classBLimit = 1;
		public List<InventoryItemClassA> classAItems = new List<InventoryItemClassA>();
		public List<InventoryItemClassB> classBItems = new List<InventoryItemClassB>();

        public Inventory(int classALimit = 6, int classBLimit = 1)
        {
            this.classALimit = classALimit;
            this.classBLimit = classBLimit;
        }

        // Automatically add an item to inventory
		public virtual bool Add(ItemController item) {

            if (item.type == ItemType.InventoryItemClassB)
            {
                InventoryItemClassB itemB = new InventoryItemClassB(item, this);
                if (classBItems.Count >= classBLimit) return false;

                // Add the Item to the slot
                classBItems.Add(itemB);
                return true;
                
            }
            if (item.type == ItemType.InventoryItemClassA)
            {
                // Check if the limit has been reached
                if (classAItems.Count >= classALimit) return false;

                // Add the item into a new slot
                InventoryItemClassA itemA = new InventoryItemClassA(item, this);
                classAItems.Add(itemA);
                return true;
            }
            return false;
		}
	}
}