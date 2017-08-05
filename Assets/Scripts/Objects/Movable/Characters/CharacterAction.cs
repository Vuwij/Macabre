using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using System.Linq;
using Objects.Immovable.Items;
using Objects.Inventory;
using Objects.Inventory.Individual;
using Objects.Movable.Characters.Individuals;
using System.Xml.Serialization;
using Objects.Immovable;
using Objects.Immovable.Furniture;
using System.Text.RegularExpressions;

namespace Objects.Movable.Characters
{
	public class CharacterAction
	{
		public enum ActionType {
			PUTS,
			TAKES,
			WALKTO,
			GIVES,
			ANIMATE,
			TELEPORT,
			ATTACK,
			LOCKDOOR,
		}

		public string actionString;
		public Character character;
		public ActionType actionType;
		public Item item;
		const float actionDelay = 0.5f;

		public CharacterAction(string actionString) {
			this.actionString = actionString;

			// Reads the action string
			Regex charRegex = new Regex("(.*?)+:");
			var match = charRegex.Match(actionString);
			Debug.Log(match.Value);


		}
		public CharacterAction ()
		{
			
		}
	}
}

