using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

using Data;
using System;

namespace UI.Screens
{
    public sealed class LoadingScreen : UIScreen, UIGameObject
    {
        public override string name
        {
            get { return "Loading Screen"; }
        }
    }
}
