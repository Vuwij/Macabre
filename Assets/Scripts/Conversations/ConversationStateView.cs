using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Objects.Movable.Characters;
using Objects.Movable.Characters.Individuals;
using Data.Database;

namespace Conversations
{
    /// <summary>
    /// The current state of the conversation, contains the current state and next state and previous state
    /// </summary>
    public partial class ConversationState
    {
        private ConversationViewStatus conversationViewStatus = ConversationViewStatus.Empty;
        
        private void SetCurrentViewFromPreviousState(ConversationState previousState)
        {
            if (previousState == null) return;
            if (previousState.addStates.Count() == 0) conversationViewStatus = ConversationViewStatus.Empty;
            if (previousState.addStates.Count() > 1) conversationViewStatus = ConversationViewStatus.PlayerMultipleReponse;
            if (previousState.currentSpeaker.character is Player) conversationViewStatus = ConversationViewStatus.CharacterResponse;
            conversationViewStatus = ConversationViewStatus.PlayerReponse;
        }

        private void DisplayNextState()
        {

        }
    }
}
