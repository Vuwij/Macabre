using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exceptions;

using Objects.Movable.Characters;
using Data.Database;

namespace Conversations
{
    // Contains the current state and controller to the next state
    public partial class ConversationState
    {
        // The characters and character controller remains the same for each conversation
        public CharacterController characterController;
        private Character character
        {
            get { return characterController.character; }
        }

        // A single previous State
        private ConversationState previousState;

        // A tree of next states
        private List<ConversationState> nextStates
        {
            get
            {
                List<ConversationState> next = new List<ConversationState>();
                if (addStates == null) return next;

                foreach(string stateName in addStates)
                {
                    ConversationState s = new ConversationState(characterController, this, stateName);
                    next.Add(s);
                }
                return next;
            }
        }
        
        // Creates a conversation for the speaker
        public ConversationState(CharacterController speakerController, ConversationState previousState = null, string stateName = "")
        {
            this.characterController = speakerController;
            this.previousState = previousState;
            if (previousState == null)
                DatabaseManager.Conversation.FindAndUpdateConversationForCharacter(character, this);
            else
                DatabaseManager.Conversation.UpdateConversationForCharacter(stateName, character, this);

            SetCurrentViewFromPreviousState(previousState);
        }

        // Update the next state and speaker with the current state
        public ConversationState GetNextState(int decision = 0)
        {
            if (nextStates.Count == 0) return null;
            if (conversationViewStatus == ConversationViewStatus.PlayerMultipleReponse)
            {
                ConversationState c = previousState.nextStates[decision];
                c.conversationViewStatus = ConversationViewStatus.PlayerResponse;
                return c;
            }
            else
                return nextStates[decision];
        }
    }
}