using System.Collections.Generic;
using UnityEngine;

namespace Objects.Inanimate.Items
{
    // A list of all of the items in the game
    public static class Items
    {
        static List<GameObject> items_ = new List<GameObject>();

        // HACK to see if this works
        public static IEnumerable<Item> items
        {
            get
            {
                if(items_ == null) UpdateItemsFromResources();
                foreach (GameObject g in items_)
                    yield return g.GetComponent<Item>();
            }
        }

        public static Item GetItemFromID(int ID)
        {
            foreach (Item i in items)
                if (i.ID == ID)
                    return i;
            return null;
        }

        private static void UpdateItemsFromResources()
        {
            Object[] items = Resources.LoadAll("Objects/Inanimate/Items");
            foreach (Object o in items)
                items_.Add((GameObject)o);
        }
    }
}
