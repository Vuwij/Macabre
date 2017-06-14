using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Objects.Immovable.Items;

namespace Objects.Inventory.Individual
{
    public class CharacterInventory : Inventory {
		public static Vector2 DropCircle
		{
			get {
				var randomCircle = (Vector2) UnityEngine.Random.onUnitSphere;
				randomCircle.Scale(new Vector2(GameSettings.dropDistance, GameSettings.dropDistance));
				return randomCircle;
			}
		}

		public CharacterInventory(GameObject gameObject, int classALimit, int classBLimit)
			: base(gameObject, classALimit, classBLimit) {}

		public void Drop(InventoryItem iitem)
        {
            if (iitem is InventoryItemClassA) {
				foreach (var item in iitem.items) {
					DropItem(item);
				}
				classAItems.Remove(iitem as InventoryItemClassA);
            }
            else {
				DropItem(iitem.items[0]);
				classBItems.Remove(iitem as InventoryItemClassB);
            }
        }

		void DropItem(Item item) {
			item.gameObject.SetActive(true);
			var inventoryFolder = item.transform.parent;
			item.transform.position = (Vector2) inventoryFolder.parent.position + DropCircle;
			item.transform.parent = item.transform.parent.parent.parent;
			if (inventoryFolder.GetComponentsInChildren<Transform>().Length == 0) GameObject.Destroy(inventoryFolder.gameObject);
		}

	}
}