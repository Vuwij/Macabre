using UnityEngine;
using System.Collections;

using Objects.Movable;
using Objects.Movable.Characters;

public static class InputManager {
	private static float inspectRadius
    {
        get { return GameSettings.inspectRadius; }
    }

	private static bool lockInspect = false;
    public static void LockInspection() { lockInspect = true; }
    public static void UnlockInspection() { lockInspect = false; }

    private static bool pendingDecision = false;
    public static void LockPendingDecision() { pendingDecision = true; }
    public static void UnlockPendingDecision() { pendingDecision = false; }
    
	private static Player player
    {
        get
        {
            return Character.player;
        }
        set
        {
            Character.player = value;
        }
    }

	public static void Update() {
		MouseInput();
		KeyboardInput();
	}
    
	static void KeyboardInput() {
        // Checks if the game is paused or else return
        if (Input.GetButtonDown ("Pause"))
            GameManager.PauseGame();
        if (GameManager.gamePaused) return;

        // Key Maps for Inventory
        if (Input.GetButtonDown ("Inventory")) {
			UIManager.ToggleDarkenScreen ();
			UIManager.ToggleScreen("Inventory");
		}

        // Key Maps for Inspection
		else if (Input.GetButtonDown ("Inspect")) {
			if (!lockInspect) player.Inspect();
        }

        // Key Maps for Conversation
		else if(Input.GetKeyDown(KeyCode.Alpha0)) {
			if (pendingDecision) Debug.LogWarning("0 is not a valid decision for conversations decision");
        }
		else if(Input.GetKeyDown(KeyCode.Alpha1)) {
			if (pendingDecision) player.DialogueDecision(1);
        }
		else if(Input.GetKeyDown(KeyCode.Alpha2)) {
			if (pendingDecision) player.DialogueDecision(2);
        }
		else if(Input.GetKeyDown(KeyCode.Alpha3)) {
			if (pendingDecision) player.DialogueDecision(3);
        }
		else if(Input.GetKeyDown(KeyCode.Alpha4)) {
			if (pendingDecision) player.DialogueDecision(4);
        }
	}

    static void MouseInput() { }
}
