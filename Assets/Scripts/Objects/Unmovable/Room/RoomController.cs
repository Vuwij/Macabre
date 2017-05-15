using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exceptions;
using Objects.Unmovable.Building;
using Objects.Unmovable.Furniture;
using Objects.Unmovable.Path;

namespace Objects.Unmovable
{
    public class RoomController : UnmovableObjectController
    {
        protected override MacabreObject model
        {
            get
            {
                throw new NotImplementedException();
            }
        }

		// The Parent Building Controller
        public BuildingController buildingController
        {
            get
            {
                var room = GetComponentInParent<BuildingController>();
                if (room == null) throw new MacabreException("Room not specified for the furniture: " + name);
                return room;
            }
        }

		#region Paths

		public VirtualPathController[] paths
		{
			get { return GetComponentsInChildren<VirtualPathController>(); }
		}

		#endregion

		#region Collision

		public override void CreateProximityCircle() { }    // Do not create a proximity circle for the room
		public override void CreateCollisionCircle() { }    // Do not create a collision circle for the room, already created
		protected override void SetupBackEdgeCollider() { } //

		#endregion

		#region Furniture

		public List<AbstractFurniture> Furniture
		{
			get
			{
				return gameObject.GetComponentsInChildren<AbstractFurniture>().ToList();
			}
		}

		#endregion
    }
}
