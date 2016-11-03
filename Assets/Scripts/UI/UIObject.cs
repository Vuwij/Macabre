using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Extensions;
using Exceptions;

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
        
        // Needs to be inherited by the upper objects
        public abstract new string name { get; }
        
        protected CanvasGroup canvasGroup {
            get { return gameObject.GetComponent<CanvasGroup>(); }
        }
        
        private Stack<UIObject> currentPanelStack
        {
            get { return UIManager.currentPanelStack; }
        }

        public void TurnOn()
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.ignoreParentGroups = true;
            currentPanelStack.Push(this);
        }

        public void TurnOff()
        {
            if (UIManager.CurrentPanel != null && UIManager.CurrentPanel != this) throw new MacabreUIException("Current Panel " + name + " is not the top on stack");
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.ignoreParentGroups = true;
            if(currentPanelStack.Count != 0) currentPanelStack.Pop();
        }
    }

    /// <summary>
    /// Interface that forces the UI Object to exist in game
    /// </summary>
    public interface UIGameObject { }
}