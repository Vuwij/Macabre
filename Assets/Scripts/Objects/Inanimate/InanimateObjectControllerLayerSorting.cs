using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Objects.Inanimate
{
   public abstract partial class InanimateObjectController : MacabreObjectController
    {
        // TODO Automatically Reposition the object
        partial void OnTriggerStay2D(Collider2D other);

        //partial void OnTriggerExit2D(Collider2D other);
    }
}
