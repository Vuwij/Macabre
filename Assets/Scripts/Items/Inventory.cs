using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**********************************************
 * Macabre Inventory System

Hello Harry,

Basically while I'm currently working on the saving/loading system. I need you to help me figure out an inventory system.
Currently the way you pick things up is go to any object, give it a pickable tag (not sure if implemenbted already), grab that object from the prefabs folder. And walk over to it and press space. Please confirm with me if it doesn't work completely, and try to see whats going on.

The Inventory should be an object inside the Character class, and will be able to store the object called by the "AddToInventoru(Object obj) class" ( I think) IN the inventory region

Current Requirements
1. Be able to specify the number of objects the character stores
2. Be able to print out a list of objects
3. Be able to move objects around in the inventory, like this object is in object index 1, need to move to object index 2 as an empty slot
4. Be able to store the inventory on a MacabreStorage object (not yet implemented)

10 (customizable) slots for quick select
50 (customizable) slots for larger storage

5. Be able to store duplicates of an object

Future requirements
1. Implement a simple UI system, not yet designed by Jack
2. Be able to identify attributes of the object, by getting the attributes from the Invetory.db database inside the Databases folder
3. Be able to combine objects, also determined by the attributes from the Inventory.db object
4. Consumable objects (as determined by Inventory.db) - All the database cammands are in the DatabaseManager.db

Long term requirements
5. Be able to view two inventories ()
6. Have the merchant override the normal inventory system, be able to sell abd buy objects based on the values of money inside characterStat within the Character Class

*/

namespace Inventory {

	public class Inventory {
		public int classALimit, classBLimit;
		protected List<InventoryObjectClassA> classAInventory = new List<InventoryObjectClassA>();
		protected List<InventoryObjectClassB> classBInventory = new List<InventoryObjectClassB>();

		/// <summary>
		/// Adds an Macabre Item into the inventory. Inventory Item will attempt to be wrapped in the Inventory Item Wrapper
		/// </summary>
		public virtual bool Add(MItem.Item item) {
			InventoryObject i = (InventoryObject) item;

			if(i is InventoryObjectClassA) {
				if(classAInventory.Count > classALimit) {
					Debug.LogWarning("Too many objects in inventory");
					return false;
				}
				else {
					AddObject(i);
					classAInventory.Add(i as InventoryObjectClassA);
					return true;
				}
			}
			if(i is InventoryObjectClassB) {
				if(classAInventory.Count > classBLimit) {
					Debug.LogWarning("Too many objects in inventory");
					return false;
				}
				else {
					AddObject(i);
					classBInventory.Add(i as InventoryObjectClassB);
					return true;
				}
			}
			throw new UnityException("InventoryObject not of any class");
		}

		/// <summary>
		/// The private Internal part of the Inventory, called from the Add method
		/// </summary>
		protected virtual void AddObject(InventoryObject i) {
			foreach(MItem.Item m in i.itemList) {
				try {
					GameObject.Destroy(m.gameObject);

				} catch (System.Exception) {
					Debug.Log("Adding Object Failed");
				}
			}
		}

		private float DropScatterDistance = 0.05f;

		/// <summary>
		/// Drop the specified object at the dropLocation.
		/// </summary>
		/// <param name="i">The object to drop</param>
		/// <param name="dropLocation">Drop location.</param>
		public virtual void Drop(InventoryObject i, Transform dropLocation) {
			if(i is InventoryObjectClassA) classAInventory.Remove(i as InventoryObjectClassA);
			if(i is InventoryObjectClassB) classBInventory.Remove(i as InventoryObjectClassB);

			foreach(MItem.Item m in i.itemList) {
				try {
					// Hides the gameobject
					var color = m.gameObject.GetComponent<SpriteRenderer>().color;
					color.a = 0;

					// Replaces the object position slightly scattered in the location
					// TODO Might need to detect if the wall is nearby to avoid glitches
					Vector2 scatter = Vector2.Scale(Random.insideUnitCircle, new Vector2(DropScatterDistance, DropScatterDistance));
					scatter += (Vector2) dropLocation.position;
					m.transform.position = scatter;

					// Enables all of the colliders of that gameobject
					Collider2D[] collider = m.gameObject.GetComponents<Collider2D>();
					foreach(Collider2D c in collider) {
						c.enabled = true;
					}

				} catch (System.Exception) {
					Debug.Log("Dropping Object Failed");
				}
			}
		}
	}
}