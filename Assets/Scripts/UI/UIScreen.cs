using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIScreen
{
	#region Screen List

	public static List<UIScreen> screenList = new List<UIScreen>();
	public static UIScreen getScreen (string s)
	{
		UIScreen u = screenList.Find (x => x.screenName == s);
		if (u == null) Debug.LogError (s + " not found");
		return u;
	}
	public static void Add (string s)
	{
		UIScreen u = new UIScreen (s);
	}

	#endregion

	public bool on;
	private GameObject screen;
	private string screenName;

	public UIScreen (string name)
	{
		screen = GameObject.Find (name);
		screenName = name;
		on = false;

		screenList.Add (this);

		if (screen) {
			screen.GetComponent<CanvasGroup> ().interactable = false;
			screen.GetComponent<CanvasGroup> ().blocksRaycasts = false;
			screen.GetComponent<CanvasGroup> ().ignoreParentGroups = true;
		} else {
			Debug.LogError (screenName + " not found");
		}
	}

	public bool toggle ()
	{
		if (screen) {
			if (System.Math.Abs (screen.GetComponent<CanvasGroup> ().alpha) < float.Epsilon) {
				on = false;
				turnOn ();
				Debug.Log (screenName + " turned on");
				return false;
			} else {
				on = true;
				turnOff ();
				Debug.Log (screenName + " turned off");
				return true;
			}
		} else {
			Debug.LogError (screenName + " Not Found");
		}
		return false;
	}

	public void turnOn ()
	{
		if (screen) {
			if (!on) {
				screen.GetComponent<CanvasGroup> ().alpha = 1;
				screen.GetComponent<CanvasGroup> ().interactable = true;
				screen.GetComponent<CanvasGroup> ().blocksRaycasts = true;
				on = true;
			}
		} else {
			Debug.LogError (screenName + " Not Found");
		}
	}

	public void turnOff ()
	{
		if (screen) {
			if (on) {
				screen.GetComponent<CanvasGroup> ().alpha = 0;
				screen.GetComponent<CanvasGroup> ().interactable = false;
				screen.GetComponent<CanvasGroup> ().blocksRaycasts = false;
				on = false;
			}
		} else {
			Debug.LogError (screenName + " Not Found");
		}
	}

	public void print ()
	{
		Debug.Log (screen.ToString ());
	}

	public GameObject getScreen ()
	{
		return screen;
	}

	public string getScreenName ()
	{
		return screenName;
	}

}