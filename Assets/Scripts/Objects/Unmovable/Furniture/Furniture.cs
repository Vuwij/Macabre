using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Exceptions;

namespace Objects.Unmovable.Furniture
{
    public abstract class AbstractFurniture : UnmovableObjectController
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
