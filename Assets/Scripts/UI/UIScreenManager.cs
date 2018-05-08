using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UI.Screens;

// This class manages everything UI related
public class UIScreenManager : MonoBehaviour
{
    public UIObject CurrentPanel
    {
        get
        {
            if (panelStack.Count == 0) return null;
            return panelStack.Peek();
        }
    }

    // Stack of the current panels
    public Stack<UIObject> panelStack = new Stack<UIObject>();
}