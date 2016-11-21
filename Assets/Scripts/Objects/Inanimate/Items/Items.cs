using System;
using System.Linq;
using System.IO;
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

        // Item Models
        [DataMember(IsRequired = true, Order = 0)]
        private Dictionary<string, Item> itemDictionary = new Dictionary<string, Item>();
        [IgnoreDataMember]
        public static Dictionary<string, Item> ItemDictionary
        {
            get { return main.itemDictionary; }
            set { main.itemDictionary = value; }
        }

        // Item Controllers
        [IgnoreDataMember]
        public List<ItemController> itemControllers = new List<ItemController>();
        [IgnoreDataMember]
        public static List<ItemController> ItemControllers
        {
            get { return main.itemControllers; }
        }

        public override void CreateNew()
        {
            DirectoryInfo allItemDirectory = new DirectoryInfo("Assets/Resources/Objects/Inanimate/Items");

            // This loads everything in the resources folder
            var allItems = allItemDirectory.GetFiles()
                .Where(x => x.Extension != ".meta");
            
            foreach (var file in allItems)
            {
                string name = Path.GetFileNameWithoutExtension(file.FullName);
                ItemDictionary.Add(name, new Item { name = name });
            }
        }

        public override void LoadAll()
        {
            Debug.Log("Loading All");
            if (itemControllers == null) itemControllers = new List<ItemController>();

            // Load all the Items on the screen
            foreach (KeyValuePair<string, Item> c in ItemDictionary)
            {
                // Load the resources first
                var charObject = Loader.Load("Objects/Inanimate/Items/" + c.Key);

                // Relocate the Item to the correct position
                charObject.transform.position = c.Value.position;

                // Add it to the list of Item controllers
                if (charObject.GetComponent<ItemController>() == null) throw new UnityException(charObject.name + " doesn't have controller attached");
                itemControllers.Add(charObject.GetComponent<ItemController>());
            }
        }
    }
}
