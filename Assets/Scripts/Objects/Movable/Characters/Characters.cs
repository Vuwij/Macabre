using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;
using System;
using System.Collections;
using Objects.Movable.Characters.Individuals;

namespace Objects.Movable.Characters
{
    public class Characters
    {
        // The model of the characters
        public Dictionary<string, Character> characters = new Dictionary<string, Character>();

        [XmlIgnore]
        public List<CharacterController> characterControllers;
        public CharacterController player
        {
            get { return characterControllers.Find(x => x is PlayerController); }
        }

        public Characters()
        {
            foreach(KeyValuePair<string, Character> c in characters) {
                // Load the resources first
                var charObject = Resources.Load("Objects/Movable/Characters/" + c.Key) as GameObject;
                
                // Instantiate the character
                UnityEngine.Object.Instantiate(charObject);

                // Add it to the list of character controllers
                if (charObject.GetComponent<CharacterController>() == null) throw new UnityException("Character doesn't have controller attached");
                characterControllers.Add(charObject.GetComponent<CharacterController>());
            }
        }
    }
}
