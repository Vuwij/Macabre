using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Objects.Inanimate {

    public abstract partial class InamimateObjectController : MonoBehaviour
    {
        protected List<MacabreObject> itemsToShowWhenEntered = new List<MacabreObject>();
        protected List<MacabreObject> itemsToHideWhenEntered = new List<MacabreObject>();
        
        // TODO, set objects to hide and show when you enter the object

    }
}