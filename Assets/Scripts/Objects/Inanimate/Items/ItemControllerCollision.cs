using System;
using UnityEngine;

namespace Objects.Inanimate.Items
{
    public abstract partial class ItemController : InanimateObjectController {
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
