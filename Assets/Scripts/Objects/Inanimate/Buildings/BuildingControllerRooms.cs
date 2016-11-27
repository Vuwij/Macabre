using System.Collections.Generic;
using System.Linq;
using Objects.Inanimate.Buildings.Components;

namespace Objects.Inanimate.Buildings
{
    public abstract partial class BuildingController : InanimateObjectController
    {
        public virtual List<VirtualRoom> Rooms
        {
            get
            {
                return gameObject.GetComponentsInChildren<VirtualRoom>().ToList();
            }
        }
    }
}
