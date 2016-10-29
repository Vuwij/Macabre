using UnityEngine;
using System;
using System.Collections;

namespace Objects.Movable.Characters.Individuals
{
    public sealed partial class PlayerController : CharacterController
    {
        new bool isRunning
        {
            get { return Input.GetButton("SpeedUp"); }
        }

        protected override Vector2 movementVelocity
        {
            get {
                return new Vector2(
                    movementSpeed * Input.GetAxisRaw("Horizontal") * 2.0f,
                    movementSpeed * Input.GetAxisRaw("Vertical"));
            }
        }
        
        void moveUsingKeyboard()
        {
            rb2D.velocity = lockMovement ? Vector2.zero : movementVelocity;
        }

        // TODO - Finish mouse movement
        void moveOnMouseClick() { throw new NotImplementedException(); }
    }
}