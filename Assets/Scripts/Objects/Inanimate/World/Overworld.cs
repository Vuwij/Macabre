using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using UnityEngine;

namespace Objects.Inanimate.World
{
    public partial class Overworld : InanimateObject
    {
        public Overworld()
        {
            // Creates the background map for the game
            InstantiateMap();

            // Loads all the buildings
            InstantiateStructures();
        }
    }
}
