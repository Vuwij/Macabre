using UnityEngine;
using UnityEngine.UI;

// This class manages everything UI related
namespace UI
{
    public abstract class UIObject
    {
        // Needs to be inherited by the upper objects
        public abstract string name { get; }
        protected CanvasGroup canvasGroup {
            get { return GameObject.Find(name).GetComponent<CanvasGroup>(); }
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
}