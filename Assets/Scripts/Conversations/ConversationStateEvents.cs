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
    public partial class ConversationState
    {
        private List<CharacterController> AllCharactersInConversation
        {
            get { return FindAllCharactersInConversation(this).ToList(); }
        }
        
        private IEnumerable<CharacterController> FindAllCharactersInConversation(ConversationState root)
        {
            yield return currentSpeaker;
            foreach (ConversationState next in root.nextStates)
                foreach (var x in FindAllCharactersInConversation(next))
                    yield return x;
        }

        private void LockAllCharacterPosition()
        {
            foreach (CharacterController character in AllCharactersInConversation)
                character.LockMovement();
        }

        private void UnlockAllCharacterPosition()
        {
            foreach (CharacterController character in AllCharactersInConversation)
                character.UnlockMovement();
        }
    }
}
