using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Objects.Inanimate.Items
{
    public abstract class Item : InanimateObject
    {
        public ItemType type;
        
        public void Drop() { throw new NotImplementedException(); }
        public void Pickup() { throw new NotImplementedException(); }

        public int ID;
        new public string name;
        new public string description;
        public List<string> properties = new List<string>();
        
    }
}
