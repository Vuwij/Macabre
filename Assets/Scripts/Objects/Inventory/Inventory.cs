using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Objects.Immovable.Items;

namespace Objects.Inventory {

	[Serializable]
	public abstract class Inventory {
		public Transform folder
		{
			get
			{
				if(gameObject.GetComponentsInChildren<Transform>().SingleOrDefault(x => x.name == "Inventory") == null)
				{
					GameObject inventoryFolder = new GameObject("Inventory");
					inventoryFolder.transform.parent = gameObject.transform;
				}

				return gameObject.GetComponentsInChildren<Transform>().SingleOrDefault(x => x.name == "Inventory");
			}
		}
		public int count {
			get {
				int c = 0;
				foreach(var item in classAItems) {
					foreach(var i in item.items) {
						if(i != null) c++;
					}
				}
				foreach(var item in classBItems) {
					foreach(var i in item.items) {
						if(i != null) c++;
					}
				}
				return c;
			}
		}

		GameObject gameObject;
		[HideInInspector]
		public int classALimit = 6, classBLimit = 1;
		public List<InventoryItemClassA> classAItems = new List<InventoryItemClassA>();
		public List<InventoryItemClassB> classBItems = new List<InventoryItemClassB>();

        public Inventory(GameObject gameObject, int classALimit, int classBLimit)
        {
			this.gameObject = gameObject;
			this.classALimit = classALimit;
            this.classBLimit = classBLimit;
        }

        public virtual bool Add(Item item) {

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