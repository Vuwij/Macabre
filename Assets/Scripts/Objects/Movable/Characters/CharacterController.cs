using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Objects.Movable.Characters.Individuals;

namespace Objects.Movable.Characters
{
    public abstract partial class CharacterController : MovingObjectController
    {
        // The character associated with the controller, found in the data structure
        public Character character
        {
            get
            {
                return Characters.CharacterDictionary[name];
            }
        }
        
        // This is the object for the character controller
        protected override MacabreObject model {
            get
            {
                return character;
            }
        }

        // A simple reference to the player for interaction in conversation
        public static PlayerController playerController
        {
            get {
                return Characters.playerController;
            }
        }

        // The child object is the one that contains the sprite
        private GameObject childObject
        {
            get { return gameObject.transform.FindChild(gameObject.name + "Sprite").gameObject; }
        }
        
        // What is called when the character gets loaded
        protected override void Start()
        {
            base.Start();
        }
    }
}