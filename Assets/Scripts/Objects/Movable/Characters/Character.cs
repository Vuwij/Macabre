using System;
using UnityEngine;
using System.Runtime.Serialization;
using Objects.Inventory.Individual;

namespace Objects.Movable.Characters
{
    // The model class for character, used by character controller
    [DataContract]
    public class Character : MovingObject
    {
        // General
        public string name = "Unnamed";
        public string location // TODO Get location of character automatically
        {
            get { throw new NotImplementedException(); }
        }

        // Animation
        // Conversation
        // Inspection
        // Inventory
        public CharacterInventory inventory = new CharacterInventory();

        // Movement
        public bool lockMovement = false;

    }
}
