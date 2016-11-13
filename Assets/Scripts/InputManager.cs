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
		if (Input.GetButtonDown ("Inspect")) {
			if (!lockInspect) player.Inspect();
        }

        // Key Maps for Conversation
        if (Input.GetKeyDown(KeyCode.Alpha1))
            player.KeyPressed(1);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            player.KeyPressed(2);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            player.KeyPressed(3);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            player.KeyPressed(4);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            player.KeyPressed(5);
        if (Input.GetKeyDown(KeyCode.Alpha6))
            player.KeyPressed(6);
        if (Input.GetKeyDown(KeyCode.Alpha7))
            player.KeyPressed(7);
        if (Input.GetKeyDown(KeyCode.Alpha8))
            player.KeyPressed(8);
        if (Input.GetKeyDown(KeyCode.Alpha9))
            player.KeyPressed(9);
        if (Input.GetKeyDown(KeyCode.Alpha0))
            player.KeyPressed(0);
    }

    static void MouseInput() { }

    static void EscapeButtonPressed()
    {
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
