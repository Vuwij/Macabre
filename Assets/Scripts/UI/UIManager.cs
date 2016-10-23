using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using UI.Panels;
using UI.Dialogues;
using UI.Screens;

// This class manages everything UI related
namespace UI
{
    public static partial class UIManager
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

        public static UIScreen[] screens
        {
            get { return UIFolder.GetComponentsInChildren<UIScreen>(); }
        }

        public static T Find<T>(string name)
            where T : UIObject
        {
            T[] tArray = UIFolder.GetComponentsInChildren<T>();
            List<T> tList = new List<T>(tArray);

            foreach (T t in tList)
                if (t.name == name)
                    return t;
            throw new UnityException("Cannot Find UI Object of type" + name);
        }

    }
}