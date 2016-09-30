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

	// This section deals with every individual screen
	#region ToggleScreen

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

	#endregion

	#region DialoguePanel

	private GameObject dialogueScreen;
	private CanvasGroup dialogueScreenGroup;
	private Text mainText;
	private Text titleText;
	private Text returnButtonText;
	private Image mainImage;
	private GameObject returnButton;
	private GameObject responseButtons;
	private Text[] responseTexts;

	private string returnButtonTextString = "Press Space to Continue";

	/// <summary>
	/// Closes the dialogue screen.
	/// </summary>
	/// <returns><c>true</c>, if dialogue screen was closed, <c>false</c> otherwise.</returns>
	public bool CloseDialogueScreen() {
		mainText.text = "";
		titleText.text = "";
		mainImage.sprite = null;
		ToggleResponseButtons (false);
		ToggleReturnButton (false);
		for(int i = 0; i < 4; i++) responseButtons.GetComponentsInChildren<Text>()[i].text = "";
		StartCoroutine("HideInspectTextBox");
		return true;
	}

	///<summary>For displaying the dialogue screen</summary>
	public bool DisplayDialogueScreen(Sprite mainImageEntry, string titleTextEntry, string mainTextEntry, bool continueText) {
		ResetDialogueScreen ();

		mainText.text = mainTextEntry;
		titleText.text = titleTextEntry;
		mainImage.sprite = mainImageEntry;
		if (continueText)
			ToggleReturnButton (true);
		returnButtonText.text = "Return";
		StartCoroutine ("ShowInspectTextBox");
		return true;
	}

	///<summary>Dialogue Screen for Character Speaking</summary>
	public bool DisplayCharacterText(Sprite mainImageEntry, string characterName, string textEntry, bool continueText) {
		ResetDialogueScreen ();

		if(mainImageEntry != null) mainImage.sprite = mainImageEntry;
		returnButtonText.text = returnButtonTextString;
		titleText.text = characterName;
		mainText.text = textEntry;
		StartCoroutine ("ShowInspectTextBox");
		return true;
	}

	///<summary>Dialogue Screen for Character Speaking Single</summary>
	public bool DisplaySingleResponse(Sprite mainImageEntry, string characterName, string textEntry, bool continueText) {
		ResetDialogueScreen ();

		if(mainImageEntry != null) mainImage.sprite = mainImageEntry;
		returnButtonText.text = returnButtonTextString;

		titleText.text = characterName;
		Debug.LogWarning (textEntry);
		mainText.text = textEntry;
		StartCoroutine ("ShowInspectTextBox");
		return true;
	}

	///<summary>Dialogue Screen for Character Speaking Multiple Respones</summary>
	public bool DisplayMultipleResponse(Sprite mainImageEntry, string characterName, string[] textEntry, bool continueText) {
		ResetDialogueScreen ();

		if(mainImageEntry != null) mainImage.sprite = mainImageEntry;
		returnButtonText.text = returnButtonTextString;

		ToggleResponseButtons (true);
		Text[] texts = responseButtons.GetComponentsInChildren<Text>();
		for (int i = 0; i < 4; i++) {
			texts [i].text += (textEntry[i] == null) ? "" : (i + 1).ToString();
			texts [i].text += ". " + textEntry [i];
		}
		
		StartCoroutine ("ShowInspectTextBox");
		return true;
	}

	/// <summary>
	/// Finds the dialogue objects
	/// </summary>
	/// <returns><c>true</c>, if dialogue objects was found, <c>false</c> otherwise.</returns>
	private bool FindDialogueObjects() {
		if(dialogueScreen == null) dialogueScreen = GameObject.Find ("Dialogue Screen");
		if (dialogueScreen == null)
			Debug.LogWarning ("Warning: Dialogue Screen Not Found");
		mainText = GameObject.Find ("Main Text").GetComponent<Text>();
		titleText = GameObject.Find ("Title Text").GetComponent<Text>();
		mainImage = GameObject.Find ("Main Image").GetComponent<Image>();
		returnButton = GameObject.Find ("Return Button");
		returnButtonText = GameObject.Find ("Return Text").GetComponent<Text>();
		dialogueScreenGroup = dialogueScreen.GetComponent<CanvasGroup>();
		responseButtons = GameObject.Find ("Response Buttons");
		if (!responseButtons)
			Debug.LogError ("Reponse Buttons Not Found");
		responseTexts = new Text[4];

		return true;
	}

	/// <summary>
	/// Resets the dialogue screen.
	/// </summary>
	private void ResetDialogueScreen() {
		mainImage.sprite = null;
		returnButtonText.text = "";
		titleText.text = "";
		mainText.text = "";
		ToggleResponseButtons (false);
		ToggleReturnButton (false);
		Component[] texts = responseButtons.GetComponentsInChildren<Text>();
		foreach (Text t in texts) {
			t.text = "";
		}
	}

	#endregion

	#region Inspection

	/// <summary>
	/// Hides the inspect text box.
	/// </summary>
	/// <returns>The inspect text box.</returns>
	private IEnumerator HideInspectTextBox() {
		while(dialogueScreenGroup.alpha > 0.0f) {
			dialogueScreenGroup.alpha -= 0.01f;
			yield return null;
		}
		yield break;
	}

	/// <summary>
	/// Shows the inspect text box.
	/// </summary>
	/// <returns>The inspect text box.</returns>
	private IEnumerator ShowInspectTextBox() {
		while(dialogueScreenGroup.alpha < 0.8f) {
			dialogueScreenGroup.alpha+= 0.01f;
			yield return null;
		}
		yield break;
	}

	private void ToggleResponseButtons(bool op) {
		if (!responseButtons)
			Debug.LogError ("Reponse Buttons not found");
		if(op) responseButtons.GetComponent<CanvasGroup>().alpha = 1;
		else responseButtons.GetComponent<CanvasGroup>().alpha = 0;
	}

	private void ToggleReturnButton(bool op) {
		if (!returnButton)
			Debug.LogError ("Return button not found");
		if (op) {
			returnButton.GetComponent<Button> ().interactable = true;
			var color = returnButton.GetComponent<Image> ().color;
			color.a = 255;
		} else {
			returnButton.GetComponent<Button> ().interactable = false;
			var color = returnButton.GetComponent<Image> ().color;
			color.a = 0;
		}
	}

	#endregion

	#region InventoryPanel

	#endregion

	#region Warning Panel

	/// <summary>
	/// Played after the warning
	/// </summary>
	public delegate void WarningAction(string option);

	/// <summary>
	/// <para>Warning the specified message, with a return </para>
	/// <para>To call this function for example, if you are warning about overriding a save it must be like this</para>
	/// <para>Warning("Save is not saved, Do you wish to continue", new string[] {"Yes", "No"}, new Action[] {{</para>
	/// <para>	() => { yourFunctionHere(); },</para>
	/// <para>	() => { functionObtionTwo(); }</para>
	/// <para>)</para>
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="options">Options.</param>
	/// <param name="a">The void function that is called when the correction option is chosen</param>
	public static void Warning (string message, string[] options, UnityEngine.Events.UnityAction[] a) {
		main.ToggleScreen ("Confirmation Screen");
		var cScreen = UIScreen.screenList.Find (g => g.getScreenName () == "Confirmation Screen").getScreen ();
		cScreen.transform.FindChild ("Text").GetComponent<Text> ().text = message;

		Debug.Log (options.Length);
		Debug.Log (a.Length);

		if (options.Length == 2) {
			// Change the text
			cScreen.transform.FindChild ("Yes").GetComponentInChildren<Text> ().text = options [0];
			cScreen.transform.FindChild ("No").GetComponentInChildren<Text> ().text = options [1];

			// Change the listeners
			var y = cScreen.transform.FindChild ("Yes").GetComponentInChildren<Button> ().onClick;
			var n = cScreen.transform.FindChild ("No").GetComponentInChildren<Button> ().onClick;
			y.RemoveAllListeners ();
			y.AddListener (() => { main.ToggleScreen ("Confirmation Screen"); });
			y.AddListener (a [0]);

			n.RemoveAllListeners ();
			n.AddListener (() => { main.ToggleScreen ("Confirmation Screen");});
			n.AddListener (a [1]);

		} else {
			Debug.LogError ("Parameter List != 2, length: " + options.Length);
			throw new Exception ("Hello World");
		}

	}

	/// <summary>
	/// Warning the specified message and options with no options to return
	/// </summary>
	/// <param name="message">The warning message</param>
	/// <param name="options">Options</param>
	public static void Warning (string message, string [] options)
	{
		//TODO Complete the warning label
	}

	private void OpenWarningPanel ()
	{
		
	}

	#endregion
}