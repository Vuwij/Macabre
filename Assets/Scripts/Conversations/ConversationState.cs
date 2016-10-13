using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Objects.Movable.Characters;
using Data.Database;

namespace Conversations
{
    /// <summary>
    /// The current state of the conversation, contains the current state and next state and previous state
    /// </summary>
    public class ConversationState
    {
        public ConversationViewStatus previousState = ConversationViewStatus.Empty;
        public ConversationViewStatus currentState = ConversationViewStatus.Empty;
        public ConversationViewStatus nextState = ConversationViewStatus.Empty;

        public CharacterController currentSpeaker;  // The current speaker
        public CharacterController nextSpeaker;
        public CharacterController[] speakers;    // Multiple people can be in the same conversation
        public CharacterController primarySpeaker
        {
            get { return speakers[0]; }
        }

        // Creates a conversation for the speaker
        public ConversationState(CharacterController speaker)
        {
            speakers = new CharacterController[1];
            speakers[0] = speaker;
            InitializeConversationFromDatabase();
        }

        // Initializing the conversation from information from the database
        private void InitializeConversationFromDatabase()
        {
            DatabaseManager.Conversation.RetrieveConversationForCharacter(primarySpeaker, this);
        }

        // Finds out the next state
        public void DetermineNextState()
        {

        }

        // Update the next state and speaker with the current state
        public void UpdateNextState()
        {
            currentState = nextState;
            previousState = currentState;
            currentSpeaker = nextSpeaker;
        }
        
        private void LockAllCharacterPosition()
        {
            foreach (CharacterController character in speakers)
                character.LockMovement();
        }

        private void UnlockAllCharacterPosition()
        {
            foreach (CharacterController character in speakers)
                character.UnlockMovement();
        }
        
        // TODO 
        private void PrintCurrentState()
        {

        }
    }
}
