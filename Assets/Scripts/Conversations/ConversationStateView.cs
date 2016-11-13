using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Objects.Movable.Characters;
using Objects.Movable.Characters.Individuals;
using Data.Database;
using UnityEngine;
using UI;
using UI.Dialogues;

namespace Conversations
{
    /// <summary>
    /// The current state of the conversation, contains the current state and next state and previous state
    /// </summary>
    public partial class ConversationState
    {
        public ConversationViewStatus conversationViewStatus = ConversationViewStatus.Empty;
        
        private void SetCurrentViewFromPreviousState(ConversationState previousState)
        {
            if (previousState == null)
            {
                if (currentSpeaker.character is Player) conversationViewStatus = ConversationViewStatus.PlayerResponse;
                else conversationViewStatus = ConversationViewStatus.CharacterResponse;
            }
            else conversationViewStatus = IdentifyCurrentViewFromPreviousState(previousState);
        }

        private ConversationViewStatus IdentifyCurrentViewFromPreviousState(ConversationState previousState)
        {
            if (previousState.addStates.Count() == 0) return ConversationViewStatus.Empty;
            if (previousState.addStates.Count() > 1) return ConversationViewStatus.PlayerMultipleReponse;
            if (previousState.currentSpeaker is PlayerController)
                return ConversationViewStatus.CharacterResponse;
            else return ConversationViewStatus.PlayerResponse;
        }

        public static void DisplayState(ConversationState convo)
        {
            var conversationDialogue = UIManager.Find<ConversationDialogue>();

            if (convo == null)
            {
                conversationDialogue.TurnOff();
                return;
            }
            else conversationDialogue.TurnOn();

            conversationDialogue.Reset();
            switch (convo.conversationViewStatus)
            {
                case ConversationViewStatus.PlayerMultipleReponse:
                    conversationDialogue.mainImage = convo.currentSpeaker.GetComponentInChildren<SpriteRenderer>().sprite;
                    conversationDialogue.responseTexts = (from state in convo.previousState.nextStates
                                                          select state.dialogue).ToArray();
                    conversationDialogue.continueText = "";
                    return;
                case ConversationViewStatus.CharacterResponse:
                case ConversationViewStatus.PlayerResponse:
                    conversationDialogue.mainImage = convo.currentSpeaker.GetComponentInChildren<SpriteRenderer>().sprite;
                    conversationDialogue.titleText = convo.currentSpeaker.name;
                    conversationDialogue.mainText = convo.dialogue;
                    conversationDialogue.continueText = "Space to Continue";
                    return;
                case ConversationViewStatus.Empty:
                default:
                    return;
            }
        }
    }
}
