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
            // Starts a new conversation
            if (conversationState == null)
                DialogueStart();
             

            // If it is a character reponse then close dialogue screen and continue
            if ((currentConversationState == ConversationViewStatus.CharacterResponse && nextConversationState != ConversationViewStatus.End) ||
                (currentConversationState == ConversationViewStatus.PlayerClosedResponse && nextConversationState != ConversationViewStatus.End)
            )
            {
                if (DatabaseManager.Conversation.CheckIfConverationHasEnded(name))
                {
                    nextConversationState = ConversationViewStatus.End;

                }
            }

            /* Clears the dialogue */
            ClearDialogue();

            /* Retrieves the correct message */
            if (nextConversationState == ConversationViewStatus.Empty)
            {
                convo = DatabaseManager.main.CONVO_start(name);
            }
            if (nextConversationState == ConversationViewStatus.PlayerMultipleReponse)
            {
                convo = DatabaseManager.main.CONVO_continue(name, decision, true);  // Decision goes here
            }
            if (nextConversationState == ConversationViewStatus.PlayerClosedResponse)
            {
                if (currentConversationState == ConversationViewStatus.PlayerMultipleReponse)
                {
                    convo = DatabaseManager.main.CONVO_finishresponse(convo, name, decision);
                    if (Settings.debugCONVO)
                        Debug.Log("5. Last Decision is " + decision);
                }
                else
                {
                    convo = DatabaseManager.main.CONVO_continue(name, decision, true);  // Decision goes here
                }
            }
            DatabaseManager.main.CONVO_printAllPrerequisites();
            if (nextConversationState == ConversationViewStatus.CharacterResponse)
            {
                convo = DatabaseManager.main.CONVO_continue(name, decision, true);  // Decision goes here
            }
            if (nextConversationState == ConversationViewStatus.End)
            {
                convo = DatabaseManager.main.CONVO_continue(name, decision, true);  // Decision goes here
            }

            /* Displays the correct message */
            convo.PrintConversation();
            if (nextConversationState == ConversationViewStatus.PlayerMultipleReponse)
            {
                InputManager.LockInspection();
                InputManager.LockPendingDecision();
                UIManager.main.DisplayMultipleResponse(
                    GameObject.FindGameObjectWithTag("PlayerSprite").GetComponentInChildren<SpriteRenderer>().sprite,
                    convo.speaker,                  // string characterName,
                    convo.response,                 // string textEntry,
                    true                            // bool continueText
                );
            }
            if (nextConversationState == ConversationViewStatus.PlayerClosedResponse)
            {
                InputManager.UnlockInspection();
                InputManager.UnlockPendingDecision();
                UIManager.main.DisplaySingleResponse(
                    GameObject.FindGameObjectWithTag("PlayerSprite").GetComponentInChildren<SpriteRenderer>().sprite,
                    convo.speaker,                  // string characterName,
                    convo.conversation,             // string textEntry,
                    true                            // bool continueText
                );
            }
            if (nextConversationState == ConversationViewStatus.Empty)
            {
                UIManager.main.DisplaySingleResponse(
                    (convo.speaker == "Player") ?   // Sprite mainImageEntry,
                    GameObject.FindGameObjectWithTag("PlayerSprite").GetComponentInChildren<SpriteRenderer>().sprite :
                    this.GetComponentInChildren<SpriteRenderer>().sprite,
                    convo.speaker,                  // string characterName,
                    convo.conversation,             // string textEntry,
                    true                            // bool continueText
                );
            }
            if (nextConversationState == ConversationViewStatus.CharacterResponse)
            {
                InputManager.UnlockInspection();
                InputManager.UnlockPendingDecision();
                UIManager.main.DisplayCharacterText(
                    this.GetComponentInChildren<SpriteRenderer>().sprite,
                    convo.speaker,                  // string characterName,
                    convo.conversation,             // string textEntry,
                    true                            // bool continueText
                );
            }
            if (nextConversationState == ConversationViewStatus.End)
            {
                UIManager.main.DisplaySingleResponse(
                    (convo.speaker == "Player") ?   // Sprite mainImageEntry,
                    GameObject.FindGameObjectWithTag("PlayerSprite").GetComponentInChildren<SpriteRenderer>().sprite :
                    this.GetComponentInChildren<SpriteRenderer>().sprite,
                    convo.speaker,                  // string characterName,
                    convo.conversation,             // string textEntry,
                    true                            // bool continueText
                );
            }

            currentConversationState = nextConversationState;

            /* Checks the next conversation */
            if (currentConversationState == ConversationViewStatus.Empty)
            {
                if (convo.speaker == "Player")
                {
                    nextConversationState = ConversationViewStatus.CharacterResponse;
                }
                else
                {
                    if (convo.options == true)
                    {
                        nextConversationState = ConversationViewStatus.PlayerMultipleReponse;
                    }
                    else
                    {
                        nextConversationState = ConversationViewStatus.PlayerClosedResponse;
                    }
                }
            }
            if (currentConversationState == ConversationViewStatus.CharacterResponse)
            {
                if (convo.speaker == "Player")
                {
                    nextConversationState = ConversationViewStatus.CharacterResponse;
                }
                else
                {
                    if (convo.options == true)
                    {
                        nextConversationState = ConversationViewStatus.PlayerMultipleReponse;
                    }
                    else
                    {
                        nextConversationState = ConversationViewStatus.PlayerClosedResponse;
                    }
                }
            }
            if (currentConversationState == ConversationViewStatus.End)
            {
                nextConversationState = ConversationViewStatus.End;
            }
            if (currentConversationState == ConversationViewStatus.PlayerClosedResponse)
            {
                if (convo.speaker == "Player")
                {
                    nextConversationState = ConversationViewStatus.CharacterResponse;
                }
                else
                {
                    if (convo.options == true)
                    {
                        nextConversationState = ConversationViewStatus.PlayerMultipleReponse;
                    }
                    else
                    {
                        nextConversationState = ConversationViewStatus.PlayerClosedResponse;
                    }
                }
            }
            if (currentConversationState == ConversationViewStatus.PlayerMultipleReponse)
            {
                nextConversationState = ConversationViewStatus.PlayerClosedResponse;
            }

            DatabaseManager.main.CONVO_printAllPrerequisites();

        }

        public void DialogueStart()
        {
            conversationState = new ConversationState(this);
            if (!DatabaseManager.Conversation.TestIfCharacterExistsInDatabase(this.name)) return;



            // If current & next = END, then next = start
            if (currentConversationState == ConversationViewStatus.End &&
                nextConversationState == ConversationViewStatus.End)
            {
                nextConversationState = ConversationViewStatus.Empty;

                UIManager.CloseDialogueScreen();
                player.UnlockMovement();
                UnlockMovement();
                Debug.Log("5. Conversation Ended");
                return;
            }

        }
        
        public void FindCharacterAndContinueConversation(int choice)
        {
            characterSpokenTo.SendMessage("DialogueDecision", choice);
        }

        /// <summary>
        /// Chooses a decision, can be 1, 2, 3, 4 and outcome is dependent
        /// </summary>
        /// <param name="decision">Can only be 1, 2, 3, 4</param>
        public void DialogueDecision(int decision)
        {
            Debug.Log("Player decision is " + decision);

            // Throws an exception if the decision is out of scope
            if (decision >= 5 || decision <= 0)
                throw new UnityException("Decision out of scope, valid decisions 1, 2, 3, 4");
            
            // Finds the closest character to speak to
            characterSpokenTo = GetNearestMacabreObject<CharacterController>();

            if (characterSpokenTo == null)
                throw new UnityException("Error: Character to reply not found");

            if (characterSpokenTo.convo.response[decision - 1] == "" || characterSpokenTo.convo.response[decision - 1] == null)
                throw new UnityException((decision).ToString() + " is not an option");

            InputManager.UnlockPendingDecision();
            characterSpokenTo.Dialogue(decision);
        }
        
    }
}