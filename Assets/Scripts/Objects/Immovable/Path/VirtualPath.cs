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

			// Turn off rooms first
			foreach(var sharedRoom in room.sharedRooms) {
				sharedRoom.gameObject.SetActive(false);
			}
			foreach (var door in room.gameObject.GetComponentsInChildren<Door>(true)) {
				door.enabled = false;
			}
			room.gameObject.SetActive(false);

			// Destination Room
			foreach(var sharedRoom in destination.sharedRooms) {
				foreach(SpriteRenderer sr in sharedRoom.gameObject.GetComponentsInChildren<SpriteRenderer>()) {
					sr.sortingLayerName = "Background";
				}
				sharedRoom.gameObject.SetActive(true);
			}
			foreach (var door in destination.GetComponentsInChildren<Door>(true)) {
				door.enabled = true;
			}
			destination.gameObject.SetActive(true);
			foreach (SpriteRenderer sr in destination.gameObject.GetComponentsInChildren<SpriteRenderer>()) {
				sr.sortingLayerName = "World";
			}
			destination.GetComponent<SpriteRenderer>().sortingLayerName = "Background";

			// Find the closest door and move to the closest door
			var destinationDoor = destination.paths.OrderBy(x => Vector2.Distance(x.transform.position, transform.position));
			characterController.transform.position = (Vector2) destinationDoor.First().transform.position + destinationDoor.First().offset;
			characterController.UpdateSortingLayer();

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