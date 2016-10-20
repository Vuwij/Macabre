using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using UnityEngine;

namespace Objects.Inanimate.World
{
    [XmlInclude(typeof(Overworld))]
    public partial class Overworld : InanimateObject
    {
        
    }
}
