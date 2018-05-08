using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Dialogues
{
    public sealed class WarningDialogue : UIDialogue
    {
		public delegate void WarningAction();
        public class Button
        {
            public string buttonName;
            event WarningAction buttonPressedAction;

            public Button(string buttonName, WarningAction action)
            {
                this.buttonName = buttonName;
                this.buttonPressedAction += action;
            }
        }
        
        public static void Warning(string message, List<Button> warningButtons)
        {
            //WarningDialogue warning = GameManager.main.UI.Find<WarningDialogue>();
        }
    }
}
