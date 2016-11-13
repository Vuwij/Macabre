using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;

namespace Objects.Inanimate.Items
{
    // A list of all of the items in the game
    [DataContract]
    public sealed class Items : EntityList
    {
        [IgnoreDataMember]
        public static Items main
        {
            get { return (MacabreWorld.current != null) ? MacabreWorld.current.items : null; }
        }

        // Character Models
        [DataMember(IsRequired = true, Order = 0)]
        private Dictionary<string, Item> itemDictionary = new Dictionary<string, Item>();
        [IgnoreDataMember]
        public static Dictionary<string, Item> ItemDictionary
        {
            get { return main.itemDictionary; }
            set { main.itemDictionary = value; }
        }

        // Character Controllers
        [IgnoreDataMember]
        public List<CharacterController> characterControllers = new List<CharacterController>();
        [IgnoreDataMember]
        public static List<CharacterController> CharacterControllers
        {
            get { return main.characterControllers; }
        }

        // FIXME Database name must match resource name
        public override void CreateNew()
        {
        }

        public override void LoadAll()
        {
        }
    }
}
