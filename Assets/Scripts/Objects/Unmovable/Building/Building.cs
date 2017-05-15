using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Objects.Unmovable.Furniture;
using Objects.Unmovable.Path;

namespace Objects.Unmovable.Building
{
    public class Building : UnmovableObject
    {
		public List<AbstractFurniture> furniture = new List<AbstractFurniture>();
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
