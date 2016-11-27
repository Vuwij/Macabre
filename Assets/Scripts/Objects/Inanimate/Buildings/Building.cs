using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Objects.Inanimate.Buildings
{
    public class Building : InanimateObject
    {
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
