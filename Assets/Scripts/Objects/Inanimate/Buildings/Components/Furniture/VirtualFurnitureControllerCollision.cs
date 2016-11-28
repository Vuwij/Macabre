using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Objects.Inanimate.Buildings.Components.Furniture
{
    public partial class VirtualFurnitureController : InanimateObjectController
    {
        public SpriteRenderer sprite
        {
            get
            {
                return GetComponent<SpriteRenderer>();
            }
        }

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
