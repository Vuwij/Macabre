using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;
using Objects.Inanimate.World.Structures;

namespace Objects.Inanimate.World
{
    public partial class Overworld : InanimateObject {
        

        private void InstantiateStructures()
        {
            Object.Instantiate(map);
        }
    }
}