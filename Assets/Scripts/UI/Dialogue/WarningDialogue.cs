using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Dialogues
{
    public class WarningDialogue : UIDialogue
    {
        public override string name
        {
            get { return "WarningDialogue"; }
        }

        public delegate void WarningAction();
        public class Button
        {
            string buttonName;
            event WarningAction buttonPressedAction;

            public Button(string buttonName, WarningAction action)
            {
                this.buttonName = buttonName;
                this.buttonPressedAction += action;
            }
        }
        
        public static void Open(string message, List<Button> warningButtons)
        {
            // TODO : Finish the warning dialogue information
        }
    }
}
