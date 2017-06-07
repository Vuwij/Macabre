using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Data.Database;

using Objects.Immovable.Items;

namespace Objects.Inventory
{
    public class InventoryItemClassB : InventoryItem
    {
        public Item item;
        
        // Contructor for converting objects on the ground
        public InventoryItemClassB(Item item, Inventory inventory) : base(item, inventory)
        {
            this.item = item;
        }

        public string name
        {
            get { return item.name; }
        }

        public void Dispose()
        {
            inventory.classBItems.Remove(this);
        }
    }
}