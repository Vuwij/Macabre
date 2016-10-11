using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Objects.Items.Inventory;

namespace Objects.Movable.Characters
{
    public abstract partial class CharacterController : MovingObjectController
    {
        private const int classALimit = 6, classBLimit = 2;
        public Inventory inventory = new Inventory(classALimit, classBLimit);

        // TODO Create inventory adding objects
        public bool AddToInventory()
        {
            return false;
        }
    }
}