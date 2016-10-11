using UnityEngine;
using UnityEngine.SceneManagement;
using Environment;
using Environment.Time;
using Environment.Audio;
using UI;
using Data;

/// <summary>
/// The Game Manager is responsible for loading everything in the correct order. Individual files do not load themselves
/// </summary>
public partial class GameManager : MonoBehaviour {
	public static GameManager main = null;

    public static bool gamePaused = false;

    void Awake()
    {
        if (main == null) main = this;
        else if (main != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SaveManager.Initialize();
    }
    
    void Update()
    {
        InputManager.Update();
    }

    public static void PauseGame() {
        if (!gamePaused) {
			UIManager.ToggleDarkenScreen();
			UIManager.ToggleScreen ("Pause Screen");
			gamePaused = true;
		} else {
			UIManager.HideAllScreens ();
			gamePaused = false;
		}
	}
    
	public void QuitGame () {
		if (!gamePaused)
			Debug.LogError ("Game must be paused before you quit game");
		else {
			UIManager.ToggleDarkenScreen ();

			string [] options = { "Yes", "No" };
			WarningPanel.Warning("Warning, Current Save being deleted, do you wish to continue", options, new UnityEngine.Events.UnityAction [] {
				() => { Application.Quit (); },
				() => { }
			});
		}
	}

    public void OnApplicationQuit()
    {
        SaveManager.OnApplicationQuit();
    }

}