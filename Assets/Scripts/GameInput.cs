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
		get { 
			var p = GameObject.Find("Player");
			if(p == null) return null;
			return p.GetComponent<Player>();
		}
	}
	GameUI UI {
		get { return Game.main.UI; }
	}

	public void GetInput() {
        // Checks if the game is paused or else return
		if (Input.GetButtonDown("Pause")) EscapeButton();
        if (Game.main.gamePaused) return;

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
