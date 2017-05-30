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
    public class GameUI
    {
        public GameObject UIFolder
        {
            get { return GameObject.Find("UI"); }
        }
		public bool FadeBackground
		{
			set
			{
				if (value == true)
					Find<DarkScreen>().TurnOn();
				else
					Find<DarkScreen>().TurnOff();
			}
		}
		public UIObject CurrentPanel
		{
			get
			{
				if (currentPanelStack.Count == 0) return null;
				return currentPanelStack.Peek();
			}
		}
		public UIDialogue[] dialogues
        {
            get { return UIFolder.GetComponentsInChildren<UIDialogue>(); }
        }
		public UIPanel[] panels
        {
            get { return UIFolder.GetComponentsInChildren<UIPanel>(); }
        }
		public UIScreen[] screens
        {
            get { return UIFolder.GetComponentsInChildren<UIScreen>(); }
        }
		public UIObject[] uiObjects
        {
            get { return UIFolder.GetComponentsInChildren<UIObject>(); }
        }

        public T Find<T>()
            where T : UIGameObject
        {
            return UIFolder.GetComponentInChildren<T>();
        }

        public Stack<UIObject> currentPanelStack = new Stack<UIObject>();
        
    }
}