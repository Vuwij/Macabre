using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Objects.Inanimate.Buildings.Components
{
    using Furniture;

    public partial class RoomController : InanimateObjectController
    {
        public List<VirtualFurnitureController> Furniture
        {
            get
            {
                return gameObject.GetComponentsInChildren<VirtualFurnitureController>().ToList();
            }
        }
    }
}
