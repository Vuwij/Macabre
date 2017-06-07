using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Objects.Immovable.Rooms;

namespace Objects.Immovable.Furniture
{
	public abstract class AbstractFurniture : ImmovableObject
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

		protected override void Start() {
			base.Start();
		}
    }
}
