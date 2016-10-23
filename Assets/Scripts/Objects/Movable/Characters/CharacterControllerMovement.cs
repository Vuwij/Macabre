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
            get { return GameSettings.useMoouseMovement; }
        }

        protected float walkingSpeed
        {
            get { return GameSettings.characterWalkingSpeed; }
        }
        protected float runningSpeeed
        {
            get { return GameSettings.characterRunningSpeed; }
        }
        protected float movementSpeed
        {
            get { return isRunning ? runningSpeeed : walkingSpeed; }
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
            get { return (rb2D.velocity.sqrMagnitude <= float.Epsilon); }
        }

        protected Vector2 movementVelocity
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

        // TODO Fix the sorting layer issue programmically
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<SpriteRenderer>() == null) return;
            var spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
            var otherSpriteRenderer = other.GetComponent<SpriteRenderer>();

            if (other.tag == "PlayerSprite" || other.tag == "CharacterSprite")
            {
                switch (gameObject.layer)
                {
                    case 16:
                        spriteRenderer.sortingLayerName = "Character - Front 2";
                        otherSpriteRenderer.sortingLayerName = "Character - Front 1";
                        break;
                    default:
                        spriteRenderer.sortingLayerName = "Character - Middle 2";
                        otherSpriteRenderer.sortingLayerName = "Character - Middle 1";
                        break;
                }
            }

            if (other.GetType() == typeof(PolygonCollider2D)) return;

            if (other.tag == "Object" || other.tag == "Inspectable" || other.tag == "Entrance")
            {
                if (spriteRenderer.sortingLayerName == "Character - Back 1") otherSpriteRenderer.sortingLayerName = "Objects - Back 1";
                else if (spriteRenderer.sortingLayerName == "Character - Back 2") otherSpriteRenderer.sortingLayerName = "Objects - Back 2";
                else if (spriteRenderer.sortingLayerName == "Character - Middle 1") otherSpriteRenderer.sortingLayerName = "Objects - Middle 1";
                else if (spriteRenderer.sortingLayerName == "Character - Middle 2") otherSpriteRenderer.sortingLayerName = "Objects - Middle 2";
                else if (spriteRenderer.sortingLayerName == "Character - Front 1") otherSpriteRenderer.sortingLayerName = "Objects - Front 1";
                else if (spriteRenderer.sortingLayerName == "Character - Front 2") otherSpriteRenderer.sortingLayerName = "Objects - Front 2";
            }
        }

        // When the object collides what actions to do
        void OnCollisionEnter2D(Collision2D other)
        {
            destinationPosition = transform.position;
            AnimateMovement();
        }

        void OnCollisionStay2D(Collision2D other)
        {
            AnimateMovement();
        }
    }
}