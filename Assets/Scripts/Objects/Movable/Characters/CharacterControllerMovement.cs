using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Objects.Movable.Characters
{
    public abstract partial class CharacterController : MovingObjectController
    {
        public bool keyboardMovement
        {
            get { return GameSettings.useKeyboardMovement; }
        }
        public bool mouseMovement
        {
            get { return GameSettings.useMouseMovement; }
        }

        protected float walkingSpeed
        {
            get { return GameSettings.characterWalkingSpeed; }
        }
        protected float runningSpeed
        {
            get { return GameSettings.characterRunningSpeed; }
        }
        public float movementSpeed
        {
            get { return isRunning ? runningSpeed : walkingSpeed; }
        }

        protected bool lockMovement {
            get { return character.lockMovement; }
            set { character.lockMovement = value; }
        }

        protected bool isRunning
        {
            get { return Input.GetButton("SpeedUp"); }
        }
        private bool isMoving // HACK Check if this value works
        {
            get { return (rb2D.velocity.sqrMagnitude >= float.Epsilon); }
        }

        protected virtual Vector2 movementVelocity
        {
            get { return new Vector2(movementSpeed, movementSpeed * 2.0f); }
        }

        public void ShowMovementAnimation(Vector3 destination, MovementAnimation movementAnimation = MovementAnimation.Smooth)
        {
            if (!isMoving) return;

            switch (movementAnimation)
            {
                case MovementAnimation.Fade:
                    StartCoroutine(MoveToDestinationFade(destination));
                    break;
                case MovementAnimation.Smooth:
                    StartCoroutine(MoveToDestinationSmooth(destination));
                    break;
                case MovementAnimation.Teleport:
                    StartCoroutine(MoveToDestinationTeleport(destination));
                    break;
                default:
                    throw new UnityException("The Movement Function doesn't accept the parameter " + movementAnimation);
            }
        }
        
        public void LockMovement()
        {
            rb2D.velocity.Set(0, 0);
            AnimateMovement();
            lockMovement = true;
        }

        public void UnlockMovement()
        {
            lockMovement = false;
        }
        
        // TODO create the move to destination types
        private IEnumerator MoveToDestinationFade(Vector3 destination)
        {
            throw new NotImplementedException();
        }

        private IEnumerator MoveToDestinationTeleport(Vector3 destination)
        {
            throw new NotImplementedException();
        }

        private IEnumerator MoveToDestinationSmooth(Vector3 destination)
        {
            throw new NotImplementedException();
        }
    }
}