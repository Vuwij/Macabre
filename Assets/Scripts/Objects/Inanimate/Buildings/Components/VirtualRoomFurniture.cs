using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Objects.Inanimate.Buildings.Components
{
    using Furniture;

    public partial class VirtualRoom : MonoBehaviour
    {
        public List<VirtualFurniture> Furniture
        {
            get
            {
                return gameObject.GetComponentsInChildren<VirtualFurniture>().ToList();
            }
        }
    }
}
