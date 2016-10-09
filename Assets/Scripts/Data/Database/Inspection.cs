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
    /// Partial class for the Database Manager - Inspection
    /// </summary>
    public partial class DatabaseManager
    {
        public class Inspection
        {
            const int inspectTextVariations = 3;
            enum InspectType { RANDOM, ORDER, ONCE, WAIT };
            static InspectType inspectType = InspectType.RANDOM;

            //FOR INSPECTING AND INTERACTING WITH OBJECTS
            public static string GetInspectText(string location, string objectName, out int viewCount)
            {
                StartDatabase(inspectLocation);

                string[] inspectText = new string[inspectTextVariations];
                int inspectTextCount = inspectText.Length;

                string returnInspectText = "";
                viewCount = new int();

                //CLEARS INSPECT TEXT VALUES
                for (int i = 0; i < inspectText.Length; i++) inspectText[i] = "";

                //GETS INSPECT TEXT FROM DATABASE AND PUTS INTO STRING ARRAY
                ExecuteSQLQuery("SELECT Id,Name,Inspect_Text,Inspect_Text_Alt,Inspect_Text_Alt_2,Inspect_Text_Type FROM '" + location + "' WHERE Name Like '" + objectName + "'");

                while (reader.Read())
                {
                    inspectText[0] = getReaderString(2);
                    inspectText[1] = getReaderString(3);
                    inspectText[2] = getReaderString(4);
                }

                //COUNT FOR EMPTY ARRAYS
                for (int i = 0; i < inspectText.Length; i++)
                {
                    if (inspectText[i] == "") inspectTextCount--;
                }

                switch (inspectType)
                {
                    case InspectType.RANDOM:
                        int choice = UnityEngine.Random.Range(0, inspectTextCount);
                        returnInspectText = inspectText[choice];
                        break;
                    case InspectType.ORDER:
                        if (viewCount < inspectTextCount) returnInspectText = inspectText[viewCount];
                        else returnInspectText = inspectText[inspectText.Length - 1];
                        break;
                    case InspectType.ONCE:
                        if (viewCount < inspectTextCount) returnInspectText = inspectText[viewCount];
                        else returnInspectText = "";
                        break;
                    case InspectType.WAIT:
                        returnInspectText = inspectText[0];
                        break;
                }

                viewCount++;

                CloseDatabase();
                return returnInspectText;
            }
        }
    }
}