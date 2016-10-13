using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using System.Linq;
using Mono.Data.SqliteClient;

using Conversations;

namespace Data.Database {

    /// <summary>
    /// Partial class for the Database Manager - Conversation
    /// </summary>
    public static partial class DatabaseManager {

        public static class Conversation
        {
            // Prerequisites for a single conversation, 0 is always empty
            public static int characterLimit { get { return GameSettings.conversationCharacterLimit; } }
            private static bool[] prerequisites = new bool[characterLimit];

            /// <summary>
            /// Tests if the character exists
            /// </summary>
            public static bool TestIfCharacterExistsInDatabase(string character)
            {
                try
                {
                    StartDatabase(conversations);
                    ExecuteSQLQuery("select * from " + character);
                    reader.Read();
                }
                catch (SqliteExecutionException s)
                {
                    Debug.Log(s.Message);
                    return false;
                }
                return true;
            }
            
            public static SingleConversation FindConversationForCharacter(string characterName)
            {
                //0. Start Database Connection
                StartDatabase(conversations);

                //1. Reset boolean flags for all conversations
                for (int i = 0; i < 500; i++)
                    prerequisites[i] = false;

                //2. Search all possible prerequisites for ones that start with S
                ExecuteSQLQuery("select \"Order\", \"End Type (0/1)\" from " + characterName + " where Prerequisites like '%S%' and \"End Type (0/1)\" not like '1' Order by Prerequisites");

                //3. Find the one with the first number S1 and End Type 0
                int starterNum = 0; int endtype = 0;
                while (reader.Read())
                {
                    starterNum = reader.GetInt32(0);
                    endtype = reader.GetInt32(1);
                }

                //4. Set that end type to 1
                if (endtype == 0)
                {// 2 for looping
                    ExecuteSQLQuery("Update " + characterName + " SET 'End Type (0/1)' = '1' where \"Order\" is " + starterNum);
                    reader.Read();
                }

                //5. Set all possible paths to have +ve boolean flags
                ExecuteSQLQuery("select \"Add Prerequisite\" from " + characterName + " where \"Order\" is " + starterNum);
                reader.Read();
                string newPrerequisiteString = getReaderString(0);
                List<string> newPrerequisites = StringToStringList(newPrerequisiteString);

                foreach (string s in newPrerequisites)
                {
                    ExecuteSQLQuery("select \"Order\" from " + characterName + " where Prerequisites like '" + s + "'");
                    reader.Read();
                    int i = reader.GetInt32(0);
                    prerequisites[i] = true;
                }

                //6. Return that specific conversation
                ExecuteSQLQuery("select \"Source\", \"Conversation\" from " + characterName + " where \"Order\" is " + starterNum);

                string speaker = ""; string newConversationString = "";
                while (reader.Read())
                {
                    speaker = getReaderString(0);
                    newConversationString = getReaderString(1);
                }

                SingleConversation newConversation = new SingleConversation();
                newConversation.speaker = (speaker == "Player") ? "Player" : characterName;
                newConversation.spoken = (speaker == "Player") ? characterName : "Player";
                newConversation.conversation = newConversationString;
                newConversation.options = (newPrerequisiteString[0] == 'C') ? true : false;
                newConversation.response = new string[4];

                return newConversation;
            }

            internal static void RetrieveConversationForCharacter(object character, ConversationState state)
            {
                throw new NotImplementedException();
            }
            
