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
	public class CharacterAction : GameAction
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
			UPDATESTAT
		}

		public int actionPriority;
		public Character character;
		public ActionType actionType;
		int quantity = 1;
		// Object 1 Type
		public object item1;
		// Object 2 Type
		public object item2;

		const float actionDelay = 0.5f;

		public CharacterAction(string actionString) {
			this.actionStringOriginal = actionString;
			this.actionString = actionString;
			ParseAction();
		}

		public virtual void ParseAction() {
			Regex regex;
			string match;

			// Reads the action prority
			regex = new Regex("\\d+. ?");
			if(regex.IsMatch(actionString)) {
				match = regex.Match(actionString).Captures[0].Value;
				actionString = actionString.Replace(match, "");
				actionPriority = Int32.Parse(match.Replace(".", "").Replace(" ", ""));
				Debug.Log(actionPriority);
			}

			// Character Name
			regex = new Regex("(.*?)+: ?");
			if(regex.IsMatch(actionString)) {
				match = regex.Match(actionString).Captures[0].Value;
				actionString = actionString.Replace(match, "");
				match = match.Replace(":","");
				if(match[match.Length-1] == ' ')
					match = match.Remove(match.Length-1); // Remove the space
				var allcharacters = GameObject.FindObjectsOfType<Character>();
				foreach(var c in allcharacters) {
					if(c.name == match) {
						character = c;
						break;
					}
				}
			}

			// Character Action
			if(character != null) {

				// Action Type
				regex = new Regex("^([\\w\\-]+) ?");
				if(regex.IsMatch(actionString)) {
					match = regex.Match(actionString).Captures[0].Value;
					actionString = actionString.Replace(match, "");
					if(match[match.Length-1] == ' ')
						match = match.Remove(match.Length-1); // Remove the space
					foreach(ActionType a in System.Enum.GetValues(typeof(ActionType))) {
						if(a.ToString() == match) {
							actionType = a;
							break;
						}
					}
				}

				// Action Quantity
				if(actionType == ActionType.GIVES 
					|| actionType == ActionType.PUTS 
					|| actionType == ActionType.TAKES) {

					regex = new Regex("\\d+ ?");
					if(regex.IsMatch(actionString)) {
						match = regex.Match(actionString).Captures[0].Value;
						actionString = actionString.Replace(match, "");
						if(match[match.Length-1] == ' ')
							match = match.Remove(match.Length-1); // Remove the space
						quantity = int.Parse(match);
					}
				}

				// Action Object 1
				regex = new Regex("\\(([^\\s]+)\\) ?");
				if(regex.IsMatch(actionString)) {
					match = regex.Match(actionString).Captures[0].Value;
					actionString.Replace(match, "");
					if(match[match.Length-1] == ' ')
						match = match.Remove(match.Length-1); // Remove the space
					match = match.Replace("(", "").Replace(")", "");

					// Item
					switch(actionType) {
					case ActionType.GIVES: // Object
					case ActionType.PUTS:
					case ActionType.TAKES:
						item1 = GetItem(match);
						break;
					case ActionType.WALKTO: // Location
					case ActionType.TELEPORT:
						item1 = GetLocation(match);
						break;
					case ActionType.ATTACK: // Character
						item1 = GetCharacter(match);
						break;
					case ActionType.ANIMATE: // Animation
						item1 = GetAnimation(match);
						break;
					case ActionType.UPDATESTAT: // Stat
						item1 = GetStat(match);
						break;
					}
				}

				// Action Object 2
				if(actionType == ActionType.PUTS
					|| actionType == ActionType.GIVES
					|| actionType == ActionType.TAKES) {

					regex = new Regex("\\(([^\\s]+)\\) ?");
					if(regex.IsMatch(actionString)) {
						match = regex.Match(actionString).Captures[0].Value;
						actionString.Replace(match, "");
						if(match[match.Length-1] == ' ')
							match = match.Remove(match.Length-1); // Remove the space
						match = match.Replace("(", "").Replace(")", "");

						if(actionType == ActionType.PUTS ||
							actionType == ActionType.TAKES) {
							item2 = GetStorageFurniture(match);
						}
						else if(actionType == ActionType.GIVES) {
							item2 = GetCharacter(match);
						}
					}
				}
			} else {
				base.ParseAction();
			}
		}

		public override void ExecuteAction() {

		}
	}
}

