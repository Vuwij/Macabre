using UnityEngine;
using System.Collections;

public class InventoryObjectClassA : InventoryObject {

	public InventoryObjectClassA(MItem.Item item) {
		itemList.Add(item);
		name += item.name;
	}
}