            public static SingleConversation Continue(string characterName, int playerChoice, bool openChoice)
            {
                char playerChoiceChar = (char)(playerChoice + 64);
                //0. Start Database Connection
                StartDatabase(conversations);

                //1a. Get Conversation Data (Single Response)
                string Source = ""; string[] Conversation = new string[4]; string[] addPrerequisites = new string[4];

                int prereqCount = 0;
                if (!openChoice)
                {
                    if (playerChoice != 0)
                    {
                        for (int i = 0; i < characterLimit; i++)
                        {
                            if (prerequisites[i])
                            {
                                int a = ExecuteSQLQueryInt("select COUNT(*) from " + characterName + " where \"Order\" is " + i +
                                    " and \"Prerequisites\" like '%" + playerChoiceChar + "'", Database.Conversation, 0);
                                if (a == 0) continue;
                                ExecuteSQLQuery("select \"Source\", \"Conversation\", \"Add Prerequisite\" from " + characterName + " where \"Order\" is " + i +
                                    " and \"Prerequisites\" like '%" + playerChoiceChar + "'");

                                while (reader.Read())
                                {
                                    Source = getReaderString(0);
                                    Conversation[0] = getReaderString(1);
                                    addPrerequisites[0] = getReaderString(2);
                                }
                                prerequisites[i] = false;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < characterLimit; i++)
                        {
                            if (prerequisites[i])
                            {
                                ExecuteSQLQuery("select \"Source\", \"Conversation\", \"Add Prerequisite\" from " + characterName + " where \"Order\" is " + i);

                                while (reader.Read())
                                {
                                    Source = getReaderString(0);
                                    Conversation[0] = getReaderString(1);
                                    addPrerequisites[0] = getReaderString(2);
                                }

                                prerequisites[i] = false;
                            }
                        }
                    }
                }

                //1a. Get Conversation Data (Multiple Response)
                else
                {
                    for (int i = 0; i < characterLimit; i++)
                    {
                        if (prerequisites[i])
                        {
                            ExecuteSQLQuery("select \"Source\", \"Conversation\", \"Add Prerequisite\" from " + characterName + " where \"Order\" is " + i);
                            while (reader.Read())
                            {
                                Source = getReaderString(0);
                                Conversation[prereqCount] = getReaderString(1);
                                addPrerequisites[prereqCount] = getReaderString(2);
                            }
                            prereqCount++;
                            prerequisites[i] = false;
                        }
                    }
                }

                //3. Set all possible paths to have +ve boolean flags
                List<string> newPrerequisites = new List<string>();

                Debug.Log(Conversation[0]);
                for (int i = 0; i < 4; i++)
                {
                    if (addPrerequisites[i] != "" && addPrerequisites[i] != null)
                        newPrerequisites.AddRange(StringToStringList(addPrerequisites[i]));
                }

                foreach (string s in newPrerequisites)
                {
                    ExecuteSQLQuery("select \"Order\" from " + characterName + " where Prerequisites like '" + s + "'");
                    reader.Read();
                    int i = reader.GetInt32(0);
                    prerequisites[i] = true;
                }

                //4. Return that conversation
                SingleConversation newConversation = new SingleConversation();
                newConversation.speaker = (Source == "Player") ? "Player" : characterName;
                newConversation.spoken = (Source == "Player") ? characterName : "Player";
                newConversation.conversation = Conversation[0];
                for (int i = 0; i < 4; i++)
                {
                    if (addPrerequisites[i] == null || addPrerequisites[i] == "")
                        continue;
                    else
                        newConversation.options = (addPrerequisites[i][0] == 'C') ? true : false;
                }

                newConversation.response = Conversation;

                return newConversation;
            }

            public static SingleConversation FinishResponse(SingleConversation convo, string characterName, int choice)
            {
                // Remove the boolean flags for everything other than the choice

                string preReq = "";
                for (int i = 0; i < GameSettings.conversationCharacterLimit; i++)
                {
                    if (prerequisites[i])
                    {
                        ExecuteSQLQuery("select \"Prerequisites\" from " + characterName + " where \"Order\" is " + i);
                        while (reader.Read())
                        {
                            preReq = getReaderString(0);
                            if ((int)(getLastCharInString(preReq)) == 'A' + ((choice - 1)) || (int)getLastCharInString(preReq) == ('a' + (choice - 1)))
                                continue;
                            else
                            {
                                prerequisites[i] = false;
                            }
                        }
                    }
                }
                var newConvo = new SingleConversation(
                    "Player",
                    characterName,
                    convo.response[choice - 1],
                    false,
                    null
                );
                return newConvo;
            }

            public static SingleConversation ReadNext(string characterName, int playerChoice, bool openChoice)
            { //If player choice is 0, then was not a choice response
                char playerChoiceChar = (char)(playerChoice + 64);
                //0. Start Database Connection
                StartDatabase(conversations);

                //1a. Get Conversation Data (Single Response)
                string Source = ""; string[] Conversation = new string[4]; string[] addPrerequisites = new string[4];

                if (!openChoice)
                {
                    if (playerChoice != 0)
                    {
                        for (int i = 0; i < characterLimit; i++)
                        {
                            if (prerequisites[i])
                            {
                                int a = ExecuteSQLQueryInt("select COUNT(*) from " + characterName + " where \"Order\" is " + i +
                                    " and \"Prerequisites\" like '%" + playerChoiceChar + "'", Database.Conversation, 0);
                                if (a == 0) continue;
                                ExecuteSQLQuery("select \"Source\", \"Conversation\", \"Add Prerequisite\" from " + characterName + " where \"Order\" is " + i +
                                    " and \"Prerequisites\" like '%" + playerChoiceChar + "'");

                                while (reader.Read())
                                {
                                    Source = getReaderString(0);
                                    Conversation[0] = getReaderString(1);
                                    addPrerequisites[0] = getReaderString(2);
                                }
                                prerequisites[i] = false;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < characterLimit; i++)
                        {
                            if (prerequisites[i])
                            {
                                ExecuteSQLQuery("select \"Source\", \"Conversation\", \"Add Prerequisite\" from " + characterName + " where \"Order\" is " + i);

                                while (reader.Read())
                                {
                                    Source = getReaderString(0);
                                    Conversation[0] = getReaderString(1);
                                    addPrerequisites[0] = getReaderString(2);
                                }

                                prerequisites[i] = false;
                            }
                        }
                    }
                }

                //1a. Get Conversation Data (Multiple Response)
                else
                {
                    int j = 0;
                    for (int i = 0; i < characterLimit; i++)
                    {
                        if (prerequisites[i])
                        {
                            ExecuteSQLQuery("select \"Source\", \"Conversation\", \"Add Prerequisite\" from " + characterName + " where \"Order\" is " + i);
                            while (reader.Read())
                            {
                                Source = getReaderString(0);
                                Conversation[j] = getReaderString(1);
                                addPrerequisites[j] = getReaderString(2);
                            }
                            j++;
                            prerequisites[i] = false;
                        }
                    }
                }

                //3. Return that conversation
                SingleConversation newConversation = new SingleConversation();
                newConversation.speaker = (Source == "Player") ? "Player" : characterName;
                newConversation.spoken = (Source == "Player") ? characterName : "Player";
                newConversation.conversation = Conversation[0];
                for (int i = 0; i < 4; i++)
                {
                    if (addPrerequisites[i] == null || addPrerequisites[i] == "")
                        continue;
                    else
                        newConversation.options = (addPrerequisites[i][0] == 'C') ? true : false;
                }

                newConversation.response = Conversation;

                return newConversation;
            }

            public static bool SetNewPrerequisites(string characterName)
            {
                return false;
            }

            public static bool CheckIfConverationHasEnded(string characterName)
            {
                for (int i = 0; i < characterLimit; i++)
                {
                    if (prerequisites[i])
                    {

                        ExecuteSQLQuery("select \"End Type (0/1)\" from " + characterName + " where \"Order\" like " + i);
                        reader.Read();
                        int v = reader.GetInt32(0);
                        if (v == 1)
                        {
                            Debug.Log("CONVO: " + i + " ends the conversation");
                            return true;
                        }
                        else
                            return false;
                    }
                }
                return false;
            }

            public static void PrintPrerequisites()
            {
                return;

                int pile = 0;
                string boolstring = "";
                for (int i = 1; i < characterLimit; i++)
                {
                    if (prerequisites[i])
                    {
                        pile = 0;
                        boolstring += "1";
                    }
                    else
                    {
                        pile++;
                        boolstring += "0";
                    }
                    if (pile > 10)
                        break;
                }
                Debug.Log("BOOL FLAGS: " + boolstring);
            }
        }
    }
}