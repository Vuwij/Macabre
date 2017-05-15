using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using System.Xml.Serialization;

namespace Objects.Unmovable
{
    public abstract class UnmovableObject : MacabreObject
    {
        [XmlIgnore]
        private GameObject gameObject;
       
        public string name;
        public string description;
        
        // Creates a collision box of the MacabreObject
        public abstract void CreateCollisionBox();
    }
}