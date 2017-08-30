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
		public CharacterAction(string actionString) : base(actionString) {
			
		}

		public override void ExecuteAction() {
			switch (actionType) {
			case ActionType.ANIMATE:
				break;
			case ActionType.ATTACK:
				break;
			case ActionType.CREATEITEM:
				break;
			case ActionType.GIVES:
				break;
			case ActionType.PUTS:
				break;
			case ActionType.TAKES:
				break;
			case ActionType.TELEPORT:
				break;
			case ActionType.UPDATESTAT:
				break;
			case ActionType.WALKTO:
				break;
			}
		}
	}
}

