using System.Xml.Serialization;
using Objects.Items.Inventory;

namespace Objects.Movable.Characters
{
    // The model class for character, used by character controller
    public class Character : MovingObject
    {
        // General
        public string name;
        public string location // TODO Get location of character automatically
        {
            get
            {
                return "";
            }
        }

        // Animation
        // Conversation
        // Inspection
        // Inventory
        public Inventory inventory = new Inventory();

        // Movement
        public bool lockMovement = false;
        
    }
}
