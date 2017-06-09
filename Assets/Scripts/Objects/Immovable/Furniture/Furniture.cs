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
		protected int OrientationX {
			get { 
				if(orientation == Orientation.NE || orientation == Orientation.SE) return 1;
				else return -1;
			}
		}
		protected int OrientationY {
			get { 
				if(orientation == Orientation.NE || orientation == Orientation.NW) return 1;
				else return -1;
			}
		}

		public enum Orientation { NE, NW, SE, SW };
		public Orientation orientation;

		protected override void Start() {
			base.Start();
		}
    }
}
