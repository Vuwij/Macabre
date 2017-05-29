using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Objects.Unmovable.Furniture
{
    public abstract class AbstractFurniture : UnmovableObject
    {
        public Room room
        {
            get
            {
                var room = GetComponentInParent<Room>();
				if (room == null) throw new Exception("Room not specified for the furniture: " + name);
                return room;
            }
        }
    }
}
