using System;
using System.Collections.Generic;
using UnityEngine;

namespace Objects.Inanimate.Buildings.Components
{
    using Path;

    public partial class RoomController : InanimateObjectController
    {
        [SerializeField]
        public VirtualPathController[] paths;
        
    }
}
