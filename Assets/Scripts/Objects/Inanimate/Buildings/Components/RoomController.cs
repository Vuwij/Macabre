using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exceptions;

namespace Objects.Inanimate.Buildings.Components
{
    public partial class RoomController : InanimateObjectController
    {
        protected override MacabreObject model
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public BuildingController buildingController
        {
            get
            {
                var room = GetComponentInParent<BuildingController>();
                if (room == null) throw new MacabreException("Room not specified for the furniture: " + name);
                return room;
            }
        }
    }
}
