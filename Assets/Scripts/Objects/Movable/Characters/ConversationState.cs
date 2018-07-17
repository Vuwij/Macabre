using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data.Databases;
using Objects.Movable.Characters;
using Objects.Movable.Characters.Individuals;
using UI;
using UI.Dialogues;
using UnityEngine;

namespace Objects.Movable.Characters
{
    public class ConversationState
    {
        public Character character;

        public string stateName;
        public List<ConversationState> nextStates = new List<ConversationState>();
        public string updateCondition;
        public string requireCondition;
        public Character speaker;
        public string dialogue;
        public List<string> conversationActions = new List<string>();

        ConversationDialogue conversationDialogue
        {
            get
            {
                UIScreenManager screenManager = UnityEngine.Object.FindObjectOfType<UIScreenManager>();
                ConversationDialogue c = screenManager.GetComponentInChildren<ConversationDialogue>(true);
                Debug.Assert(c != null);
                return c;
            }
        }

        public List<ConversationState> enabledNextStates {
            get {
                List<ConversationState> conversationStates = new List<ConversationState>();
                foreach(ConversationState c in nextStates) {
                    Dictionary<string, string> conditionList = ParseConditionList(c.requireCondition);

                    bool passConditions = true;
                    foreach(KeyValuePair<string, string> kvp in conditionList) {
                        // Check Temporary Events
                        if(!kvp.Key.Contains("Game.")) {
                            if (character.characterEvents.ContainsKey(kvp.Key)) {
                                if (character.characterEvents[kvp.Key] != kvp.Value) {
                                    passConditions = false;
                                    continue;
                                }
                            }
                        }
                        else {
                            GameManager gameManager = UnityEngine.Object.FindObjectOfType<GameManager>();
                            Debug.Assert(gameManager != null);

                            string gameKey = kvp.Key.Replace("Game.", "");

                            if (gameManager.gameEvents.ContainsKey(gameKey)) {
                                if (gameManager.gameEvents[gameKey] != kvp.Value) {
                                    passConditions = false;
                                    continue;
                                }
                            }
                        }
                    }

                    if (passConditions)
                        conversationStates.Add(c);
                }

                return conversationStates;
            }
        }
        
        Dictionary<string, string> ParseConditionList(string s) {
            Dictionary<string, string> conditionList = new Dictionary<string, string>();

            string[] conditions = s.Split('\n');
            if (conditions[0] == "")
                return conditionList;
            
            foreach(var cond in conditions) {
                string c = cond.Replace(" ", "");
                Debug.Assert(c.Contains("="));
                string[] statement = c.Split('=');
                Debug.Assert(statement.Length == 2);
                conditionList.Add(statement[0], statement[1]);
            }

            return conditionList;
        }
        
        public void UpdateConversationConditions() {
            Dictionary<string, string> conditionList = ParseConditionList(updateCondition);
            foreach (KeyValuePair<string, string> kvp in conditionList)
            {
                // Check Temporary Events
                if (!kvp.Key.Contains("Game.")) {
                    if (character.characterEvents.ContainsKey(kvp.Key))
                        character.characterEvents[kvp.Key] = kvp.Value;
                    else
                        character.characterEvents.Add(kvp.Key, kvp.Value);
                }
                else
                {
                    GameManager gameManager = UnityEngine.Object.FindObjectOfType<GameManager>();
                    Debug.Assert(gameManager != null);

                    string gameKey = kvp.Key.Replace("Game.", "");
                    if (gameManager.gameEvents.ContainsKey(gameKey))
                        gameManager.gameEvents[gameKey] = kvp.Value;
                    else
                        gameManager.gameEvents.Add(gameKey, kvp.Value);
                }
            }
        }

        public void DisplayCurrent()
        {
            conversationDialogue.Reset();
            conversationDialogue.gameObject.SetActive(true);

            SpriteRenderer spriteRenderer = speaker.GetComponentInChildren<SpriteRenderer>();
            conversationDialogue.mainImage = spriteRenderer.sprite;
            conversationDialogue.titleText = speaker.name;
            conversationDialogue.mainText = dialogue;
        }
        
