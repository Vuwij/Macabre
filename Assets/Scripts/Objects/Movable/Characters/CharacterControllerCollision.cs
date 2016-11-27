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

        protected override void CreateCollisionCircle()
        {
            float width = spriteRenderer.sprite.rect.width;
            CollisionCircle.radiusX = width / 3.0f;
            CollisionCircle.radiusY = width / 6.0f;
        }
    }
}