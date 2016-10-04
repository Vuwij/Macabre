using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

using Events;

/// <summary>
/// This class is for a dictionary of events
/// </summary>
public static class EventManager
{

	/* ---------------------------------
	 *  	THE DICTIONARY OF EVENTS
	 * ---------------------------------*/
	static EventDictionary<EventKey, EventAction> events = new EventDictionary<EventKey, EventAction>();

	static void Start ()
	{
		LoadEventsFromDatabase ();
	}

	public static void ToggleEvent (string eventName)
	{
		
	}

	private static void LoadEventsFromDatabase ()
	{
		// Do some magic here
	}
}