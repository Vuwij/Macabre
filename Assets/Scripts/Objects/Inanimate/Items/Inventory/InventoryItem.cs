using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Data.Database;

namespace Objects.Items.Inventory
{
    public abstract class InventoryItem
    {
        /// <summary>
        /// The Item that the inventory Object wraps around it
        /// </summary>
        public List<Item> itemList = new List<Item>();
        public string name
        {
            get
            {
                string s = "";
                foreach (Item i in itemList)
                    s += i.name + " ";
                return s;
            }
        }

        // The number you can combine
        [Range(1, 4)]
        public int combinationStack = 1;

        // Contructor for converting objects on the ground
        public InventoryItem(Item item)
        {
            itemList.Add(item);
        }

        // If you want to convert the MacabreItem into an Inventory Object. It will have to be wrapped around first
        public static explicit operator InventoryItem(Item m)
        {
            var data = DatabaseManager.ItemCombine.ItemFindData(m.name);
            if (data is ClassAData) return new InventoryObjectClassA(m);
            if (data is ClassBData) return new InventoryObjectClassB(m);

            Debug.LogError("The object " + m.name + " cannot be converted into an Inventory Object");
            return null;
        }

        // For combining objects a and b, first check the combination amount
        public static InventoryItem operator +(InventoryItem a, InventoryItem b)
        {
            // This exception is if the items literally cannot be combined
            if (a.combinationStack + b.combinationStack > 4)
            {
                //TODO: Need a warning message to the player that the items cannot be combined
                string s = "The items: " + a.name + " and " + b.name + " cannot be combined." +
                    " The maximum combination stack is 4 and the current stack is " + a.combinationStack + b.combinationStack;
                Debug.LogWarning(s);
                return a;
            }

            // This exception is if the type a and b are both
            if (a is InventoryObjectClassB && b is InventoryObjectClassB)
            {
                string s = "The items: " + a.name + " and " + b.name + " cannot be combined." +
                    " The maximum combination stack is 4 and the current stack is " + a.combinationStack + b.combinationStack;
                return a;
            }

            // Stacking the inventory objects together
            a.combinationStack += b.combinationStack;
            a.itemList.AddRange(b.itemList);
            return a;
        }
    }
}