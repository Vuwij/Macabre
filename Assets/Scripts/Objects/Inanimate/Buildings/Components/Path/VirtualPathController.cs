using System;
using UnityEngine;
using System.Collections;
using Exceptions;

namespace Objects.Inanimate.Buildings.Components.Path
{
    public partial class VirtualPathController : InanimateObjectController
    {
        protected override MacabreObject model
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public RoomController room
        {
            get
            {
                var room = GetComponentInParent<RoomController>();
                if (room == null) throw new MacabreException("Room not specified for the furniture: " + name);
                return room;
            }
        }
    }
}