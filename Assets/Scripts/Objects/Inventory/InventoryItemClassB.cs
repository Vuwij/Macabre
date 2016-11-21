using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Data.Database;

using Objects.Inanimate.Items;

namespace Objects.Inventory
{
    public class InventoryItemClassB : InventoryItem
    {
        public ItemController item;
        public string name
        {
            get { return item.name; }
        }
        
        // Contructor for converting objects on the ground
        public InventoryItemClassB(ItemController item)
        {
            this.item = item;
        }
    }
}