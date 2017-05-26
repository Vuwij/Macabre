using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Objects.Unmovable.Furniture;
using Objects.Unmovable.Path;

namespace Objects.Unmovable.Building
{
    public abstract partial class Building : UnmovableObject
    {
		public List<AbstractFurniture> furniture = new List<AbstractFurniture>();
		public List<VirtualPath> path = new List<VirtualPath>();

		#region Rooms

		public virtual List<RoomController> Rooms
		{
			get
			{
				return gameObject.GetComponentsInChildren<RoomController>().ToList();
			}
		}

		#endregion
    }
}
