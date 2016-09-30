using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class GameManager : Manager {
	public static GameManager main = null;

	private GameObject player;
	private Slider loadingSlider;

	#region Setup

	void Awake () {
		if (main == null) {
			main = this;
		} else if (main != this) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);
	}

	void Start() {
		// For testing Macabre
		if (Settings.EnableMacabreTest) {
			MacabreTest m = new MacabreTest ();
		}

		// Set up the game clock. Which is used by the save manager
		GameClock.main = new GameClock ();
	}

	#endregion

	#region Game Methods

	public void PauseGame() {
		Debug.Log ("Unpausing Screen");
		// If the game is already paused and other menus are open, go back to the previous menu
		if (!gamePaused) {
			UIManager.main.ToggleDarkenScreen();
			UIManager.main.ToggleScreen ("Pause Screen");
			gamePaused = true;
		}
		// Otherwise hide everything
		else {
			UIManager.main.HideAllScreens ();
			gamePaused = false;
		}
	}

	public void Options ()
	{

	}

	public void Quit () {
		if (!gamePaused)
			Debug.LogError ("Game must be paused before you quit game");
		else {
			UIManager.main.ToggleDarkenScreen ();

			string [] options = { "Yes", "No" };
			UIManager.Warning ("Warning, Current Save being deleted, do you wish to continue", options, new UnityEngine.Events.UnityAction [] {
				() => { QuitConfirmQuit(); },
				() => { }
			});
		}
	}

	public void QuitConfirmQuit () {
		Application.Quit ();
	}

	#endregion

	#region LoadingThings

	private void LoadComponent <T> ()
		where T: Manager {

		if (gameObject.GetComponent<T> () != null) {
			gameObject.AddComponent<T> ();
		}
	}

	private void LoadCamera() {
		LoadResource<Camera> ("Main Camera");
	}

	private void LoadResource <T> (string objectName)
		where T: Component {
		Resources.LoadAsync<T> (objectName);
	}

	private void LoadEverythingSingleCheck(string objectName) {
		var obj = GameObject.Find (objectName);
		if (obj == null) {
			Debug.LogWarning ("Warning: " + objectName + " not found or hidden, please find in in the prefab folder");
		}
	}

	#endregion

	#region Creating the Game

	[HideInInspector]
	public bool gamePaused = false;

	// Loads the game scene
	public void LoadGame() {
		
	}

	public void NewGame() {
//		UIManager.main.TurnOnScreen ("Loading Screen");
		UIManager.main.TurnOffScreen ("Start Screen");
		SceneManager.LoadScene ("Game", LoadSceneMode.Additive);
//		SceneManager.SetActiveScene (SceneManager.GetSceneByName ("Game"));
//		UIManager.main.TurnOnScreen ("Loading Screen");
//
//		// Set up new game data
//		MacabreSceneManager.NewGame ();
		SaveManager.NewGame ();
//
//		UIManager.main.FindGameObjects ();
//		UIManager.main.TurnOffScreen ("Loading Screen");
//		Debug.Log ("Game Loaded");
	}

	public void ResumeGame() {
		SceneManager.LoadScene ("Game", LoadSceneMode.Additive);
		SceneManager.SetActiveScene (SceneManager.GetSceneByName ("Game"));
		UIManager.main.TurnOnScreen ("Loading Screen");

		// Set up new game data
		MacabreSceneManager.NewGame ();
		SaveManager.NewGame ();

		UIManager.main.FindGameObjects ();
		UIManager.main.TurnOffScreen ("Start Screen");
		UIManager.main.TurnOffScreen ("Loading Screen");
		Debug.Log ("Game Loaded");
	}

	public void QuitGame () {
		gamePaused = true;
		Quit ();
	}

	public void OpenSettings() {

	}

	private bool managersLoaded = false;

	public void ReturnToMainMenu() {
	}

	#endregion

	#region SaveManager Shortcuts

	public void S_ToggleMenu(string s) {
		Debug.Log ("Hi");
		switch (s) {
		case "Save/Load":
			SaveManager.main.ToggleSaveMenu ();
			return;
		case "Load":
			SaveManager.main.LoadSavePauseScreen ();
			return;
		case "Rename":
			SaveManager.main.RenameSavePauseScreen ();
			return;
		case "Delete":
			SaveManager.main.DeleteSavePauseScreen ();
			return;
		case "Back":
			SaveManager.main.ToggleSaveMenu ();
			return;
		}
	}

	#endregion
}
