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
	public class Database
	{
		public static Database main;
		public Databases.Characters characters;
		public Databases.Conversations conversations;
		public Databases.Items items;

		public IDataReader reader;
		public IDbConnection dbconn;

		static string connectionString
		{
			get { return "URI=file:" + GameManager.main.saves.current.database + ",version=3"; }
		}

		public Database()
		{
			dbconn = new SqliteConnection(connectionString);
			dbconn.Open();

			main = this;
			characters = new Data.Databases.Characters (this);
			conversations = new Data.Databases.Conversations (this);
			items = new Data.Databases.Items (this);
		}

		~Database()
		{
			if (reader != null) reader.Close();
			if (dbconn != null) dbconn.Close();

		}
	}
}

