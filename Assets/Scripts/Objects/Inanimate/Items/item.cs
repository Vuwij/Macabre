using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Data.Database;

namespace Objects.Inanimate.Items
{
    [DataContract]
    public class Item : InanimateObject
    {
        public int ID;
        new public string name;
        new public string description;
        public List<string> attributes = new List<string>();

        public Dictionary<string, object> properties = new Dictionary<string, object>();

        public Item() { }
        public Item(string name)
        {
            this.name = name;
            DatabaseConnection.ItemDB.UpdateItemInfo(this);
        }

        public void Drop() { throw new NotImplementedException(); }
        public void Pickup() { throw new NotImplementedException(); }

        public override void CreateCollisionBox()
        {

        }
    }
}
