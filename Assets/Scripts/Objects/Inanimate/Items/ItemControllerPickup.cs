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
            if(obj is Movable.Characters.CharacterController)
            {
                bool addedToInventory = (obj as Movable.Characters.CharacterController).AddToInventory(this);

                if (addedToInventory) gameObject.SetActive(false);
            }
        }
    }
}
