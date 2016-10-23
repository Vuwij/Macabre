using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using System.Linq;
using Mono.Data.SqliteClient;
using Objects.Inanimate.Items;

namespace Data.Database
{
    public partial class DatabaseManager
    {
        public static class ItemCombine
        {
            public static void GetItemData(string name, Item item)
            {
                string itemTable = "";
                if (item.type == ItemType.InventoryItemClassA)
                    itemTable = "AClassItems";
                else
                    itemTable = "BClassItems";

                ExecuteSQLQuery("select * from " + itemTable + " where ItemName is '" + name + "'");

                reader.Read();

                item.ID = reader.GetInt32(0);
                item.name = reader.GetString(1);
                item.description = reader.GetString(2);
                item.properties = Utility.StringToStringArray(reader.GetString(3));

                throw new UnityException("The object " + name + " is not listed in the database");
            }
            
            public static Item FindCombinationItem(Item a, Item b)
            {
                ExecuteSQLQuery("select * from AClassCombined where ItemID1 is " + a.ID + " and AClassID2 is " + b.ID);
                reader.Read();

                int finalItemID = reader.GetInt32(0);
                string combinationText = reader.GetString(3);

                return Items.GetItemFromID(finalItemID);
            }
        }
    }
}