using System;
using UnityEngine;
using System.Collections;

namespace Objects.Inanimate.Buildings.Components.Path
{
    public partial class VirtualPathController : InanimateObjectController
    {
        protected override PolygonCollider2D proximityBox
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override void SetupBackEdgeCollider()
        {
            // No back edge collider paths
        }

        public override void CreateProximityCircle()
        {
            // Don't create a proximity circle
        }

    }
}