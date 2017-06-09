using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data.Database;
using Objects.Movable.Characters;
using Objects.Movable.Characters.Individuals;
using UI;
using UI.Dialogues;
using UnityEngine;

namespace Conversation
{
    public class ConversationState
    {
		#region State Logic

		// The characters and character controller remains the same for each conversation
        public Character character;

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
                    ConversationState s = new ConversationState(character, this, stateName);
                    next.Add(s);
                }
                return next;
            }
        }
        
        // Creates a conversation for the speaker
        public ConversationState(Objects.Movable.Characters.Character speakerController, ConversationState previousState = null, string stateName = "")
        {
            this.character = speakerController;
            this.previousState = previousState;
            if (previousState == null)
            {
				DatabaseConnection.ConversationDB.FindAndUpdateConversationForCharacter(character, this);
                LockAllCharacterPosition();
            }
            else
				DatabaseConnection.ConversationDB.UpdateConversationForCharacter(stateName, character, this);

            SetCurrentViewFromPreviousState(previousState);
        }

        // Update the next state and speaker with the current state
        public ConversationState GetNextState(int decision = 0)
        {
            if (nextStates.Count == 0)
            {
                UnlockAllCharacterPosition();
                return null;
            }
            if (conversationViewStatus == ConversationViewStatus.PlayerMultipleReponse)
            {
                ConversationState c = previousState.nextStates[decision];
                c.conversationViewStatus = ConversationViewStatus.PlayerResponse;
                return c;
            }
            else
                return nextStates[decision];
        }

		#endregion

		#region Database Information

		// From the database information
		public string stateName;
		public string[] addStates;
		public Objects.Movable.Characters.Character currentSpeaker;
		public string dialogue;
		public string addEvents;
		public string removeEvents;
		public string requireEvents;

		private int NextStateCount
		{
			get
			{
				if (conversationViewStatus == ConversationViewStatus.PlayerMultipleReponse)
					return previousState.addStates.Length + 1;
				else return 1;
			}
		}

		public bool InputIsValid(int input)
		{
			if (conversationViewStatus == ConversationViewStatus.PlayerMultipleReponse)
			if (input > 0 && input < NextStateCount) return true;
			return false;
		}

		public void Print()
		{
			string stateInfo = "";

			// Basic Info
			stateInfo += "State: " + stateName + "\n";
			stateInfo += "Speaker: " + currentSpeaker + "\n";
			stateInfo += "Next States: " + "\n";

			if(addStates != null)
				foreach (var nextState in addStates)
					stateInfo += "  States: " + nextState + "\n";

			// Dialogue
			stateInfo += "Dialogue: " + dialogue + "\n";

			// Event Info
			stateInfo += "Events: " + "\n";
			stateInfo += "  Add Events: " + "\n";
			foreach (var addEvent in addEvents)
				stateInfo += "  " + addEvents + "\n";
			stateInfo += "  Remove Events: " + "\n";
			foreach (var removeEvents in addEvents)
				stateInfo += "  " + removeEvents + "\n";
			stateInfo += "  Require Events: " + "\n";
			foreach (var requireEvents in addEvents)
				stateInfo += "  " + requireEvents + "\n";

			Debug.Log(stateInfo);
		}

		#endregion

		#region Actions

		private List<Objects.Movable.Characters.Character> AllCharactersInConversation
		{
			get { return FindAllCharactersInConversation(this).Distinct().ToList(); }
		}

		private IEnumerable<Objects.Movable.Characters.Character> FindAllCharactersInConversation(ConversationState root)
		{
			yield return currentSpeaker;
			foreach (ConversationState next in root.nextStates)
				foreach (var x in FindAllCharactersInConversation(next))
					yield return x;
		}

		private void LockAllCharacterPosition()
		{
			foreach (Objects.Movable.Characters.Character character in AllCharactersInConversation)
				character.isTalking = true;
		}

		private void UnlockAllCharacterPosition()
		{
			foreach (Objects.Movable.Characters.Character character in AllCharactersInConversation)
				character.isTalking = false;
		}

		#endregion

		#region View

		public ConversationViewStatus conversationViewStatus = ConversationViewStatus.Empty;

		private void SetCurrentViewFromPreviousState(ConversationState previousState)
		{
			if (previousState == null)
			{
				if (currentSpeaker is Player) conversationViewStatus = ConversationViewStatus.PlayerResponse;
				else conversationViewStatus = ConversationViewStatus.CharacterResponse;
			}
			else conversationViewStatus = IdentifyCurrentViewFromPreviousState(previousState);
		}

		private ConversationViewStatus IdentifyCurrentViewFromPreviousState(ConversationState previousState)
		{
			if (previousState.addStates.Count() == 0) return ConversationViewStatus.Empty;
			if (previousState.addStates.Count() > 1) return ConversationViewStatus.PlayerMultipleReponse;
			if (previousState.currentSpeaker is Player)
				return ConversationViewStatus.CharacterResponse;
			else return ConversationViewStatus.PlayerResponse;
		}

		public static void DisplayState(ConversationState convo)
		{
			var conversationDialogue = Game.main.UI.Find<ConversationDialogue>();

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

		#endregion

    }
}