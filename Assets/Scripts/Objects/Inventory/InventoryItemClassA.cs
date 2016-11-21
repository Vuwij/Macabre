using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Data.Database;

using Objects.Inanimate.Items;
using UI.Panels;

namespace Objects.Inventory
{
    public class InventoryItemClassA : InventoryItem
    {
        public List<ItemController> items = new List<ItemController>();
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

        public InventoryItemClassA(ItemController item)
        {
            items.Add(item);
            UpdateInventoryScreen();
        }

        // For combining objects a and b, first check the combination amount
        public const int classALimit = 4;
        public static InventoryItemClassA operator +(InventoryItemClassA a, InventoryItemClassA b)
        {
            if (a.count + b.count > classALimit) return null;
            a.items.AddRange(b.items);
            b.items.Clear();
            return a;
        }
        
        private void UpdateInventoryScreen()
        {
            InventoryPanel panel = UI.UIManager.Find<InventoryPanel>();
            // For uncombined objects
        }
    }
}