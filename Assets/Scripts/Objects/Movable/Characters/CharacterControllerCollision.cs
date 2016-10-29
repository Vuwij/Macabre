using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;

namespace Objects.Movable.Characters
{
    public abstract partial class CharacterController : MovingObjectController
    {
        private SpriteRenderer spriteRenderer
        {
            get { return GetComponentInChildren<SpriteRenderer>(); }
        }
        private EllipseCollider2D ellipseCollider2D
        {
            get { return GetComponent<EllipseCollider2D>(); }
        }

        public override void CreateCollisionBox()
        {
            if (ellipseCollider2D != null) return;
            gameObject.AddComponent<EllipseCollider2D>();

            float width = spriteRenderer.sprite.rect.width;
            Debug.Log(width);
        }

        // When the object collides what actions to do
        void OnCollisionEnter2D(Collision2D other)
        {
            AnimateMovement();
        }

        void OnCollisionStay2D(Collision2D other)
        {
            AnimateMovement();
        }
    }
}