using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Objects.Inanimate.Buildings.Components
{
    public partial class RoomController : InanimateObjectController
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