        public void Display()
        {
            conversationDialogue.Reset();

            if(stateName == "" || stateName == "Silent")
                LockCharacterPositions();

            if (enabledNextStates.Count == 1) {
                ConversationState nextState = enabledNextStates[0];
                if (nextState.stateName == "" || nextState.stateName == "Silent") {
                    conversationDialogue.gameObject.SetActive(false);
                    character.characterEvents.Clear();
                    UnlockCharacterPositions();
                }
                else
                {
                    SpriteRenderer spriteRenderer = nextState.speaker.GetComponentInChildren<SpriteRenderer>();
                    Debug.Assert(spriteRenderer != null);
                    conversationDialogue.gameObject.SetActive(true);
                    conversationDialogue.mainImage = spriteRenderer.sprite;
                    conversationDialogue.titleText = nextState.speaker.name;
                    conversationDialogue.mainText = nextState.dialogue;
                }
            }
            else
            {
                conversationDialogue.gameObject.SetActive(true);

                SpriteRenderer spriteRenderer = enabledNextStates[0].speaker.GetComponentInChildren<SpriteRenderer>();
                Debug.Assert(spriteRenderer != null);
                conversationDialogue.mainImage = spriteRenderer.sprite;

                List<string> responsetexts = new List<string>();
                foreach (ConversationState state in enabledNextStates)
                    responsetexts.Add(state.dialogue);
                    
                conversationDialogue.responseTexts = responsetexts.ToArray();            
            }
        }

        public void AnimateConversationActions()
        {         
            foreach (string conversationAction in conversationActions)
            {
                Debug.Assert(conversationAction != "");
                GameManager.main.AddGameTask(conversationAction);
            }
        }

        public void LockCharacterPositions()
        {
            // Try finding a suitable position for the character to walk
            Debug.Log("Relocating characters");
            Debug.Assert(character != null);
            PixelCollider characterPixelCollider = character.GetComponentInChildren<PixelCollider>();
            PixelRoom pixelRoom = characterPixelCollider.GetPixelRoom();
            pixelRoom.StampPixelCollider(characterPixelCollider);

            GameObject player = GameObject.Find("Player");
            Debug.Assert(player != null);

            PixelCollider playerCollider = player.GetComponentInChildren<PixelCollider>();
            Character playerCharacter = player.GetComponent<Character>();

            KeyValuePair<PixelPose, float> bestPlayerMovementWayPoint = playerCollider.FindBestWayPoint();
            KeyValuePair<PixelPose, float> bestCharacterMovementWayPoint = characterPixelCollider.FindBestWayPoint();

            float playerMoveTooMuchWhileTalkingThreshold = 5.0f;
            if(bestPlayerMovementWayPoint.Value < playerMoveTooMuchWhileTalkingThreshold) {
                GameTask gameTask = new GameTask(GameTask.TaskType.WALKTO);
                gameTask.character = playerCharacter;
                gameTask.arguments.Add(pixelRoom.name);
                gameTask.arguments.Add(bestPlayerMovementWayPoint.Key);
                GameManager.main.gameTasks.Enqueue(gameTask);
            }
            else {
                GameTask gameTask = new GameTask(GameTask.TaskType.WALKTO);
                gameTask.character = character;
                gameTask.arguments.Add(pixelRoom.name);
                gameTask.arguments.Add(bestCharacterMovementWayPoint.Key);
                GameManager.main.gameTasks.Enqueue(gameTask);
            }
        }

        public void UnlockCharacterPositions()
        {
            
        }

        public ConversationState NextState(int option = 1) {
            if (enabledNextStates.Count == 1)
                return enabledNextStates[0];
            if (option > enabledNextStates.Count) return null;
            return enabledNextStates[option - 1];
        }
    }
}