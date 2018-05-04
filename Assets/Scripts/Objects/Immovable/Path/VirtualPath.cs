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
				if (room == null) Debug.LogWarning("Room not specified for the furniture: " + name);
                return room;
            }
        }

		public Room destination;
		public Vector2 offset;

		#region IInspectable implementation
		public virtual void InspectionAction(Object controller, RaycastHit2D hit)
		{
			if (!(controller is Movable.Characters.Character)) return;
			var characterController = controller as Movable.Characters.Character;

			Game.main.UI.Find<DarkScreen>().TurnOn();

			room.gameObject.SetActive(false);
			destination.gameObject.SetActive(true);

			// Find the closest door and move to the closest door
			var destinationDoor = destination.paths.OrderBy(x => Vector2.Distance(x.transform.position, transform.position));
			characterController.transform.position = (Vector2) destinationDoor.First().transform.position + destinationDoor.First().offset;

			Game.main.UI.Find<DarkScreen>().TurnOff();
		}
		#endregion

		protected override void Start() {
			base.Start();
		}

		void OnDrawGizmos() {
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere((Vector2) transform.position + offset, 1);
		}
    }
}