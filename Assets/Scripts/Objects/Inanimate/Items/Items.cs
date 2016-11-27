using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using Data.Database;
using Exceptions;

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

        public ItemController this[string s]
        {
            get
            {
                return GetItem(s);
            }
        }

        public const string startItemDirectory = "Assets/Resources/Objects/Inanimate/Items/LoadAtStart";
        public const string allItemDirectory = "Assets/Resources/Objects/Inanimate/Items";

        // Finds the gameobject, then loads into the world or parent if it is not in the world
        public ItemController GetItem(string name, GameObject parent = null)
        {
            ItemController item = ItemControllers.Where(x => x.name == name).SingleOrDefault();
            if (item != null) return item;

            if (!File.Exists(allItemDirectory + "/" + name + ".prefab")) throw new UnityException("Resource " + name + " does not exist in Items Directory");

            GameObject g = Loader.LoadToWorld("Objects/Inanimate/Items/" + name);
            
            if (parent != null)
            {
                g.transform.parent = parent.transform;
                g.transform.position = parent.transform.position;
            }

            var itemController = g.GetComponent<ItemController>();
            if (itemController == null) throw new MacabreException("Item " + name + " does not contain ItemController");
            itemControllers.Add(itemController);
            itemDictionary.Add(itemController.name, new Item(itemController.name));
            return itemController;
        }
        
        public override void CreateNew()
        {
            DirectoryInfo itemsAtStart = new DirectoryInfo(startItemDirectory);

            // This loads everything in the resources folder
            var allItems = itemsAtStart.GetFiles()
                .Where(x => x.Extension != ".meta");
            
            foreach (var file in allItems)
            {
                string name = Path.GetFileNameWithoutExtension(file.FullName);
                ItemDictionary.Add(name, new Item(name));
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
                // FIXME, loader.load should store the resource in this class somewhere
                GameObject itemObject;
                if (File.Exists(startItemDirectory + "/" + c.Key + ".prefab"))
                    itemObject = Loader.LoadToWorld("Objects/Inanimate/Items/LoadAtStart/" + c.Key);
                else
                    itemObject = Loader.LoadToWorld("Objects/Inanimate/Items/" + c.Key);

                // Relocate the Item to the correct position
                itemObject.transform.position = c.Value.position;

                // Add it to the list of Item controllers
                if (itemObject.GetComponent<ItemController>() == null) throw new UnityException(itemObject.name + " doesn't have controller attached");
                itemControllers.Add(itemObject.GetComponent<ItemController>());
            }
        }
    }
}
