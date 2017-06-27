using UnityEngine;
using UnityEngine.UI;
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
		public Dictionary<string, Font> fonts = new Dictionary<string, Font>();

        public T Find<T>()
            where T : UIGameObject
        {
            return UIFolder.GetComponentInChildren<T>();
        }

		public void LoadFonts() {
			Font font = Resources.Load("Fonts/Munro", typeof(Font)) as Font;
			fonts.Add("Munro", font);
			font = Resources.Load("Fonts/Munro_small", typeof(Font)) as Font;
			fonts.Add("Munro_small", font);
		}
    }
}