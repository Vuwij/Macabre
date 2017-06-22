using System;
using UnityEngine;
using Objects.Inventory.Individual;
using Objects.Immovable.Items;
using Objects.Inventory;
using UnityEditor;
using System.Collections;

namespace Objects.Immovable.Furniture
{
	public class StorageFurniture : AbstractFurniture, IInspectable, IItemContainer
	{
		public int storageCount = 1;
		public ObjectInventory storage;

		protected override void Start() {
			storage = new ObjectInventory(gameObject, storageCount);
			base.Start();
			reloadItems();
		}

		protected virtual void reloadItems() {
			// Erase items that don't exist
			foreach(var obj in GetComponentsInChildren<Item>()) {
				bool itemFound = false;
				foreach(InventoryItem ii in storage.classAItems) {
					if(ii.items.Find(x => x.name == obj.name)) itemFound = true;
				}
				if(!itemFound)
					StartCoroutine(Destroy(obj.gameObject));
			}

			// Add inventory
			foreach(InventoryItem ii in storage.classAItems) {
				for(int i = 0; i < ii.items.Count; i++) {
					if(ii.items[i] == null) continue;
					else if(PrefabUtility.GetPrefabParent(ii.items[i]) == null && PrefabUtility.GetPrefabObject(ii.items[i]) != null) {
						if(GetComponentInChildren<Transform>().Find(ii.items[i].name) == null) {
							var go = GameObject.Instantiate(ii.items[i], transform);
							go.name = ii.items[i].name;
							ii.items[i] = go;
						}
					}
					else {
						ii.items[i].gameObject.transform.parent = transform;
					}
				}
			}

			foreach(InventoryItem ii in storage.classAItems) {
				for(int i = 0; i < ii.items.Count; i++) {
					if(ii.items[i] == null) continue;
					ii.items[i].UpdateSortingLayer();
				}
			}
		}

		IEnumerator Destroy(GameObject go)
		{
			yield return new WaitForEndOfFrame();
			DestroyImmediate(go);
		}

		#region IInspectable implementation

		public void InspectionAction (Object controller, UnityEngine.RaycastHit2D hit)
		{
			// Views the inventory of the table, allows the player to pick up objects
		}

		#endregion
	}
}

