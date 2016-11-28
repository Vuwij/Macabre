using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Objects.Inanimate.Buildings.Components.Furniture;
using Objects.Inanimate.Buildings.Components.Path;

namespace Objects.Inanimate.Buildings
{
    public class Building : InanimateObject
    {
        public List<VirtualFurniture> furniture = new List<VirtualFurniture>();
        public List<VirtualPathController> path = new List<VirtualPathController>();

        public Building(string name)
        {
            this.name = name;
        }
        
        public override void CreateCollisionBox()
        {
            throw new NotImplementedException();
        }
    }
}
