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
        
        public void TurnOn()
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.ignoreParentGroups = true;
        }

        public void TurnOff()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.ignoreParentGroups = true;
        }   
    }

    /// <summary>
    /// Interface that forces the UI Object to exist in game
    /// </summary>
    public interface UIGameObject { }
}