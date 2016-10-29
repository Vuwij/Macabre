using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace UI.Screens {

    public sealed class UIParentScreen : UIScreen {
        public static UIParentScreen main = null;

        public override string name
        {
            get { return "UIScreen"; }
        }

        void Awake() {
            if (main == null) main = this;
            else if (main != this) Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }
    }
}