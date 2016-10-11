using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Objects.Movable.Characters
{
    public abstract partial class CharacterController : MovingObjectController
    {
        private Animator animator
        {
            get
            {
                return GetComponentInChildren<Animator>();
            }
        }  
        private Vector2 destinationPosition
        {
            get { return character.destinationPosition; }
            set { character.destinationPosition = value; }
        }

        public void AnimateDeath()
        {
            animator.SetBool(Animator.StringToHash("Die"), true);
        }

        partial void AnimateMovement(bool isMoving)
        {
            int xDir = 0, yDir = 0;

            if (isMoving)
            {
                if (mouseMovement)
                {
                    if (destinationPosition.x > transform.position.x) xDir = 1;
                    else if (destinationPosition.x < transform.position.x) xDir = -1;

                    if (destinationPosition.y > transform.position.y) yDir = 1;
                    else if (destinationPosition.y < transform.position.y) yDir = -1;
                }
                if (keyboardMovement)
                {
                    if (movementVelocity.x > 0) xDir = 1;
                    else if (movementVelocity.x < 0) xDir = -1;

                    if (movementVelocity.y > 0) yDir = 1;
                    else if (movementVelocity.y < 0) yDir = -1;
                }

                animator.SetBool(Animator.StringToHash("IsActive"), false);
                animator.SetBool(Animator.StringToHash("IsMoving"), true);

                if (xDir != 0) animator.SetFloat(Animator.StringToHash("MoveSpeed-x"), xDir);
                if (yDir != 0) animator.SetFloat(Animator.StringToHash("MoveSpeed-y"), yDir);

            }
            else
            {
                animator.SetBool(Animator.StringToHash("IsActive"), true);
                animator.SetBool(Animator.StringToHash("IsMoving"), false);
            }
        }
    }
}