using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Objects.Movable.Characters;
using Objects.Movable.Characters.Individuals;
using Data.Database;
using System.Data;
using Mono.Data.SqliteClient;

namespace Conversations
{

    /// <summary>
    /// A single conversation, a simple statement said by a single person
    /// </summary>
    public partial class ConversationState
    {
        // From the database information
        public string stateName;
        public string[] addStates;
        public Objects.Movable.Characters.CharacterController currentSpeaker;
        public string dialogue;

        public delegate void Action();
        //public event Action action;

        public string addEvents;
        public string removeEvents;
        public string requireEvents;

        private int NextStateCount
        {
            get
            {
                if (conversationViewStatus == ConversationViewStatus.PlayerMultipleReponse)
                    return previousState.addStates.Length + 1;
                else return 1;
            }
        }

        public bool InputIsValid(int input)
        {
            if (conversationViewStatus == ConversationViewStatus.PlayerMultipleReponse)
                if (input > 0 && input < NextStateCount) return true;
            return false;
        }

        public void Print()
        {
            string stateInfo = "";

            // Basic Info
            stateInfo += "State: " + stateName + "\n";
            stateInfo += "Speaker: " + currentSpeaker + "\n";
            stateInfo += "Next States: " + "\n";

            if(addStates != null)
                foreach (var nextState in addStates)
                    stateInfo += "  States: " + nextState + "\n";

            // Dialogue
            stateInfo += "Dialogue: " + dialogue + "\n";

            // Event Info
            stateInfo += "Events: " + "\n";
            stateInfo += "  Add Events: " + "\n";
            foreach (var addEvent in addEvents)
                stateInfo += "  " + addEvents + "\n";
            stateInfo += "  Remove Events: " + "\n";
            foreach (var addEvent in addEvents)
                stateInfo += "  " + removeEvents + "\n";
            stateInfo += "  Require Events: " + "\n";
            foreach (var addEvent in addEvents)
                stateInfo += "  " + requireEvents + "\n";

            Debug.Log(stateInfo);
        }
    }
}
