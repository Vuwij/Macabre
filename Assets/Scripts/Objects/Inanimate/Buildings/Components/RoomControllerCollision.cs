using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Objects.Inanimate.Buildings.Components
{
    public partial class RoomController : InanimateObjectController
    {
        public override void CreateProximityCircle() { }    // Do not create a proximity circle for the room
        public override void CreateCollisionCircle() { }    // Do not create a collision circle for the room, already created
        protected override void SetupBackEdgeCollider() { } //
    }
}
