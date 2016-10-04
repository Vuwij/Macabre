using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using System.Linq;
using Mono.Data.SqliteClient;

namespace Data.Database
{
    public partial class DatabaseManager
    {
        static IDataReader reader;
        static IDbConnection dbconn;
        static IDbCommand dbcmd;

        /// <summary>
        /// This area is the database, there are 3 databases, which are all used
        /// </summary>
        enum Database
        {
            InspectLocation, Conversation, ItemCombine
        }
        static Database currentDatabase;
        static string inspectLocation, conversations, itemCombine;

        static bool DatabasesLoaded = false;
        public static void LoadDatabases()
        {
            var save = Save.saveInfo.currentSave;
            string loadURI = save.saveLocation;

            inspectLocation = "URI=file:" + loadURI + "/Inspect.db";
            conversations = "URI=file:" + loadURI + "/Conversations.db";
            itemCombine = "URI=file:" + loadURI + "/ItemCombine.db";

            DatabasesLoaded = true;
        }

        static void StartDatabase(string databaseLocation)
        {
            dbconn = (IDbConnection)new SqliteConnection(databaseLocation);
            dbconn.Open(); //Open connection to the database.
            dbcmd = dbconn.CreateCommand();
        }

        static void CloseDatabase()
        {
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }

        static void ExecuteSQLQuery(string query)
        {
            Debug.Log("SQL: " + query);
            dbcmd.CommandText = query;
            reader = dbcmd.ExecuteReader();
        }

        static void ExecuteSQLQuery(string query, Database db)
        {
            switch (db)
            {
                case Database.Conversation:
                    StartDatabase(conversations);
                    break;
                case Database.InspectLocation:
                    StartDatabase(inspectLocation);
                    break;
                case Database.ItemCombine:
                    StartDatabase(itemCombine);
                    break;
            }
            try
            {
                ExecuteSQLQuery(query);
            }
            catch (SqliteSyntaxException s)
            {
                Debug.LogError(s.Message);
            }
        }

        /// <summary>
        /// This Function gets the index of the database
        /// </summary>
        /// <returns>The SQL query string.</returns>
        /// <param name="query">The Query To Execute</param>
        /// <param name="db">The database</param>
        /// <param name="index">The index to query</param>
        static string ExecuteSQLQueryString(string query, Database db, int index)
        {
            ExecuteSQLQuery(query, db);
            while (reader.Read()) return reader.GetString(index);
            return "";
        }

        /// <summary>
        /// This Function gets the int of the index of a database
        /// </summary>
        /// <returns>The SQL query int.</returns>
        /// <param name="query">The query</param>
        /// <param name="db">The database</param>
        /// <param name="index">The index</param>
        static int ExecuteSQLQueryInt(string query, Database db, int index)
        {
            ExecuteSQLQuery(query, db);
            while (reader.Read()) return reader.GetInt32(index);
            return 0;
        }

        #region General Methods

        private static IEnumerable<int> StringToIntList(string str)
        {
            if (String.IsNullOrEmpty(str))
                yield break;

            foreach (var s in str.Split(','))
            {
                int num;
                if (int.TryParse(s, out num))
                    yield return num;
            }
        }

        private static List<string> StringToStringList(string str)
        {
            String[] stringList = str.Split(',');
            return new List<string>(stringList);
        }

        private static string getReaderString(int col)
        {
            if (reader.IsDBNull(col)) return "";
            return reader.GetString(col);
        }

        public static char getLastCharInString(string s)
        {
            return s[s.Length - 1];
        }

        #endregion
    }
}