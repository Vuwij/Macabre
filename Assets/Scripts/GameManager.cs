using UnityEngine;
using System.Collections.Generic;
using UI;
using Data;
using Objects;
using UI.Panels;
using UI.Dialogues;

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

    // TODO : sometimes pausing doesn't show the pause screen, sometimes the inventory screen
    public static void PauseGame() {
        gamePaused = true;
        UIPanel panel = UIManager.Find<UIPanel>("Pause Panel");
        panel.TurnOn();
    }

    public static void ResumeGame()
    {
        gamePaused = false;
        UIPanel panel = UIManager.Find<UIPanel>("Pause Panel");
        panel.TurnOff();
    }
    
	public void QuitGame () {
		if (!gamePaused)
			Debug.LogError ("Game must be paused before you quit game");
		else {
            string message = "Warning, Current Save being deleted, do you wish to continue";
            WarningDialogue.Button yes = new WarningDialogue.Button("Yes", OnApplicationQuit);
            WarningDialogue.Button no = new WarningDialogue.Button("Yes", () => { });

            WarningDialogue.Warning(message, new List<WarningDialogue.Button>() { yes, no });
		}
	}

    public void OnApplicationQuit()
    {
        SaveManager.OnApplicationQuit();
    }
}