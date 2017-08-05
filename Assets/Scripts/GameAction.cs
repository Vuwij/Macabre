using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using System.Linq;
using Objects.Immovable.Items;
using Objects.Inventory;
using Objects.Inventory.Individual;
using Objects.Movable.Characters;
using Objects.Movable.Characters.Individuals;
using System.Xml.Serialization;
using Objects.Immovable;
using Objects.Immovable.Furniture;
using System.Text.RegularExpressions;

public class GameAction
{
	protected string actionString;
	public string actionStringOriginal;

	public virtual void ParseAction() {

	}

	public virtual void ExecuteAction() {

	}

	// Objects
	protected Item GetItem(string name) {
		throw new NotImplementedException();
	}

	// Storage Object
	protected StorageFurniture GetStorageFurniture(string name) {
		throw new NotImplementedException();
	}

	// Location
	protected GameObject GetLocation(string name) {
		throw new NotImplementedException();
	}

	// Characters
	protected Character GetCharacter(string name) {
		throw new NotImplementedException();
	}

	// Animation
	protected Animation GetAnimation(string name) {
		throw new NotImplementedException();
	}

	// Stats
	protected int GetStat(string name) {
		throw new NotImplementedException();
	}
}