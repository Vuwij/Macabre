using UnityEngine;
using System;
using System.Collections;

namespace Objects.Movable.Characters.Individuals
{
    public partial class PlayerController : CharacterController
    {
        Vector2 inputDirection;
        
        protected new bool isRunning
        {
            get { return Input.GetButton("SpeedUp"); }
        }

        new protected Vector2 movementVelocity
        {
            get { return new Vector2(
                movementSpeed * Input.GetAxisRaw("Horizontal"),
                movementSpeed * Input.GetAxisRaw("Vertical") * 2.0f);
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