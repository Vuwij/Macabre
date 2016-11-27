using System;
using UnityEngine;
using Objects.Movable.Characters;

namespace Objects.Inanimate.Items
{
    public partial class ItemController : IInspectable
    {
        // The character associated with the controller, found in the data structure
        public void InspectionAction(MacabreObjectController obj, RaycastHit2D hit)
        {
            PickUp(obj as Movable.Characters.CharacterController);
        }

        public void PickUp(Movable.Characters.CharacterController obj)
        {
            if (obj is Movable.Characters.CharacterController)
            {
                bool addedToInventory = (obj).AddToInventory(this);

                if (addedToInventory)
                {
                    gameObject.SetActive(false);
                    gameObject.transform.parent = (obj).InventoryFolder;
                }
            }
        }
        
        public static Vector2 DropCircle
        {
            get {
                // TODO: Make sure that this drop point doesn't collide with objects/glitch and create drop animation
                var randomCircle = (Vector2) UnityEngine.Random.onUnitSphere;
                randomCircle.Scale(new Vector2(GameSettings.dropDistance, GameSettings.dropDistance));
                return randomCircle;
            }
        }

        // Called from CharacterInventory.Drop()
        public void Drop()
        {
            gameObject.SetActive(true);

            var inventoryFolder = gameObject.transform.parent;

            // Set the location to the same as the gameobject with some randomness
            gameObject.transform.position = (Vector2) inventoryFolder.parent.position + DropCircle;

            // Move out of the inventory folder to the world
            gameObject.transform.parent = gameObject.transform.parent.parent.parent;
            
            // Destroy the inventory folder if
            if (inventoryFolder.GetComponentsInChildren<Transform>().Length == 0) Destroy(inventoryFolder.gameObject);
        }
    }
}
