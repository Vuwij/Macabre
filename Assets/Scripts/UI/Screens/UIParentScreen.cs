using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace UI.Screens {

    public sealed class UIParentScreen : UIScreen {
        public static UIParentScreen main = null;

        void Awake() {
            if (main == null) main = this;
            else if (main != this) Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }
    }
}