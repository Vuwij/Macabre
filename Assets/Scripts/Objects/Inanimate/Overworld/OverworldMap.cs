using UnityEngine;
using System.Xml.Serialization;
using Extensions;

namespace Objects.Inanimate.World
{
    public partial class Overworld : InanimateObject {
        public GameObject map;

        private void LoadAllMap()
        {
            map = Loader.LoadToWorld("Environment/Overworld");
        }
    }
}