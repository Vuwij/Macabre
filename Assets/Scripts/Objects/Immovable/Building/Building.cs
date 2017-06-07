using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Objects.Immovable.Furniture;
using Objects.Immovable.Path;
using Objects.Immovable.Rooms;

namespace Objects.Immovable.Buildings
{
    public abstract partial class Building : ImmovableObject
    {
		public List<AbstractFurniture> furniture = new List<AbstractFurniture>();
		public List<VirtualPath> path = new List<VirtualPath>();

		#region Rooms

		public virtual List<Room> Rooms
		{
			get
			{
				return gameObject.GetComponentsInChildren<Room>().ToList();
			}
		}

		#endregion
    }
}
