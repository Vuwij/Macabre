using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using Exceptions;
using UI;
using UI.Screens;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Objects.Unmovable.Path
{
    public class VirtualPathController : UnmovableObjectController, IOffsetable
    {
        protected override MacabreObject model
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [SerializeField]
        public RoomController destination;

        // Find the closest offset to enter into
        public Vector2 offset;
        public Vector2 newPosition
        {
            get { return (Vector2) transform.position + newPosition; }
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

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawIcon(transform.position + (Vector3)offset, "Light Gizmo.tiff", true);
        }
#endif

		#region Collision

		protected override PolygonCollider2D proximityBox
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		protected override void SetupBackEdgeCollider()
		{
			// No back edge collider paths
		}

		public override void CreateProximityCircle()
		{
			// Don't create a proximity circle
		}

		#endregion

		#region Inspection

		// Find the room's closest door
		public void InspectionAction(MacabreObjectController controller, RaycastHit2D hit)
		{
			if (!(controller is Movable.Characters.CharacterController)) return;
			var characterController = controller as Movable.Characters.CharacterController;

			UIManager.Find<DarkScreen>().TurnOn();

			// Change the active scene
			destination.gameObject.SetActive(true);
			room.gameObject.SetActive(false);

			// Find the closest door and move to the closest door
			var destinationDoor = destination.paths.OrderBy(x => Vector2.Distance(x.transform.position, transform.position));
			characterController.transform.position = (Vector2) destinationDoor.First().transform.position + destinationDoor.First().offset;

			UIManager.Find<DarkScreen>().TurnOff();
		}
		#endregion
    }
}