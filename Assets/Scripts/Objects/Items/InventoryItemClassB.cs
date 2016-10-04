using UnityEngine;
using System.Collections;

public class InventoryObjectClassB : InventoryObject {

	public InventoryObjectClassB(MItem.Item item) {
		itemList.Add(item);
		name += item.name;
	}

}
