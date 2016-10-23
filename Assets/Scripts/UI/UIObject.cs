using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Extensions;

// This class manages everything UI related
namespace UI
{
    public abstract class UIObject
    {
        private static GameObject UIScreen
        {
            get
            {
                return GameObject.Find("UI Screen") ?? (GameObject)Object.Instantiate(Resources.Load("UI/UI Screen"));
            }
        }

        // Needs to be inherited by the upper objects
        public abstract string name { get; }
        
        protected GameObject gameObject
        {
            get
            {
                return UIScreen.GetGameObjectWithinChildren(name);
            }
            set
            {
                value.transform.parent = UIScreen.transform;
            }
        }

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
}