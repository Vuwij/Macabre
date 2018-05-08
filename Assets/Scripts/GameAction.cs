using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class GameAction
{
	protected string actionString;

	// All the action types
	public enum ActionType {
		PUTS,
		TAKES,
		WALKTO,
		GIVES,
		CREATEITEM,
		ANIMATE,
		TELEPORT,
		ATTACK,
		LOCKDOOR,
		UPDATESTAT
	}
	public ActionType actionType;
	public float actionDelay = 0.5f;

	// Base components of an action
	public string priority;
	public string identity;
	public string action;
	public string quantity;
	public List<string> arguments = new List<string> ();

	public GameAction(string actionString) {
		this.actionString = actionString;
		ParseAction(actionString);
	}

	public static GameAction GetAction(string actionString) {
		Regex regex;
		string priority;
		string identity;

		// Reads the action prority
		regex = new Regex("\\d+. ?");
		priority = ParseRegexAndReplace (regex, ref actionString);

		// Read the action identity
		regex = new Regex("(.*?)+: ?");
		identity = ParseRegexAndReplace (regex, ref actionString);

		// Loop through the character list database
		return new GameAction(actionString);
	}

	public virtual void ParseAction(string actionString) {
		Regex regex;
		
		// Reads the action prority
		regex = new Regex("\\d+. ?");
		priority = ParseRegexAndReplace (regex, ref actionString);

		// Read the action identity
		regex = new Regex("(.*?)+: ?");
		identity = ParseRegexAndReplace (regex, ref actionString);

		// Read the action
		regex = new Regex ("^([\\w\\-]+) ?");
		action = ParseRegexAndReplace (regex, ref actionString);

		// Action Quantity
		regex = new Regex ("\\d+ ?");
		quantity = ParseRegexAndReplace (regex, ref actionString);

		// Action Objects
		regex = new Regex ("\\(([^\\s]+)\\) ?");
		string argument = " ";
		while (argument != "") {
			argument = ParseRegexAndReplace (regex, ref actionString);
			arguments.Add (argument);
		}
	}

	protected static string ParseRegexAndReplace(Regex regex, ref string actionString) {
		if(regex.IsMatch(actionString)) {
			string match = regex.Match (actionString).Captures [0].Value;
			actionString.Replace (match, "");
			if (match [match.Length - 1] == ' ')
				match = match.Remove (match.Length - 1); // Remove the space
			match = match.Replace ("(", "").Replace (")", "");
			return match;
		}
		return "";
	}

	public virtual void ExecuteAction() {
	}

	protected GameObject NewItem(string name) {
		var obj = Resources.Load ("Objects/Immovable/Items/" + name, typeof(GameObject)) as GameObject;
		return obj;
	}

	// Storage Object
	protected GameObject GetStorageFurniture(string name) {
		// Name is stored in the right format
		throw new NotImplementedException();
	}

	// Location
	protected GameObject GetLocation(string name) {
		throw new NotImplementedException();
	}

	// Characters
	protected GameObject GetCharacter(string name) {
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