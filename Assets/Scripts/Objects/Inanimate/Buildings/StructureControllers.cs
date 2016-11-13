using System;
using System.Collections.Generic;
using UnityEngine;

using Objects.Inanimate.World.Structures.Buildings.Controllers;

namespace Objects.Inanimate.World.Structures.Buildings
{
    /// <summary>
    /// A list of all the structures in the game
    /// </summary>
    public abstract partial class StructureControllers
    {
        public List<BuildingController> buildingControllers
        {
            get { throw new NotImplementedException(); }
        }
    }
}
