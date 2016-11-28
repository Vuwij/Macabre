using System;
using UnityEngine;
using System.Collections;

namespace Objects.Inanimate.Buildings.Components.Path
{
    public partial class VirtualPathController : InanimateObjectController
    {
        protected override Collider2D collisionBox
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override Collider2D proximityBox
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override Vector2[] SpriteProximityVertices
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}