using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class EventManager : Manager
{

	/* ---------------------------------
	 *  	THE DICTIONARY OF EVENTS
	 * ---------------------------------*/
	static EventDictionary<EventKey, EventAction> events = new EventDictionary<EventKey, EventAction>();

	void Start ()
	{
		LoadEventsFromDatabase ();
	}
	public static void ToggleEvent (string eventName)
	{
		
	}

	private void LoadEventsFromDatabase ()
	{
		// Do some magic here
	}
}

public class EventKey {
	readonly string key;
	public EventKey (string key_)
	{
		this.key = key_;
	}
	public static implicit operator string (EventKey e)
	{
		return e.ToString ();
	}
	public static implicit operator EventKey (string s)
	{
		return new EventKey (s);
	}
	EventAction e = new EventAction ();
}

public class EventAction
{
	public delegate void Action ();
	public event Action action;
	public bool repeatable = true;

	public List<EventAction> ExclusionList;
	public List<EventAction> PrerequisiteList;

	public int actionCount;

	public EventAction ()
	{
		action += () => actionCount++;
	}

	public void Execute () { action (); }

}

public class EventDictionary<TKey, TValue> : Dictionary<TKey, TValue>
	where TKey : EventKey
	where TValue : EventAction {
}