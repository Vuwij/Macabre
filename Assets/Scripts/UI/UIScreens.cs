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
    public class UIScreens
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

		public Stack<UIObject> currentPanelStack = new Stack<UIObject>();

        public T Find<T>()
            where T : UIGameObject
        {
            return UIFolder.GetComponentInChildren<T>();
        }   
    }
}