using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public static class UIManager {
    private static GameObject UIScreens
    {
        get
        {
            GameObject g = GameObject.Find("UI Screen");
            if(g == null) Debug.LogError("Warning: UI Screen Not Found");
            return g;
        }
    }
    private static UIScreen darkScreen = new UIScreen("Dark Screen");
    private static List<UIScreen> screenList
    {
        get
        {
            if (UIScreen.screenList.Count == 0) FindUIScreens();
            if (UIScreen.screenList.Count == 0) throw new UnityException("Screens not loaded properly");
            return UIScreen.screenList;
        }
        set
        {
            UIScreen.screenList = value;
        }
    }
    
	// This function needs to be reused whenever anything in the UI Screen is used.
	public static void FindUIScreens() {
		CanvasGroup [] uChild = UIScreens.GetComponentsInChildren<CanvasGroup> ();

		foreach (CanvasGroup g in uChild) {
			Debug.Log (g.transform.name);

			if (g.transform.name == "UI Screen") continue;
			if (UIScreen.screenList.Find (x => x.getScreenName () == g.name) != null) continue;
			UIScreen.Add (g.transform.name);
		}
	}
    
    // Finds all the UI Screens
	public static void HideAllScreens() {
		foreach (UIScreen s in UIScreen.screenList) {
			s.turnOff ();
		}
	}
    
	public static void ToggleScreen(string screenName) {
		UIScreen.getScreen (screenName).toggle ();
	}

	public static void TurnOnScreen(string screenName) {
		UIScreen.getScreen (screenName).turnOn ();
	}

	public static void TurnOffScreen(string screenName) {
		UIScreen.getScreen (screenName).turnOff ();
	}

	public static bool ToggleDarkenScreen() {
		return darkScreen.toggle ();
	}
    
    public static void FadeShowScreen(string screenName)
    {
    }

    public static void FadeHideScreen(string screenName)
    {

    }

    static IEnumerable FadeShowHideScreen(bool fade = true)
    {
        yield break;
    }

}