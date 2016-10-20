using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using UI.Panels;
using UI.Dialogues;

// This class manages everything UI related
namespace UI
{
    public static class UIManager
    {
        public static GameObject UIFolder
        {
            get { return GameObject.Find("UI"); }
        }
        
        // Dialogues pop up, cannot be toggled
        public static UIDialogue[] dialogues
        {
            get { return UIFolder.GetComponentsInChildren<UIDialogue>(); }
        }

        public static UIPanel[] panels
        {
            get { return UIFolder.GetComponentsInChildren<UIPanel>(); }
        }
        
    }
}