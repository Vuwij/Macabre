using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.Data;
using System.Linq;
using Mono.Data.SqliteClient;

namespace Data
{
	public class Table
	{
		Database database;
		protected Regex numRegex = new Regex("[0-9]{1,}");

		public Table (Database database)
		{
			this.database = database;
		}

		protected IDataReader Reader {
			get {
				return database.reader;
			}
		}

		public void ExecuteSQL(string query)
		{
			using (IDbCommand dbcmd = database.dbconn.CreateCommand())
			{
				dbcmd.CommandText = query;
				database.reader = dbcmd.ExecuteReader();
			}
		}
			
		// Return a list of integers
		protected List<int> StringToIntList(string str)
		{
			List<int> list = new List<int>();
			foreach (var s in str.Split(',', ' '))
			{
				int num;
				if (int.TryParse(s, out num))
					list.Add(num);
			}
			return list;
		}

		// Returns a list of strings
		protected string[] StringToStringList(string str)
		{
			if (str == "") return null;
			string[] stringList = str.Split(',');
			for (int i = 0; i < stringList.Count(); i++)
				stringList[i] = stringList[i].Trim();
			return stringList;
		}

		protected List<string> StringToStringArray(string str)
		{
			String[] stringArray = str.Split(',');
			return new List<string>(stringArray);
		}

		protected int GetNumberOfString(string s)
		{
			string numString = numRegex.Match(s).Groups[1].Value;
			int num;
			int.TryParse(numString, out num);
			return num;
		}
	}
}

