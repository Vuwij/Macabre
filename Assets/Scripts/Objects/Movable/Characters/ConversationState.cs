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

namespace Objects.Movable.Characters
{
    public class ConversationState
    {
		List<Objects.Movable.Characters.Character> AllCharactersInConversation
		{
			get { return FindAllCharactersInConversation(this).Distinct().ToList(); }
		}
		int NextStateCount
		{
			get
			{
				if (conversationViewStatus == ConversationViewStatus.PlayerMultipleReponse)
					return previousState.addStates.Length + 1;
				else return 1;
			}
		}
		List<ConversationState> NextStates
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
		public bool InputIsValid(int input)
		{
			if (conversationViewStatus == ConversationViewStatus.PlayerMultipleReponse)
			if (input >= 0 && input < NextStateCount) return true;
			return false;
		}

		public ConversationViewStatus conversationViewStatus = ConversationViewStatus.Empty;
		public Character character;
		ConversationState previousState;
		ConversationDialogue conversationDialogue;

		// From the database information
		public string stateName;
		public string[] addStates;
		public Objects.Movable.Characters.Character currentSpeaker;
		public string dialogue;
		public string addEvents;
		public string removeEvents;
		public string requireEvents;

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

			conversationDialogue = Game.main.UI.Find<ConversationDialogue>();
			conversationDialogue.TurnOn();
		}

		public ConversationState GetNextState(int decision = 0)
		{
			if (NextStates.Count == 0)
			{
				UnlockAllCharacterPosition();
				return null;
			}
			if (conversationViewStatus == ConversationViewStatus.PlayerMultipleReponse)
			{
				ConversationState c = previousState.NextStates[decision];
				c.conversationViewStatus = ConversationViewStatus.PlayerResponse;
				return c;
			}
			else
				return NextStates[decision];
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

		#region Actions

		IEnumerable<Objects.Movable.Characters.Character> FindAllCharactersInConversation(ConversationState root)
		{
			yield return currentSpeaker;
			foreach (ConversationState next in root.NextStates)
				foreach (var x in FindAllCharactersInConversation(next))
					yield return x;
		}

		void LockAllCharacterPosition()
		{
			Debug.Log("Locking positions");
			foreach (var character in AllCharactersInConversation) {
				Debug.Log(character.name);
				if(character == null) continue;
				character.isTalking = true;
			}
			var player = GameObject.Find("Player").GetComponent<Player>();
			player.isTalking = true;

			// Make all the characters face each other
			var firstCharacter = AllCharactersInConversation[0];
			Vector2 center = (firstCharacter.transform.position - player.transform.position);
			Vector2.Scale(center, new Vector2(0.5f, 0.5f));
			center = (Vector2) player.transform.position + center;

			Debug.DrawLine(player.transform.position, center, Color.red, 10.0f);
			player.GetComponentInChildren<Animator>().SetFloat("MoveSpeed-x", center.x - player.transform.position.x);
			player.GetComponentInChildren<Animator>().SetFloat("MoveSpeed-y", center.y - player.transform.position.y);
			foreach(var character in AllCharactersInConversation) {
				var anim = character.GetComponentInChildren<Animator>();
				anim.SetFloat("MoveSpeed-x", center.x - character.transform.position.x);
				anim.SetFloat("MoveSpeed-y", center.y - character.transform.position.y);
				anim.SetBool("IsActive", true);
			}
		}

		void UnlockAllCharacterPosition()
		{
			foreach (Objects.Movable.Characters.Character character in AllCharactersInConversation) {
				if(character == null) continue;
				character.isTalking = false;
				var anim = character.GetComponentInChildren<Animator>();
				anim.SetFloat("MoveSpeed-x", 0.0f);
				anim.SetFloat("MoveSpeed-y", 0.0f);
				anim.SetBool("IsActive", false);
			}
			var player = GameObject.Find("Player").GetComponent<Player>();
			player.isTalking = false;
		}

		#endregion

		#region Display

		void SetCurrentViewFromPreviousState(ConversationState previousState)
		{
			if (previousState == null)
			{
				if (currentSpeaker is Player) conversationViewStatus = ConversationViewStatus.PlayerResponse;
				else conversationViewStatus = ConversationViewStatus.CharacterResponse;
			}
			else conversationViewStatus = IdentifyCurrentViewFromPreviousState(previousState);
		}

		ConversationViewStatus IdentifyCurrentViewFromPreviousState(ConversationState previousState)
		{
			if (previousState.addStates.Count() == 0) return ConversationViewStatus.Empty;
			if (previousState.addStates.Count() > 1) return ConversationViewStatus.PlayerMultipleReponse;
			if (previousState.currentSpeaker is Player)
				return ConversationViewStatus.CharacterResponse;
			else return ConversationViewStatus.PlayerResponse;
		}

		public void DisplayState()
		{
			conversationDialogue.Reset();

			switch (conversationViewStatus)
			{
			case ConversationViewStatus.PlayerMultipleReponse:
				conversationDialogue.mainImage = currentSpeaker.GetComponentInChildren<SpriteRenderer>().sprite;
				conversationDialogue.responseTexts = (from state in previousState.NextStates
					select state.dialogue).ToArray();
				conversationDialogue.continueText = "";
				return;
			case ConversationViewStatus.CharacterResponse:
			case ConversationViewStatus.PlayerResponse:
				conversationDialogue.mainImage = currentSpeaker.GetComponentInChildren<SpriteRenderer>().sprite;
				conversationDialogue.titleText = currentSpeaker.name;
				conversationDialogue.mainText = dialogue;
				conversationDialogue.continueText = "Space to Continue";
				return;
			case ConversationViewStatus.Empty:
			default:
				return;
			}
		}

		public static void TurnOff()
		{
			var conversationDialogue = Game.main.UI.Find<ConversationDialogue>();
			conversationDialogue.TurnOff();
		}

		#endregion

    }
}