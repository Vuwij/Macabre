using System;
using UnityEngine;
using Objects.Inventory.Individual;
using Objects.Immovable.Items;
using Objects.Inventory;

namespace Objects.Immovable.Furniture
{
	public class StorageFurniture : AbstractFurniture, IInspectable
	{
		public int storageCount = 1;
		public ObjectInventory storage;

		protected override void Start() {
			base.Start();
			reloadItems();
		}

		protected virtual void initializeInventory() {
			storage = new ObjectInventory(gameObject, storageCount);
		}

		#region IInspectable implementation

		public void InspectionAction (Object controller, UnityEngine.RaycastHit2D hit)
		{
			// Views the inventory of the table, allows the player to pick up objects
		}

		protected virtual void reloadItems() {}

		#endregion
	}
}

