using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Exceptions;
using System.IO;
using System.Collections.Generic;

namespace Objects
{
    public abstract partial class MacabreObjectController : MonoBehaviour {
        // The SpriteRenderer of the object
        private SpriteRenderer spriteRenderer
        {
            get { return GetComponentInChildren<SpriteRenderer>(); }
        }
        
        // This property is visible in inspector (used to create the collision box)
        public Texture2D footprint;
        
        // The Collider for the object
        protected abstract Collider2D collisionBox { get; }
        protected virtual Vector2[] SpriteColliderVectices
        {
            get {
                throw new MacabreException(name + " doesn't contain a sprite for edge detection or contains a edge collider.");
            }
        }

        // The Proximity detector for the object
        protected abstract Collider2D proximityBox { get; }
        protected abstract Vector2[] SpriteProximityVertices { get; }
        
    }
}
