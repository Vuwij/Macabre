using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Objects.Inanimate.Items;

namespace Objects.Inventory.Individual
{
    public class CharacterInventory : Inventory {

        public void Drop(InventoryItem iitem)
        {
            List<ItemController> individualItems = new List<ItemController>();
            if (iitem is InventoryItemClassA)
            {
                individualItems.AddRange((iitem as InventoryItemClassA).items);
                classAItems.Remove(iitem as InventoryItemClassA);
            }
            else
            {
                individualItems.Add((iitem as InventoryItemClassB).item);
                classAItems.Remove(iitem as InventoryItemClassA);
            }
            
            foreach (var item in individualItems)
                item.Drop();
        }	
	}
}