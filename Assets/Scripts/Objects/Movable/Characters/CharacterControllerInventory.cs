using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Objects.Inanimate.Items;
using Objects.Inanimate.Items.Inventory.Individual;

namespace Objects.Movable.Characters
{
    public abstract partial class CharacterController : MovingObjectController
    {
        public CharacterInventory inventory
        {
            get { return character.inventory; }
        }

        // TODO Create inventory adding objects
        public void AddToInventory(Item i)
        {
            inventory.Add(i);
        }
    }
}