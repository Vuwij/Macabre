using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using UnityEngine;

namespace Objects.Inanimate.World
{
    public partial class Overworld : InanimateObject, ILoadable
    {
        public void CreateNew() { }
 
        public void LoadAll()
        {
            LoadAllMap();
        }

        public override void CreateCollisionBox()
        {

        }
    }
}
