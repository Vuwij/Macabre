using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using Data;
using Extensions.Buttons;
using System;
using UI.Dialogues;

namespace UI.Panels
{
    public sealed class PausePanel : UIPanel
    {
        public void Resume()
        {
            //TurnOff();
        }

        public void Load()
        {
            //SavePanel savePanel = GameManager.main.UI.Find<SavePanel>();
            //savePanel.TurnOn();
        }
        public void Options()
        {
			//OptionsPanel optionsPanel = GameManager.main.UI.Find<OptionsPanel>();
            //optionsPanel.TurnOn();
        }
        public void GiveUp()
        {
     //       WarningDialogue.Warning("Are you sure you wish to give up?",
     //           new List<WarningDialogue.Button>()
     //           {
     //               new WarningDialogue.Button("It is never too late", () => { TurnOff(); }),
					//new WarningDialogue.Button("Let me die already", () => { GameManager.main.Quit(); }),
                //});
        }

        protected override void OnEnable()
        {
            base.OnEnable();
			GameManager.main.Pause();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
			GameManager.main.Resume();
        }
    }
}
