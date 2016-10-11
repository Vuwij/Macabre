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
        //S = Start, R = Character Response, C = Player Closed Response, O = Player Open Response, E = End Conversation
        ConversationStatus CONVO_status_next = ConversationStatus.Start;
        ConversationStatus CONVO_status_current;

        private CharacterController characterSpokenTo;
        SingleConversation convo = new SingleConversation();
        string selectedResponse;
        
        // TODO enable multiple character conversation
        // Starts a conversation between the character and the player
        public void StartConversation()
        {
            if (!DatabaseManager.Conversation.TestCharacterConversation(this.name)) return;
            
            LockMovement();
            player.LockMovement();

            Dialogue();
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


        public void Dialogue(int decision = 0)
        {
            // If current & next = END, then next = start
            if (CONVO_status_current == ConversationStatus.EndConversation &&
                CONVO_status_next == ConversationStatus.EndConversation)
            {
                CONVO_status_next = ConversationStatus.Start;

                UIManager.main.CloseDialogueScreen();
                player.UnlockMovement();
                UnlockMovement();
                Debug.Log("5. Conversation Ended");
                return;
            }

            // If it is a character reponse then close dialogue screen and continue
            if ((CONVO_status_current == ConversationStatus.CharacterResponse && CONVO_status_next != ConversationStatus.EndConversation) ||
                (CONVO_status_current == ConversationStatus.PlayerClosedResponse && CONVO_status_next != ConversationStatus.EndConversation)
            )
            {
                if (DatabaseManager.main.CONVO_checkEndConversations(name))
                {
                    CONVO_status_next = ConversationStatus.EndConversation;

                }
            }

            /* Clears the dialogue */
            ClearDialogue();

            /* Retrieves the correct message */
            if (CONVO_status_next == ConversationStatus.Start)
            {
                convo = DatabaseManager.main.CONVO_start(name);
            }
            if (CONVO_status_next == ConversationStatus.PlayerOpenResponse)
            {
                convo = DatabaseManager.main.CONVO_continue(name, decision, true);  // Decision goes here
            }
            if (CONVO_status_next == ConversationStatus.PlayerClosedResponse)
            {
                if (CONVO_status_current == ConversationStatus.PlayerOpenResponse)
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
            if (CONVO_status_next == ConversationStatus.CharacterResponse)
            {
                convo = DatabaseManager.main.CONVO_continue(name, decision, true);  // Decision goes here
            }
            if (CONVO_status_next == ConversationStatus.EndConversation)
            {
                convo = DatabaseManager.main.CONVO_continue(name, decision, true);  // Decision goes here
            }

            /* Displays the correct message */
            convo.PrintConversation();
            if (CONVO_status_next == ConversationStatus.PlayerOpenResponse)
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
            if (CONVO_status_next == ConversationStatus.PlayerClosedResponse)
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
            if (CONVO_status_next == ConversationStatus.Start)
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
            if (CONVO_status_next == ConversationStatus.CharacterResponse)
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
            if (CONVO_status_next == ConversationStatus.EndConversation)
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

            CONVO_status_current = CONVO_status_next;

            /* Checks the next conversation */
            if (CONVO_status_current == ConversationStatus.Start)
            {
                if (convo.speaker == "Player")
                {
                    CONVO_status_next = ConversationStatus.CharacterResponse;
                }
                else
                {
                    if (convo.options == true)
                    {
                        CONVO_status_next = ConversationStatus.PlayerOpenResponse;
                    }
                    else
                    {
                        CONVO_status_next = ConversationStatus.PlayerClosedResponse;
                    }
                }
            }
            if (CONVO_status_current == ConversationStatus.CharacterResponse)
            {
                if (convo.speaker == "Player")
                {
                    CONVO_status_next = ConversationStatus.CharacterResponse;
                }
                else
                {
                    if (convo.options == true)
                    {
                        CONVO_status_next = ConversationStatus.PlayerOpenResponse;
                    }
                    else
                    {
                        CONVO_status_next = ConversationStatus.PlayerClosedResponse;
                    }
                }
            }
            if (CONVO_status_current == ConversationStatus.EndConversation)
            {
                CONVO_status_next = ConversationStatus.EndConversation;
            }
            if (CONVO_status_current == ConversationStatus.PlayerClosedResponse)
            {
                if (convo.speaker == "Player")
                {
                    CONVO_status_next = ConversationStatus.CharacterResponse;
                }
                else
                {
                    if (convo.options == true)
                    {
                        CONVO_status_next = ConversationStatus.PlayerOpenResponse;
                    }
                    else
                    {
                        CONVO_status_next = ConversationStatus.PlayerClosedResponse;
                    }
                }
            }
            if (CONVO_status_current == ConversationStatus.PlayerOpenResponse)
            {
                CONVO_status_next = ConversationStatus.PlayerClosedResponse;
            }

            DatabaseManager.main.CONVO_printAllPrerequisites();

        }
    }
}