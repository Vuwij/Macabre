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

        protected override PolygonCollider2D collisionBox
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override PolygonCollider2D proximityBox
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
