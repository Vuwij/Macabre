using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Objects.Movable.Characters
{
    public class CharacterControllers
    {
        // TODO initialize this with the characters that already exist in the game
        [XmlIgnore]
        public List<CharacterController> controllers = new List<CharacterController>();

        public IEnumerable<Character> characters
        {
            get
            {
                foreach (CharacterController c in controllers)
                    yield return c.character;
            }
        }
        
    }
}
