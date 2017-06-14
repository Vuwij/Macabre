using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Extensions;

// This class manages everything UI related
namespace UI
{
    public abstract class UIObject : MonoBehaviour
    {
        private static GameObject UIScreen
        {
            get
            {
                return GameObject.Find("UI Screen");
            }
        }
		protected CanvasGroup canvasGroup {
            get { return gameObject.GetComponent<CanvasGroup>(); }
        }
        private Stack<UIObject> currentPanelStack
        {
			get { return Game.main.UI.currentPanelStack; }
        }

		public bool stackable = true;

        public virtual void TurnOn()
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.ignoreParentGroups = true;
			if(stackable)
            	currentPanelStack.Push(this);
        }

        public virtual void TurnOff()
        {
			if (Game.main.UI.CurrentPanel != null && Game.main.UI.CurrentPanel != this) throw new UnityException("Current Panel " + name + " is not the top on stack");
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.ignoreParentGroups = true;
			if(stackable)
            	if(currentPanelStack.Count != 0) currentPanelStack.Pop();
        }
    }

    /// <summary>
    /// Interface that forces the UI Object to exist in game
    /// </summary>
    public interface UIGameObject { }
}