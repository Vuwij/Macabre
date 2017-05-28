using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using Data;
using Extensions.Buttons;
using System;
using UI.Dialogues;

namespace UI.Panels
{
    public sealed class PausePanel : UIPanel, UIGameObject
    {
        public override string name
        {
            get { return "Pause Panel"; }
        }

        public void Resume()
        {
            TurnOff();
        }

        public void Load()
        {
            SavePanel savePanel = Game.main.UI.Find<SavePanel>();
            savePanel.TurnOn();
        }
        public void Options()
        {
			OptionsPanel optionsPanel = Game.main.UI.Find<OptionsPanel>();
            optionsPanel.TurnOn();
        }
        public void GiveUp()
        {
            WarningDialogue.Warning("Are you sure you wish to give up?",
                new List<WarningDialogue.Button>()
                {
                    new WarningDialogue.Button("It is never too late", () => { TurnOff(); }),
					new WarningDialogue.Button("Let me die already", () => { Game.main.Quit(); }),
                });
        }

        public override void TurnOn()
        {
            base.TurnOn();
			Game.main.Pause();
        }

        public override void TurnOff()
        {
            base.TurnOff();
			Game.main.Resume();
        }
    }
}
