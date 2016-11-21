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
    public static partial class UIManager
    {
        public static bool FadeBackground
        {
            set
            {
                if (value == true)
                    Find<DarkScreen>().TurnOn();
                else
                    Find<DarkScreen>().TurnOff();
            }
        }
    }
}