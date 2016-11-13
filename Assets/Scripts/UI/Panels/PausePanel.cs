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
            SavePanel savePanel = UIManager.Find<SavePanel>();
            savePanel.TurnOn();
        }
        public void Options()
        {
            OptionsPanel optionsPanel = UIManager.Find<OptionsPanel>();
            optionsPanel.TurnOn();
        }
        public void GiveUp()
        {
            WarningDialogue.Warning("Are you sure you wish to give up?",
                new List<WarningDialogue.Button>()
                {
                    new WarningDialogue.Button("It is never too late", () => { TurnOff(); }),
                    new WarningDialogue.Button("Let me die already", () => { GameManager.QuitGame(); }),
                });
        }

        public override void TurnOn()
        {
            base.TurnOn();
            GameManager.PauseGame();
        }

        public override void TurnOff()
        {
            base.TurnOff();
            GameManager.ResumeGame();
        }
    }
}
