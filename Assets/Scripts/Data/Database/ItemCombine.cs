using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using System.Linq;
using Mono.Data.SqliteClient;

namespace Data.Database
{

    /// <summary>
    /// Partial class for the Database Manager - Item Combination
    /// </summary>
    public partial class DatabaseManager
    {

        /// <summary>
        /// Finds the data for an item based on the name
        /// </summary>
        /// <returns>A structure containing data</returns>
        public static ClassData ItemFindData(string name)
        {
            ExecuteSQLQuery("select * from AClassItems where ItemName is '" + name + "'", Database.ItemCombine);

            while (reader.Read())
            {
                ClassAData data = new ClassAData();
                data.ItemID = reader.GetInt32(0);
                data.ItemName = getReaderString(1);
                data.ItemDescription = getReaderString(2);
                data.ItemProperties = getReaderString(3).Split(',').Select(sValue => sValue.Trim()).ToArray();
                return data;
            }

            ExecuteSQLQuery("select * from BClassItems where ItemName is '" + name + "'", Database.ItemCombine);

            while (reader.Read())
            {
                ClassBData data = new ClassBData();
                data.ItemID = reader.GetInt32(0);
                data.ItemName = getReaderString(1);
                data.ItemDescription = getReaderString(2);
                data.ItemProperties = getReaderString(3).Split(',').Select(sValue => sValue.Trim()).ToArray();
                return data;
            }

            throw new UnityException("The object " + name + " is not listed in the database");
        }

        /// <summary>
        /// Finds the data for an item based on its ID
        /// </summary>
        /// <returns>A structure containing the obejct data</returns>
        public static ClassData ItemFindData(int itemID)
        {
            ExecuteSQLQuery("select * from AClassItems where ItemID is " + itemID, Database.ItemCombine);

            while (reader.Read())
            {
                ClassAData data = new ClassAData(); ;
                data.ItemID = reader.GetInt32(0);
                data.ItemName = getReaderString(1);
                data.ItemDescription = getReaderString(2);
                data.ItemProperties = getReaderString(3).Split(',').Select(sValue => sValue.Trim()).ToArray();
                return data;
            }

            ExecuteSQLQuery("select * from BClassItems where ItemID is " + itemID, Database.ItemCombine);
            while (reader.Read())
            {
                ClassBData data = new ClassBData(); ;
                data.ItemID = reader.GetInt32(0);
                data.ItemName = getReaderString(1);
                data.ItemDescription = getReaderString(2);
                data.ItemProperties = getReaderString(3).Split(',').Select(sValue => sValue.Trim()).ToArray();
                return data;
            }

            throw new UnityException("The object ID " + itemID + " is not listed in the database");
        }

        /// <summary>
        /// Returns the combination data for the combination between item 1 and 2
        /// </summary>
        /// <param name="item1">Item1's name</param>
        /// <param name="item2">Item2's name</param>
        public static ItemCombineData ItemFindCombinationData(string item1, string item2)
        {
            ClassAData item1Data = (ClassAData)ItemFindData(item1);
            ClassAData item2Data = (ClassAData)ItemFindData(item2);

            if (!(item1Data is ClassAData)) Debug.LogWarning("The item " + item1 + " is not a class A item");
            if (!(item2Data is ClassAData)) Debug.LogWarning("The item " + item2 + " is not a class A item");

            ExecuteSQLQuery("select * from AClassCombined where AClassID1 is " + item1Data.ItemID + " and AClassID2 is " + item2Data.ItemID, Database.ItemCombine);
            ItemCombineData itemCombine;
            while (reader.Read())
            {
                ItemCombineData data = new ItemCombineData();
                data.ItemID = reader.GetInt32(0);
                data.AClassCombinedID = getReaderString(1);
                data.AClassID1 = reader.GetInt32(2);
                data.AClassID2 = reader.GetInt32(3);
                data.CombinationName = getReaderString(4);
                data.CombinationText = getReaderString(5);
                return data;
            }
            return null;
        }

        /// <summary>
        /// Takes two item strings and returns the combined item data
        /// </summary>
        /// <returns>The combination.</returns>
        public static ClassAData FindCombination(string item1, string item2)
        {
            ItemCombineData i = ItemFindCombinationData(item1, item2);
            ClassAData j = (ClassAData)ItemFindData(i.AClassCombinedID);
            return j;
        }
    }
}