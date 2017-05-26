using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Dialogues
{
    public sealed class WarningDialogue : UIDialogue, UIGameObject
    {
        public override string name
        {
            get { return "WarningDialogue"; }
        }

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
            WarningDialogue warning = Game.main.UI.Find<WarningDialogue>();
        }
    }
}
