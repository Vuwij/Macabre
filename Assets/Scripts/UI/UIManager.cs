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

        public static UIObject[] uiObjects
        {
            get { return UIFolder.GetComponentsInChildren<UIObject>(); }
        }

        // Three Find functions
        public static T Find<T>(string name)
            where T : UIObject
        {
            T[] tArray = UIFolder.GetComponentsInChildren<T>();
            List<T> tList = new List<T>(tArray);

            foreach (T t in tList)
                if (t.name == name)
                    return t;
            throw new UnityException("Cannot Find UI Object of type " + name);
        }

        public static T Find<T>()
            where T : UIGameObject
        {
            return UIFolder.GetComponentInChildren<T>();
        }

        public static UIObject Find(string name)
        {
            UIObject[] objs = UIFolder.GetComponentsInChildren<UIObject>();
            foreach (UIObject obj in objs)
                if (obj.name == name)
                    return obj;
            throw new UnityException("Cannot Find UI Object of type " + name);
        }

        // The UI stack
        public static Stack<UIObject> currentPanelStack = new Stack<UIObject>();
        public static UIObject CurrentPanel
        {
            get
            {
                if (currentPanelStack.Count == 0) return null;
                return currentPanelStack.Peek();
            }
        }
    }
}