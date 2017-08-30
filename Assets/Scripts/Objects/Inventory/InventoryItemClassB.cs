using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Data.Databases;

using Objects.Immovable.Items;

namespace Objects.Inventory
{
	[Serializable]
	public class InventoryItemClassB : InventoryItem
    {
		public string name
		{
			get { return "TBA"; }
		}

		// Contructor for converting objects on the ground
        public InventoryItemClassB(Item item, Inventory inventory) : base(item, inventory)
        {
			if(this.items.Count == 0)
				this.Add(item);
        }

        public void Dispose()
        {
            inventory.classBItems.Remove(this);
        }
    }
}