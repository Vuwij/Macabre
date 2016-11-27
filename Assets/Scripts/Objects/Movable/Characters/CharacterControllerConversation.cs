using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Conversations;
using Data;
using Data.Database;

namespace Objects.Movable.Characters
{
    public abstract partial class CharacterController : MovingObjectController
    {
        public static ConversationState conversationState;

        // Invoked everytime when the spacebar is pressed or an decision is made
        public ConversationState Dialogue(int decision = 0)
        {
            // Check if the character exists in database
            if (conversationState == null) conversationState = new ConversationState(this);
            else conversationState = conversationState.GetNextState(decision);

            ConversationState.DisplayState(conversationState);
            
            return conversationState;
        }
    }
}