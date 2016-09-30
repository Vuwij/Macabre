using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using System.Linq;
using Mono.Data.SqliteClient;

namespace MItem {
	public class ClassData {
		public int ItemID;
		public string ItemName;
		public string ItemDescription;
		public string[] ItemProperties;
	}

	public class ClassAData : ClassData {}
	public class ClassBData : ClassData {}

	public class ItemCombineData {
		public int ItemID;
		public string AClassCombinedID;
		public int AClassID1;
		public int AClassID2;
		public string CombinationName;
		public string CombinationText;
	}
}

/// <summary>
/// Partial class for the Database Manager - Item Combination
/// </summary>
public partial class DatabaseManager : Manager  {

	/// <summary>
	/// Finds the data for an item based on the name
	/// </summary>
	/// <returns>A structure containing data</returns>
	public MItem.ClassData ItemFindData(string name) {
		ExecuteSQLQuery("select * from AClassItems where ItemName is '" + name + "'", Database.ItemCombine);

		while(reader.Read()) {
			MItem.ClassAData data = new MItem.ClassAData();
			data.ItemID = reader.GetInt32(0);
			data.ItemName = getReaderString(1);
			data.ItemDescription = getReaderString(2);
			data.ItemProperties = getReaderString(3).Split(',').Select(sValue => sValue.Trim()).ToArray();
			return data;
		}

		ExecuteSQLQuery("select * from BClassItems where ItemName is '" + name + "'", Database.ItemCombine);

		while(reader.Read()) {
			MItem.ClassBData data = new MItem.ClassBData();
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
	public MItem.ClassData ItemFindData(int itemID) {
		ExecuteSQLQuery("select * from AClassItems where ItemID is " + itemID, Database.ItemCombine);

		while(reader.Read()) {
			MItem.ClassAData data = new MItem.ClassAData();;
			data.ItemID = reader.GetInt32(0);
			data.ItemName = getReaderString(1);
			data.ItemDescription = getReaderString(2);
			data.ItemProperties = getReaderString(3).Split(',').Select(sValue => sValue.Trim()).ToArray();
			return data;
		}

		ExecuteSQLQuery("select * from BClassItems where ItemID is " + itemID, Database.ItemCombine);
		while(reader.Read()) {
			MItem.ClassBData data = new MItem.ClassBData();;
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
	public MItem.ItemCombineData ItemFindCombinationData(string item1, string item2) {
		MItem.ClassAData item1Data = (MItem.ClassAData) ItemFindData(item1);
		MItem.ClassAData item2Data = (MItem.ClassAData) ItemFindData(item2);

		if(!(item1Data is MItem.ClassAData)) Debug.LogWarning("The item " + item1 + " is not a class A item");
		if(!(item2Data is MItem.ClassAData)) Debug.LogWarning("The item " + item2 + " is not a class A item");

		ExecuteSQLQuery("select * from AClassCombined where AClassID1 is " + item1Data.ItemID + " and AClassID2 is " + item2Data.ItemID, Database.ItemCombine);
		MItem.ItemCombineData itemCombine;
		while(reader.Read()) {
			MItem.ItemCombineData data = new MItem.ItemCombineData();
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
	public MItem.ClassAData FindCombination(string item1, string item2) {
		MItem.ItemCombineData i = ItemFindCombinationData(item1, item2);
		MItem.ClassAData j = (MItem.ClassAData) ItemFindData(i.AClassCombinedID);
		return j;
	}
}
