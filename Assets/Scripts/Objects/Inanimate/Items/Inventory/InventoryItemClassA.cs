using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Data.Database;

namespace Objects.Inanimate.Items.Inventory
{
    public class InventoryItemClassA
    {
        public List<Item> items = new List<Item>();
        public int count { get { return items.Count; } }

        public string name
        {
            get
            {
                string s = "";
                foreach (Item i in items)
                    s += i.position + " ";
                return s;
            }
        }
        
        public InventoryItemClassA(Item item)
        {
            items.Add(item);
        }

        // For combining objects a and b, first check the combination amount
        public const int classALimit = 4;
        public static InventoryItemClassA operator +(InventoryItemClassA a, InventoryItemClassA b)
        {
            if (a.count + b.count > classALimit) return null;
            a.items.AddRange(b.items);
            return a;
        }
    }
}