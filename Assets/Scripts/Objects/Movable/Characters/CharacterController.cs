using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Objects.Movable.Characters.Individuals;

namespace Objects.Movable.Characters
{
    public abstract partial class CharacterController : MovingObjectController
    {
        // This is the object for the character controller
        new private Character mObject;
        public Character character { get { return mObject; } }

        private string characterName { get { return mObject.name; } }

        // A simple reference to the player for interaction in conversation
        public static PlayerController playerController
        {
            get { return GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>(); }
        }

        // The child object is the one that contains the sprite
        private GameObject childObject
        {
            get { return gameObject.transform.FindChild(gameObject.name + "Sprite").gameObject; }
        }
        
    }
}