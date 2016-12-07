using System;
using UnityEngine;

namespace Objects.Inanimate.Buildings
{
    public abstract partial class BuildingController : InanimateObjectController
    {
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
