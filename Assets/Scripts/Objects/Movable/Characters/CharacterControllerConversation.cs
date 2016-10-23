using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Conversations;
using Data;
using Data.Database;

// TODO Fix all the conversation problems
namespace Objects.Movable.Characters
{
    public abstract partial class CharacterController : MovingObjectController
    {
        private ConversationState conversationState;

        // Invoked everytime when the spacebar is pressed or an decision is made
        public void Dialogue(int decision = 0)
        {
            if (conversationState == null) conversationState = new ConversationState(this);
            conversationState = conversationState.GetNextStateAndDisplay(decision);
        }
    }
}