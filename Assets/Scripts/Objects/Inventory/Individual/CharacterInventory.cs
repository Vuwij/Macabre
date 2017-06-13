using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Objects.Immovable.Items;

namespace Objects.Inventory.Individual
{
    public class CharacterInventory : Inventory {

		public CharacterInventory(GameObject gameObject, int classALimit, int classBLimit)
			: base(gameObject, classALimit, classBLimit) {}

		public void Drop(InventoryItem iitem)
        {
            if (iitem is InventoryItemClassA)
            {
				foreach (var item in iitem.items)
					item.Drop();
            }
            else
            {
				iitem.items[0].Drop();
            }
        }	
	}
}