using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.Data;
using System.Linq;
using Mono.Data.SqliteClient;

namespace Data.Database
{
    public partial class DatabaseManager
    {
        public class Utility
        {
            // Return a list of integers
            public static List<int> StringToIntList(string str)
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
            public static string[] StringToStringList(string str)
            {
                string[] stringList = str.Split(',');
                for (int i = 0; i < stringList.Count(); i++)
                    stringList[i] = stringList[i].Trim();
                return stringList;
            }

            public static List<string> StringToStringArray(string str)
            {
                String[] stringArray = str.Split(',');
                return new List<string>(stringArray);
            }
            
            public static string getReaderString(int col)
            {
                if (reader.IsDBNull(col)) return "";
                return reader.GetString(col);
            }

            private static Regex numRegex = new Regex("[0-9]{1,}");
            public static int GetNumberOfString(string s)
            {
                string numString = numRegex.Match(s).Groups[1].Value;
                int num;
                int.TryParse(numString, out num);
                return num;
            }
        }
    }
}