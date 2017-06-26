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
public class Game : MonoBehaviour {
	public static Game main = null;
	public static string dataPath;

	public Saves saves;
	public GameInput input = new GameInput();
	public UIScreens UI = new UIScreens();
	public GameClock clock = new GameClock();

	void Awake()
    {
        if (main == null) main = this;
        else if (main != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

		UI.LoadFonts();
		dataPath = Application.dataPath;
		saves = new Saves();
		InvokeRepeating("PeriodicUpdate", 0.0f, 5.0f);
    }

    void Update()
    {
       	input.GetInput();
    }

	void PeriodicUpdate()
	{
		clock.PeriodicUpdate();
	}

	void OnApplicationQuit() {}

	#region Pause, Resume, Quit

	public bool gamePaused = false;

    public void Pause() {
        gamePaused = true;
    }

    public void Resume()
    {
        gamePaused = false;
    }
    
	public void Quit () {
		if (!gamePaused)
			Debug.LogError ("Game must be paused before you quit game");
		else {
            string message = "Warning, Current Save being deleted, do you wish to continue";
            WarningDialogue.Button yes = new WarningDialogue.Button("Yes", main.OnApplicationQuit);
            WarningDialogue.Button no = new WarningDialogue.Button("Yes", () => { });

            WarningDialogue.Warning(message, new List<WarningDialogue.Button>() { yes, no });
		}
	}

	#endregion
}