using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Objects.Inanimate.Items
{
    [DataContract]
    public class Item : InanimateObject
    {
        public void Drop() { throw new NotImplementedException(); }
        public void Pickup() { throw new NotImplementedException(); }

        public int ID;
        new public string name;
        new public string description;
        public List<string> attributes = new List<string>();

        public Dictionary<string, object> properties = new Dictionary<string, object>();
        
        public override void CreateCollisionBox()
        {

        }
    }
}
