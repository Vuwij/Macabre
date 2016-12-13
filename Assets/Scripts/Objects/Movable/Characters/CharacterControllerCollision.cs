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

        public override void CreateCollisionCircle()
        {
            if (collisionCircle == null) collisionCircle = gameObject.AddComponent<EllipseCollider2D>();
            float width = spriteRenderer.sprite.rect.width;
            collisionCircle.radiusX = width * 0.25f;
            collisionCircle.radiusY = width * 0.125f;
        }
    }
}