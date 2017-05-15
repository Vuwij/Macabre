using UnityEngine;
using System.Collections.Generic;
using UI;
using Data;
using Objects;
using Environment;
using UI.Panels;
using UI.Dialogues;

/// <summary>
/// The Game Manager is responsible for loading everything in the correct order. Individual files do not load themselves
/// </summary>
public partial class GameManager : MonoBehaviour {
	public static GameManager main = null;
    
    void Awake()
    {
        if (main == null) main = this;
        else if (main != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SaveManager.Initialize();
        TimeController.Setup();
    }
    
    void Update()
    {
        InputManager.Update();
    }

    public static bool gamePaused = false;

    public static void PauseGame() {
        gamePaused = true;
        TimeController.Stop();
    }

    public static void ResumeGame()
    {
        gamePaused = false;
        TimeController.Start();       
    }
    
	public static void QuitGame () {
		if (!gamePaused)
			Debug.LogError ("Game must be paused before you quit game");
		else {
            string message = "Warning, Current Save being deleted, do you wish to continue";
            WarningDialogue.Button yes = new WarningDialogue.Button("Yes", main.OnApplicationQuit);
            WarningDialogue.Button no = new WarningDialogue.Button("Yes", () => { });

            WarningDialogue.Warning(message, new List<WarningDialogue.Button>() { yes, no });
		}
	}

    private void OnApplicationQuit()
    {
        SaveManager.OnApplicationQuit();
        TimeController.Stop();
    }
}