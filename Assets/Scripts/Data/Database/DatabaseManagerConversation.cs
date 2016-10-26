using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using System.Linq;
using Mono.Data.SqliteClient;

using Conversations;
using Objects.Movable.Characters;

/**
 * State Logic
 *  StateName       The current conversation state
 *  AddStates       A list of next states, if there is more than one, it is a open response
 * 
 * State Properties
 *  CurrentSpeaker  The current speaker in the context
 *  Dialogue        What that current speaker is saying
 *  Action          What action calls are invoked in the conversation
 * 
 * Events
 *  AddEvent        Add a state in the event property list, a list of all states that exist
 *  RemoveEvent     Remove a state in the event property list
 *  RequireEvent    Require Event for this conversation to be enabled
 */

namespace Data.Database {

    /// <summary>
    /// Partial class for the Database Manager - Conversation
    /// </summary>
    public static partial class DatabaseManager
    {
        public static class Conversation
        {
            // Prerequisites for a single conversation, 0 is always empty
            public static int characterLimit { get { return GameSettings.conversationCharacterLimit; } }

            // Finds the single response for a character and responds with a singleConversation
            public static void FindAndUpdateConversationForCharacter(Character characterTable, ConversationState conversationStateToUpdate)
            {
                string table = "Conversations_" + characterTable.name;

                // Select all conversations with no prerequisites
                ExecuteSQLQuery("SELECT * FROM " + table + @"WHERE StateName LIKE 'S%' OR StateName LIKE 's%'");

                // Fill in the information with the reader row
                UpdateStateWithRow(reader, conversationStateToUpdate);
            }

            // When you found a response, just update the table
            public static void UpdateConversationForCharacter(string stateName, Character characterTable, ConversationState conversationStateToUpdate)
            {
                string table = "Conversations_" + characterTable.name;

                // Execute the query
                ExecuteSQLQuery("SELECT * FROM " + table + @"WHERE StateName LIKE '" + stateName + "'");

                // Fill in the information with the reader row
                UpdateStateWithRow(reader, conversationStateToUpdate);
            }

            private static void UpdateStateWithRow(IDataReader reader, ConversationState s)
            {
                reader.Read();
                s.stateName = reader.GetString(0);
                s.addStates = DatabaseManager.Utility.StringToStringList(reader.GetString(1));
                s.currentSpeaker = MacabreWorld.current.characterControllers.Find(x => x.character.name == reader.GetString(2));
                s.dialogue = reader.GetString(3);

                // TODO Link action with string
                //ParseActionString(reader.GetString(4));

                s.addEvents = reader.GetString(5);
                s.removeEvents = reader.GetString(6);
                s.requireEvents = reader.GetString(7);
            }
        }
    }
}