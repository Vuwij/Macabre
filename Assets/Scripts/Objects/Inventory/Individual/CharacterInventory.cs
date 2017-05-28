using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Objects.Unmovable.Items;

namespace Objects.Inventory.Individual
{
    public class CharacterInventory : Inventory {

		public CharacterInventory(GameObject gameObject, int classALimit, int classBLimit)
			: base(gameObject, classALimit, classBLimit) {}

		public void Drop(InventoryItem iitem)
        {
            List<Item> individualItems = new List<Item>();
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