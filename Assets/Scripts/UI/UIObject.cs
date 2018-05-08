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

		public bool stackable = true;

        protected virtual void Start() {}

        protected virtual void OnEnable()
        {
            CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.ignoreParentGroups = true;

            UIScreenManager screenManager = transform.parent.GetComponent<UIScreenManager>();
            Debug.Assert(screenManager != null);
            screenManager.panelStack.Push(this);
        }

        protected virtual void OnDisable()
        {
            CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();

            UIScreenManager screenManager = transform.parent.GetComponent<UIScreenManager>();
            Debug.Assert(screenManager != null);
            Debug.Assert(screenManager.panelStack.Peek() == this);
            screenManager.panelStack.Pop();
        }
    }
}