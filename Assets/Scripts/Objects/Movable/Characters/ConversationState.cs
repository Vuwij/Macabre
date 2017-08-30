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
		List<Objects.Movable.Characters.Character> AllCharactersInConversation
		{
			get { return new List<Objects.Movable.Characters.Character>() {currentSpeaker}; }
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

					var reqEventList = SeparateEvents(s.requireEvents);
					var excEventList = SeparateEvents(s.excludeEvents);
					bool validateRequireEvents = true;
					foreach(var e in reqEventList) {
						if(Game.main.eventList.Find(x => (x == e)) == null)
							validateRequireEvents = false;
					}
					foreach(var e in excEventList) {
						if(Game.main.eventList.Find(x => (x == e)) != null) {
							validateRequireEvents = false;
						}
					}
					if(validateRequireEvents == false) continue;

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
		List<CharacterAction> characterAction = new List<CharacterAction>();

		// From the database information
		public string stateName;
		public string[] addStates;
		public Objects.Movable.Characters.Character currentSpeaker;
		public string dialogue;
		public string actions;
		public string addEvents;
		public string removeEvents;
		public string requireEvents;
		public string excludeEvents;

		public ConversationState(Objects.Movable.Characters.Character speakerController, ConversationState previousState = null, string stateName = "")
		{
			this.character = speakerController;
			this.previousState = previousState;
			if (previousState == null)
			{
				Game.main.db.conversations.FindAndUpdateConversationForCharacter(character, this);
				LockAllCharacterPosition();
				UpdateEvents();
			}
			else
				Game.main.db.conversations.UpdateConversationForCharacter(stateName, character, this);

			SetCurrentViewFromPreviousState(previousState);
			conversationDialogue = Game.main.UI.Find<ConversationDialogue>();
			conversationDialogue.TurnOn();
		}

		public ConversationState GetNextState(int decision = 0)
		{
			UpdateEvents();
			ParseAction();
			RunAction();

			ConversationState nextState;
			if (NextStates.Count == 0)
			{
				UnlockAllCharacterPosition();
				nextState =  null;
			}
			else if (conversationViewStatus == ConversationViewStatus.PlayerMultipleReponse)
			{
				ConversationState c = previousState.NextStates[decision];
				c.conversationViewStatus = ConversationViewStatus.PlayerResponse;
				nextState = c;
			}
			else
				nextState = NextStates[decision];

			return nextState;
		}

		#region Actions

		void LockAllCharacterPosition()
		{
//			Debug.Log("Locking positions");
			foreach (var character in AllCharactersInConversation) {
//				Debug.Log(character.name);
				if(character == null) continue;
				character.isTalking = true;
			}
			var player = GameObject.Find("Player").GetComponent<Player>();
			player.isTalking = true;

			// Make all the characters face each other
			var firstCharacter = AllCharactersInConversation[0];
			Vector2 center = (firstCharacter.transform.position - player.transform.position);
			center = Vector2.Scale(center, new Vector2(0.5f, 0.5f));
			center = (Vector2) player.transform.position + center;

			Debug.DrawLine(player.transform.position, center, Color.red, 10.0f);
			player.GetComponentInChildren<Animator>().SetFloat("MoveSpeed-x", center.x - player.transform.position.x);
			player.GetComponentInChildren<Animator>().SetFloat("MoveSpeed-y", center.y - player.transform.position.y);
			foreach(var character in AllCharactersInConversation) {
				var anim = character.GetComponentInChildren<Animator>();
				anim.SetBool("IsActive", true);
				anim.SetFloat("MoveSpeed-x", center.x - character.transform.position.x);
				anim.SetFloat("MoveSpeed-y", center.y - character.transform.position.y);
//				Debug.Log(center.x - character.transform.position.x);
//				Debug.Log(center.y - character.transform.position.y);
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

		void ParseAction() {
			var actionStrings = actions.Split('\n');
			foreach(var astring in actionStrings) {
				if(astring != "") {
					characterAction.Add(new CharacterAction(astring));
				}
			}
		}

		void RunAction() {
		}

		#endregion

		#region Events

		string[] SeparateEvents(string events) {
			if(events == null) return new string[]{};
			events = events.Replace(", ",",");
			var eventsList = events.Split(',');
			if(eventsList.Length == 1 && eventsList[0] == "") return new string[]{};
			return eventsList;
		}

		void UpdateEvents() {
			var addEventList = SeparateEvents(addEvents);
			var removeEventList = SeparateEvents(removeEvents);

			foreach(var e in addEventList) {
				if(Game.main.eventList.Find(x => x == e) == null)
					Game.main.eventList.Add(e);
			}
			foreach(var e in removeEventList) {
				if(Game.main.eventList.Find(x => x == e) != null)
					Game.main.eventList.Remove(e);
			}
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