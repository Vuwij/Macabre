using System;
using Objects.Inventory;
using UnityEngine;

namespace Objects.Immovable.Furniture
{
	public class Table : StorageFurniture
	{
		public Vector2 verticalOffset = new Vector2(0, 15.0f);

		protected override void Start() {
			base.Start();
		}

		protected override void reloadItems() {
			foreach(InventoryItem ii in storage.classAItems) {
				foreach(var item in ii) {
					item.gameObject.transform.parent = transform;
					item.gameObject.transform.localPosition = verticalOffset + ii.tableOffset;
				}
			}
		}
	}
}

