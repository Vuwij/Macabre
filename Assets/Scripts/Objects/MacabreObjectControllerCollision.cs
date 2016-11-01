using UnityEngine;
using Exceptions;
using System.Collections.Generic;

namespace Objects
{
    public abstract partial class MacabreObjectController : MonoBehaviour {
        // The SpriteRenderer of the object
        private SpriteRenderer spriteRenderer
        {
            get { return GetComponentInChildren<SpriteRenderer>(); }
        }

        // The Collider for the object
        protected abstract Collider2D collisionBox { get; }

        // This property is visible in inspector
        public Sprite spriteColliderShape;
        protected virtual Vector2[] SpriteColliderVectices
        {
            get {
                if(spriteColliderShape == null) throw new MacabreException(name + " doesn't contain a sprite for edge detection or contains a edge collider.");
                return spriteColliderShape.vertices;
            }
        }

    }
}
