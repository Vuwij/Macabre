using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : Manager {
	public static UIManager main;

	// The dark screen is frequently used
	private UIScreen darkScreen;

	#region Setup

	void OnEnable () {
		if(!main) main = this;
		else Destroy(gameObject);
		DontDestroyOnLoad (gameObject);
	}

	void Start ()
	{
		FindGameObjects ();
	}

	// This function needs to be reused whenever anything in the UI Screen is used.
	public void FindGameObjects() {
		if (GameObject.Find ("UI Screen") == null) {
			Debug.LogWarning ("Warning: UI Screen Not Found");
			return;
		}

		// Some default ones
		darkScreen = new UIScreen("Dark Screen");

		// Find other screens
		GameObject u = GameObject.Find ("UI Screen");
		CanvasGroup [] uChild = u.GetComponentsInChildren<CanvasGroup> ();

		foreach (CanvasGroup g in uChild) {
			Debug.Log (g.transform.name);

			if (g.transform.name == "UI Screen") continue;
			if (UIScreen.screenList.Find (x => x.getScreenName () == g.name) != null) continue;
			UIScreen.Add (g.transform.name);
		}

		FindDialogueObjects();
		CancelInvoke ("FindGameObjects");
	}

	#endregion
    
	public bool AssertUIScreen(string screenName) {
		if (UIScreen.getScreen (screenName) == null) {
			Debug.Log (screenName + " not found");
			return false;
		}
		Debug.Log (screenName + " found");
		return true;
	}

	public void HideAllScreens() {
		foreach (UIScreen s in UIScreen.screenList) {
			s.turnOff ();
		}
	}

	public bool GetStatusOfScreen(string screenName) {
		return UIScreen.getScreen (screenName).on;
	}

	public void ToggleScreen(string screenName) {
		UIScreen.getScreen (screenName).toggle ();
	}

	public void TurnOnScreen(string screenName) {
		UIScreen.getScreen (screenName).turnOn ();
	}

	public void TurnOffScreen(string screenName) {
		UIScreen.getScreen (screenName).turnOff ();
	}

	public bool ToggleDarkenScreen() {
		return darkScreen.toggle ();
	}
}