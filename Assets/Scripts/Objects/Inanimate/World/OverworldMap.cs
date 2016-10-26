﻿using UnityEngine;
using System.Xml.Serialization;

namespace Objects.Inanimate.World
{
    public partial class Overworld : InanimateObject {
        [XmlIgnore]
        public GameObject map = Resources.Load("Environment/Overworld") as GameObject;
        
        public void InstantiateMap()
        {
            Object.Instantiate(map);
        }
    }
}