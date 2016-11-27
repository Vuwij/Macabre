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
        public static class ItemDB
        {
            public static Item UpdateItemInfo(ItemController itemController)
            {
                Item item = itemController.item;
                return UpdateItemInfo(item, itemController.type);
            }
            
            public static Item UpdateItemInfo(Item item, ItemType type = ItemType.InventoryItemClassA)
            {
                Item updated = GetItemInfo(item.name, type);

                item.ID = updated.ID;
                item.description = updated.name;
                item.attributes = updated.attributes;

                return item;
            }

            public static Item GetItemInfo(string name, ItemType type = ItemType.InventoryItemClassA)
            {
                string itemTable = "";
                if (type == ItemType.InventoryItemClassA)
                    itemTable = "Items_ClassA";
                else
                    itemTable = "Items_ClassB";

                ExecuteSQLQuery("select * from " + itemTable + " where Name is '" + name + "'");

                if(reader.Read())
                {
                    return new Item
                    {
                        ID = reader.GetInt32(0),
                        name = reader.GetString(1),
                        description = reader.GetString(2),
                        attributes = Utility.StringToStringArray(reader.GetString(3))
                    };
                }
                throw new UnityException("Item " + name + " not found in the database");
            }

            public static Item GetItemInfo(int id, ItemType type = ItemType.InventoryItemClassA)
            {
                string itemTable = "";
                if (type == ItemType.InventoryItemClassA)
                    itemTable = "Items_ClassA";
                else
                    itemTable = "Items_ClassB";

                ExecuteSQLQuery("select * from " + itemTable + " where ID is '" + id + "'");

                if (reader.Read())
                {
                    return new Item
                    {
                        ID = reader.GetInt32(0),
                        name = reader.GetString(1),
                        description = reader.GetString(2),
                        attributes = Utility.StringToStringArray(reader.GetString(3))
                    };
                }
                throw new UnityException("Item " + id + " not found in the database");
            }

            // TODO Validate and Fix
            public static Item FindCombination(Item a, Item b)
            {
                int finalItemID = -1;

                UpdateItemInfo(a);
                UpdateItemInfo(b);
                
                // Search both ways
                ExecuteSQLQuery("select * from Items_Combine where ItemID1 is " + a.ID + " and ItemID2 is " + b.ID);
                if (reader.Read())
                    finalItemID = reader.GetInt32(0);

                ExecuteSQLQuery("select * from Items_Combine where ItemID1 is " + b.ID + " and ItemID2 is " + a.ID);
                if (reader.Read())
                    finalItemID = reader.GetInt32(0);

                // If not found
                if (finalItemID == -1) return null;

                return GetItemInfo(finalItemID);
            }
        }
    }
}