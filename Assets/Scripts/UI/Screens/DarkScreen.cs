using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

using Data;
using System;

namespace UI.Screens
{
    public sealed class DarkScreen : UIScreen, UIGameObject
    {
        public override string name
        {
            get { return "Dark Screen"; }
        }
    }
}
