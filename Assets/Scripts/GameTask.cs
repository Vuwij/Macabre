using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class GameTask
{
    // All the task types are enumerated here
    // Tasks are stored in queues stored in every character in the game
    // The current implementation is asynchronous and all the characters in the game do their own thing
    // without waiting for other characters to complete their task

	public enum TaskType {
		PUTS,
		TAKES,
		NAVIGATE,
		GIVES,
		CREATEITEM,
		ANIMATE,
		TELEPORT,
		ATTACK,
		UPDATESTAT
	}
	public TaskType taskType;
	public List<object> arguments = new List<object>();

	// Base components of an task for parsing the task
	// 1. Player: PUTS 10 (Gold) (Buildings_Inn_Floor 1 Room 1_Bar)
	// <1> priority - 2 tasks can happen at the same time
    // <Player> identity - which character/identity is doing what action
    // <PUTS> action - what the character does
    // <10> quantity - how much of the item
	// <(Gold), (Building_Inn_Floor 1 Room 1 bar) - Arguments

	// The string for execution
    string actionString;
	string priority;
	string identity;
	string action;
	string quantity;
	List<string> stringArguments = new List<string> ();
    
	public GameTask() {}

	public GameTask(string actionString) {
		this.actionString = actionString;
		ParseAction(actionString);
	}

	public GameTask(TaskType taskType, List<string> stringArguments) {
		this.taskType = taskType;
		this.stringArguments = stringArguments;
	}   

	public void ParseAction(string actionString) {
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
}