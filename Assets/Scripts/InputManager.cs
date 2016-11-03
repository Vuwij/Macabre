using UnityEngine;
using System.Collections;

using UI;
using UI.Screens;
using Objects.Movable;
using Objects.Movable.Characters.Individuals;


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
    
	private static PlayerController player
    {
        get { return Objects.Movable.Characters.CharacterController.playerController; }
    }

	public static void Update() {
		MouseInput();
		KeyboardInput();
	}
    
	static void KeyboardInput() {
        // Checks if the game is paused or else return
        if (Input.GetButtonDown("Pause")) EscapeButtonPressed();
        if (GameManager.gamePaused) return;

        // Key Maps for Inventory
        if (Input.GetButtonDown ("Inventory")) {
			UIManager.FadeBackground = true;
			UIManager.Find<UIScreen>("Inventory").TurnOn();
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
			if (pendingDecision) player.Dialogue(1);
        }
		else if(Input.GetKeyDown(KeyCode.Alpha2)) {
			if (pendingDecision) player.Dialogue(2);
        }
		else if(Input.GetKeyDown(KeyCode.Alpha3)) {
			if (pendingDecision) player.Dialogue(3);
        }
		else if(Input.GetKeyDown(KeyCode.Alpha4)) {
			if (pendingDecision) player.Dialogue(4);
        }
	}

    static void MouseInput() { }

    static void EscapeButtonPressed()
    {
        if (UIManager.CurrentPanel == null) return;
        if (UIManager.CurrentPanel is UI.Panels.PausePanel)
        {
            if (GameManager.gamePaused) GameManager.ResumeGame();
            else GameManager.PauseGame();
        }
        bool hasUIPanel = false;
        foreach (var p in UIManager.currentPanelStack)
        {
            if (p is UI.Panels.UIPanel)
            {
                hasUIPanel = true;
                break;
            }
        }
        if(hasUIPanel)
            UIManager.CurrentPanel.TurnOff();
        else
            UIManager.Find<UI.Panels.PausePanel>().TurnOn();
    }
}
