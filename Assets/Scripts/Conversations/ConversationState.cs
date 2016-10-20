using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Objects.Movable.Characters;
using Data.Database;

namespace Conversations
{
    // Contains the current state and controller to the next state
    public partial class ConversationState
    {
        // The characters and character controller remains the same for each conversation
        private CharacterController characterController;
        private Character character
        {
            get { return characterController.character; }
        }

        // A tree of next states
        private ConversationState previousState;
        private List<ConversationState> nextStates
        {
            get
            {
                List<ConversationState> next = new List<ConversationState>();
                foreach(string stateName in addStates)
                {
                    ConversationState s = new ConversationState(characterController);
                    DatabaseManager.Conversation.UpdateConversationForCharacter(stateName, character, s);
                    next.Add(s);
                }
                return next;
            }
        }
        
        // Creates a conversation for the speaker
        public ConversationState(CharacterController speakerController, ConversationState previousState = null)
        {
            this.characterController = speakerController;
            DatabaseManager.Conversation.FindAndUpdateConversationForCharacter(character, this);
            SetCurrentViewFromPreviousState(previousState);
        }

        // Update the next state and speaker with the current state
        public ConversationState GetNextStateAndDisplay(int decision = 0)
        {
            return nextStates[decision];
            DisplayNextState();
        }
    }
}
}
