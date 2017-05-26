using UnityEngine;
using System.Collections;
using UI;
using UI.Panels;
using UI.Screens;
using Objects.Movable;
using Objects.Movable.Characters.Individuals;

public class GameInput {
	Player player
	{
		get { return Objects.Movable.Characters.Character.player; }
	}
	GameUI UI {
		get { return Game.main.UI; }
	}

	public bool inspectionLocked = false;

	public void GetInput() {
        // Checks if the game is paused or else return
		if (Input.GetButtonDown("Pause")) EscapeButton();
        if (Game.main.gamePaused) return;

        // Key Maps for Inventory
        if (Input.GetButtonDown ("Inventory")) {
            if(!Game.main.UI.currentPanelStack.Contains(UI.Find<DarkScreen>()))
                UI.Find<InventoryPanel>().TurnOn();
            else
                UI.Find<InventoryPanel>().TurnOff();
		}

        // Key Maps for Inspection
		if (Input.GetButtonDown ("Inspect")) {
			if (!inspectionLocked) player.Inspect();
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
    }

    void EscapeButton()
    {
        bool hasUIPanel = false;
        foreach (var p in UI.currentPanelStack)
        {
            if (p is UI.Panels.UIPanel)
            {
                hasUIPanel = true;
                break;
            }
        }

        if(hasUIPanel)
            UI.CurrentPanel.TurnOff();
        else
            UI.Find<UI.Panels.PausePanel>().TurnOn();
    }
}
