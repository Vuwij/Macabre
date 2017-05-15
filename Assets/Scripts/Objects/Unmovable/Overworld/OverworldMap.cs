using UnityEngine;
using System.Xml.Serialization;
using Extensions;

namespace Objects.Unmovable
{
    public partial class Overworld : UnmovableObject {
        public GameObject map;

        // TODO : Fix here
        private void LoadAllMap()
        {
            // map = Loader.LoadToWorld("Environment/Overworld");
        }
    }
}