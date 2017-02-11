using System;
using UnityEngine;
using System.Collections;
using Exceptions;
using Objects.Movable.Characters;
using UI;
using UI.Screens;
using System.Linq;

namespace Objects.Inanimate.Buildings.Components.Path
{
    public partial class VirtualPathController : InanimateObjectController, IInspectable
    {
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
    }
}