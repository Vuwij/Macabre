using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Objects.Unmovable.Furniture
{
    public abstract class AbstractFurniture : UnmovableObject
    {
        public RoomController room
        {
            get
            {
                var room = GetComponentInParent<RoomController>();
				if (room == null) throw new Exception("Room not specified for the furniture: " + name);
                return room;
            }
        }
    }
}
