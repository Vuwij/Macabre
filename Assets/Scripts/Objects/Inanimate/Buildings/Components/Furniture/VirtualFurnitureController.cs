using System;
using System.Collections.Generic;
using System.Linq;
using Exceptions;

namespace Objects.Inanimate.Buildings.Components.Furniture
{
    public partial class VirtualFurnitureController : InanimateObjectController
    {
        public RoomController room
        {
            get
            {
                var room = GetComponentInParent<RoomController>();
                if (room == null) throw new MacabreException("Room not specified for the furniture: " + name);
                return room;
            }
        }
 
        protected override MacabreObject model
        {
            get { throw new NotImplementedException(); }
        }
    }
}
