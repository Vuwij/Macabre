using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The idea of the inventory object is that it turns into a MacabreObject when it is dropped and once its picked up it turns
/// into an inventory object. Thus an implicit conversion is requested
/// The InventoryObject will wrap the MItem class
/// </summary>
public class InventoryObject {

	/// <summary>
	/// The Item that the inventory Object wraps around it
	/// </summary>
	public List<MItem.Item> itemList = new List<MItem.Item>();
	public string name;

	// The number you can combine
	[Range (1, 4)]
	public int combinationStack = 1;

	// Contructor for converting objects on the ground
	public InventoryObject() {}
	public InventoryObject(MItem.Item item) {
		itemList.Add(item);
		name += item.name;
	}

	// If you want to convert the MacabreItem into an Inventory Object. It will have to be wrapped around first
	public static explicit operator InventoryObject (MItem.Item m)
	{
		var data = DatabaseManager.main.ItemFindData(m.name);
		if(data is MItem.ClassAData) return new InventoryObjectClassA(m);
		if(data is MItem.ClassBData) return new InventoryObjectClassB(m);

		Debug.LogError("The object " + m.name + " cannot be converted into an Inventory Object");
		return null;
	}

	// For combining objects a and b, first check the combination amount
	public static InventoryObject operator +(InventoryObject a, InventoryObject b)
	{
		// This exception is if the items literally cannot be combined
		if (a.combinationStack + b.combinationStack > 4) {
			//TODO: Need a warning message to the player that the items cannot be combined
			string s = "The items: " + a.name + " and " + b.name + " cannot be combined." +
				" The maximum combination stack is 4 and the current stack is " + a.combinationStack + b.combinationStack;
			Debug.LogWarning (s);
			return a;
		}

		// This exception is if the type a and b are both
		if(a is InventoryObjectClassB && b is InventoryObjectClassB) {
			string s = "The items: " + a.name + " and " + b.name + " cannot be combined." +
				" The maximum combination stack is 4 and the current stack is " + a.combinationStack + b.combinationStack;
			return a;
		}

		// Stacking the inventory objects together
		a.combinationStack += b.combinationStack;
		a.itemList.AddRange(b.itemList);
		a.name += " + " + b.name;
		return a;
	}

}