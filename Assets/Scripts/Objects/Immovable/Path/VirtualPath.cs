using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using UI;
using UI.Screens;
using Objects.Immovable.Rooms;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Objects.Immovable.Path
{
	public class VirtualPath : ImmovableObject, IInspectable
    {
		public Vector2 newPosition
        {
            get { return (Vector2) transform.position + newPosition; }
        }
		public Room room
        {
            get
            {
                var room = GetComponentInParent<Room>();
				if (room == null) throw new Exception("Room not specified for the furniture: " + name);
                return room;
            }
        }

		public Room destination;
		public Vector2 offset;

		#region IInspectable implementation
		public void InspectionAction(Object controller, RaycastHit2D hit)
		{
			if (!(controller is Movable.Characters.Character)) return;
			var characterController = controller as Movable.Characters.Character;

			Game.main.UI.Find<DarkScreen>().TurnOn();

			// Change the active scene
			destination.gameObject.SetActive(true);
			room.gameObject.SetActive(false);

			// Find the closest door and move to the closest door
			var destinationDoor = destination.paths.OrderBy(x => Vector2.Distance(x.transform.position, transform.position));
			characterController.transform.position = (Vector2) destinationDoor.First().transform.position + destinationDoor.First().offset;

			Game.main.UI.Find<DarkScreen>().TurnOff();
		}
		#endregion

		protected override void Start() {
			base.Start();
		}

		#if UNITY_EDITOR
		void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawIcon(transform.position + (Vector3)offset, "Light Gizmo.tiff", true);
		}
		#endif
    }
}