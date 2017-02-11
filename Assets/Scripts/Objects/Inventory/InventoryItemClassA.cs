using System;
using System.Collections.Generic;

using Data.Database;

using Objects.Inanimate.Items;
using UnityEngine;

namespace Objects.Inventory
{
    public class InventoryItemClassA : InventoryItem
    {
        public List<ItemController> items = new List<ItemController>();
        public const int classALimit = 4;

        public InventoryItemClassA(ItemController item, Inventory inventory) : base(item, inventory)
        {
            items.Add(item);
        }

        public void Dispose()
        {
            items.Clear();
            inventory.classAItems.Remove(this);
        }

        public int count { get { return items.Count; } }
        public string name
        {
            get
            {
                string s = "";
                foreach (ItemController i in items)
                    s += i.item.position + " ";
                return s;
            }
        }

        // For combining objects a and b, first check the combination amount
        public static InventoryItemClassA operator +(InventoryItemClassA a, InventoryItemClassA b)
        {
            Item combined = null;
            Inventory inventory = a.inventory;

            // If a combination could be found
            if (a.count == 1 && b.count == 1)
                combined = DatabaseConnection.ItemDB.FindCombination(a.items[0].item, b.items[0].item);

            if (combined != null)
            {
                var newItemController = Items.main.GetItem(combined.name);

                // Move the new item into the same location
                newItemController.transform.parent = a.items[0].transform.parent;

                // Remove A and B in the objects
                UnityEngine.Object.Destroy(a.items[0].gameObject);
                UnityEngine.Object.Destroy(b.items[0].gameObject);
                a.Dispose();
                b.Dispose();

                // Add new object in the inventory
                newItemController.gameObject.SetActive(false);
                inventory.Add(newItemController);
            }
            else
            {
                if (a.count + b.count > classALimit) return null;
                a.items.AddRange(b.items);
                b.Dispose();
            }
            
            return a;
        }
    }
}