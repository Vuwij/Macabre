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
        
    }
}