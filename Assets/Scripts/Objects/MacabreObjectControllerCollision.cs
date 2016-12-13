using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Exceptions;
using System.IO;
using System.Collections.Generic;

namespace Objects
{
    public abstract partial class MacabreObjectController : MonoBehaviour
    {
        // The SpriteRenderer of the object
        private SpriteRenderer spriteRenderer
        {
            get { return GetComponentInChildren<SpriteRenderer>(); }
        }
        
        // This property is visible in inspector (used to create the collision box)
        public Texture2D footprint;

        // Abstract creates of the collision and proximity
        public abstract void CreateCollisionCircle();
        public abstract void CreateProximityCircle();

        // The Collider and Proximity Boxfor the object
        protected abstract PolygonCollider2D collisionBox { get; }
        protected abstract PolygonCollider2D proximityBox { get; }
        
        private Vector2[] collisionVertices
        {
            get { return collisionBox.points; }
        }
        private Vector2[] proximityVertices
        {
            get { return proximityBox.points; }
        }
    }
}
