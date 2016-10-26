using System;
using System.Xml.Serialization;
using Objects.Inanimate.Items.Inventory.Individual;

namespace Objects.Movable.Characters
{
    // The model class for character, used by character controller
    public class Character : MovingObject
    {
        // General
        public string name;
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
