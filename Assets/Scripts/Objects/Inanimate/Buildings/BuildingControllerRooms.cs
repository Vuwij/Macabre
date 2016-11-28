using System.Collections.Generic;
using System.Linq;
using Objects.Inanimate.Buildings.Components;

namespace Objects.Inanimate.Buildings
{
    public abstract partial class BuildingController : InanimateObjectController
    {
        public virtual List<RoomController> Rooms
        {
            get
            {
                return gameObject.GetComponentsInChildren<RoomController>().ToList();
            }
        }
    }
}
