using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using System.Linq;
using Mono.Data.SqliteClient;

/// <summary>
/// Partial class for the Database Manager - Conversation
/// </summary>
public partial class DatabaseManager : Manager  {
	
	#region Variables

	string[] charText = new string[4]; //The 4 text options that are returned

	/// <summary>
	/// Prerequisites for a single conversation, 0 is always empty
	/// </summary>
	bool[] CONVO_Prerequisites = new bool[Settings.ConversationCharacterLimit];

	#endregion

	public bool CONVO_TestCharacterConversation (string character) {
		try {
			StartDatabase (conversations);
			ExecuteSQLQuery ("select * from " + character);
			reader.Read ();
		} catch (SqliteExecutionException s) {
			Debug.Log (s.Message);
			return false;
		}
		return true;
	}

	public bool CONVO_setEventPrerequisites(string character) {
		return true;
	}

	public singleConversation CONVO_start(string characterName) {
		//0. Start Database Connection
		StartDatabase(conversations);

		//1. Reset boolean flags for all conversations
		for (int i = 0; i < 500; i++)
			CONVO_Prerequisites [i] = false;

		//2. Search all possible prerequisites for ones that start with S
		ExecuteSQLQuery("select \"Order\", \"End Type (0/1)\" from " + characterName + " where Prerequisites like '%S%' and \"End Type (0/1)\" not like '1' Order by Prerequisites");

		//3. Find the one with the first number S1 and End Type 0
		int starterNum = 0; int endtype = 0;
		while (reader.Read ()) {
			starterNum = reader.GetInt32 (0);
			endtype = reader.GetInt32 (1);
		}

		//4. Set that end type to 1
		if (endtype == 0) {// 2 for looping
			ExecuteSQLQuery ("Update " + characterName + " SET 'End Type (0/1)' = '1' where \"Order\" is " + starterNum);
			reader.Read ();
		}

		//5. Set all possible paths to have +ve boolean flags
		ExecuteSQLQuery("select \"Add Prerequisite\" from " + characterName + " where \"Order\" is " + starterNum);
		reader.Read ();
		string newPrerequisiteString = getReaderString (0);
		List<string> newPrerequisites = StringToStringList (newPrerequisiteString);

		foreach (string s in newPrerequisites) {
			ExecuteSQLQuery("select \"Order\" from " + characterName + " where Prerequisites like '" + s + "'");
			reader.Read();
			int i = reader.GetInt32(0);
			CONVO_Prerequisites[i] = true;
		}

		//6. Return that specific conversation
		ExecuteSQLQuery("select \"Source\", \"Conversation\" from " + characterName + " where \"Order\" is " + starterNum);

		string speaker = ""; string newConversationString = "";
		while (reader.Read ()) {
			speaker = getReaderString (0);
			newConversationString = getReaderString (1);
		}

		singleConversation newConversation = new singleConversation();
		newConversation.speaker = (speaker == "Player") ? "Player" : characterName;
		newConversation.spoken = (speaker == "Player") ? characterName : "Player";
		newConversation.conversation = newConversationString;
		newConversation.options = (newPrerequisiteString[0] == 'C') ? true : false;
		newConversation.response = new string[4];

		return newConversation;
	}

	/// <summary>
	/// Continues a single conversation, used for single reponses
	/// </summary>
	/// <returns>The o continue.</returns>
	/// <param name="characterName">Name of interacting character</param>
	/// <param name="playerChoice">The character</param>
	/// <param name="openChoice">If set to <c>true</c> the thing looks for a choice (NOT FINISHED, implement later, set to true for now</param>
	public singleConversation CONVO_continue(string characterName, int playerChoice, bool openChoice) {
		char playerChoiceChar = (char) (playerChoice + 64);
		//0. Start Database Connection
		StartDatabase(conversations);

		//1a. Get Conversation Data (Single Response)
		string Source = ""; string[] Conversation = new string[4]; string[] addPrerequisites = new string[4];

		int prereqCount = 0;
		if (!openChoice) {
			if (playerChoice != 0) {
				for (int i = 0; i < Settings.ConversationCharacterLimit; i++) {
					if (CONVO_Prerequisites [i]) {
						int a = ExecuteSQLQueryInt ("select COUNT(*) from " + characterName + " where \"Order\" is " + i + 
							" and \"Prerequisites\" like '%" + playerChoiceChar + "'", Database.Conversation, 0);
						if(a == 0) continue;
						ExecuteSQLQuery ("select \"Source\", \"Conversation\", \"Add Prerequisite\" from " + characterName + " where \"Order\" is " + i + 
							" and \"Prerequisites\" like '%" + playerChoiceChar + "'");

						while (reader.Read ()) {
							Source = getReaderString (0);
							Conversation [0] = getReaderString (1);
							addPrerequisites [0] = getReaderString (2);
						}
						CONVO_Prerequisites [i] = false;
					}
				}
			} else {
				for (int i = 0; i < Settings.ConversationCharacterLimit; i++) {
					if (CONVO_Prerequisites [i]) {
						ExecuteSQLQuery ("select \"Source\", \"Conversation\", \"Add Prerequisite\" from " + characterName + " where \"Order\" is " + i);

						while (reader.Read ()) {
							Source = getReaderString (0);
							Conversation [0] = getReaderString (1);
							addPrerequisites [0] = getReaderString (2);
						}

						CONVO_Prerequisites [i] = false;
					}
				}
			}
		}

		//1a. Get Conversation Data (Multiple Response)
		else {
			for (int i = 0; i < Settings.ConversationCharacterLimit; i++) {
				if (CONVO_Prerequisites [i]) {
					ExecuteSQLQuery ("select \"Source\", \"Conversation\", \"Add Prerequisite\" from " + characterName + " where \"Order\" is " + i);
					while (reader.Read ()) {
						Source = getReaderString (0);
						Conversation[prereqCount] = getReaderString (1);
						addPrerequisites[prereqCount] = getReaderString (2);
					}
					prereqCount++;
					CONVO_Prerequisites[i] = false;
				}
			}
		}

		//3. Set all possible paths to have +ve boolean flags
		List<string> newPrerequisites = new List<string>();

		if (Settings.debugDATA) Debug.Log (Conversation[0]);
		for (int i = 0; i < 4; i++) {
			if (addPrerequisites [i] != "" && addPrerequisites [i] != null)
				newPrerequisites.AddRange (StringToStringList (addPrerequisites [i]));
		}

		foreach (string s in newPrerequisites) {
			ExecuteSQLQuery ("select \"Order\" from " + characterName + " where Prerequisites like '" + s + "'");
			reader.Read ();
			int i = reader.GetInt32 (0);
			CONVO_Prerequisites [i] = true;
		}

		//4. Return that conversation
		singleConversation newConversation = new singleConversation();
		newConversation.speaker = (Source == "Player") ? "Player" : characterName;
		newConversation.spoken = (Source == "Player") ? characterName : "Player";
		newConversation.conversation = Conversation[0];
		for (int i = 0; i < 4; i++) {
			if (addPrerequisites [i] == null || addPrerequisites [i] == "")
				continue;
			else 
				newConversation.options = (addPrerequisites[i][0] == 'C') ? true : false;
		}

		newConversation.response = Conversation;

		return newConversation;
	}

	public singleConversation CONVO_finishresponse(singleConversation convo, string characterName, int choice) {
		// Remove the boolean flags for everything other than the choice

		string preReq = "";
		for (int i = 0; i < Settings.ConversationCharacterLimit; i++) {
			if (CONVO_Prerequisites [i]) {
				ExecuteSQLQuery ("select \"Prerequisites\" from " + characterName + " where \"Order\" is " + i);
				while (reader.Read ()) {
					preReq = getReaderString (0);
					if ((int)(getLastCharInString (preReq)) == 'A' + ((choice - 1)) || (int)getLastCharInString (preReq) == ('a' + (choice - 1)))
						continue;
					else {
						CONVO_Prerequisites [i] = false;
					}
				}
			}
		}
		var newConvo = new singleConversation (
			"Player",
			characterName,
			convo.response [choice-1],
			false,
			null
		);
		return newConvo;
	}

	public singleConversation CONVO_readnext(string characterName, int playerChoice, bool openChoice) { //If player choice is 0, then was not a choice response
		char playerChoiceChar = (char) (playerChoice + 64);
		//0. Start Database Connection
		StartDatabase(conversations);

		//1a. Get Conversation Data (Single Response)
		string Source = ""; string[] Conversation = new string[4]; string[] addPrerequisites = new string[4];

		if (!openChoice) {
			if (playerChoice != 0) {
				for (int i = 0; i < Settings.ConversationCharacterLimit; i++) {
					if (CONVO_Prerequisites [i]) {
						int a = ExecuteSQLQueryInt ("select COUNT(*) from " + characterName + " where \"Order\" is " + i + 
							" and \"Prerequisites\" like '%" + playerChoiceChar + "'", Database.Conversation, 0);
						if(a == 0) continue;
						ExecuteSQLQuery ("select \"Source\", \"Conversation\", \"Add Prerequisite\" from " + characterName + " where \"Order\" is " + i + 
							" and \"Prerequisites\" like '%" + playerChoiceChar + "'");

						while (reader.Read ()) {
							Source = getReaderString (0);
							Conversation [0] = getReaderString (1);
							addPrerequisites [0] = getReaderString (2);
						}
						CONVO_Prerequisites [i] = false;
					}
				}
			} else {
				for (int i = 0; i < Settings.ConversationCharacterLimit; i++) {
					if (CONVO_Prerequisites [i]) {
						ExecuteSQLQuery ("select \"Source\", \"Conversation\", \"Add Prerequisite\" from " + characterName + " where \"Order\" is " + i);

						while (reader.Read ()) {
							Source = getReaderString (0);
							Conversation [0] = getReaderString (1);
							addPrerequisites [0] = getReaderString (2);
						}

						CONVO_Prerequisites [i] = false;
					}
				}
			}
		}

		//1a. Get Conversation Data (Multiple Response)
		else {
			int j = 0;
			for (int i = 0; i < Settings.ConversationCharacterLimit; i++) {
				if (CONVO_Prerequisites [i]) {
					ExecuteSQLQuery ("select \"Source\", \"Conversation\", \"Add Prerequisite\" from " + characterName + " where \"Order\" is " + i);
					while (reader.Read ()) {
						Source = getReaderString (0);
						Conversation[j] = getReaderString (1);
						addPrerequisites[j] = getReaderString (2);
					}
					j++;
					CONVO_Prerequisites[i] = false;
				}
			}
		}

		//3. Return that conversation
		singleConversation newConversation = new singleConversation();
		newConversation.speaker = (Source == "Player") ? "Player" : characterName;
		newConversation.spoken = (Source == "Player") ? characterName : "Player";
		newConversation.conversation = Conversation[0];
		for (int i = 0; i < 4; i++) {
			if (addPrerequisites [i] == null || addPrerequisites [i] == "")
				continue;
			else 
				newConversation.options = (addPrerequisites[i][0] == 'C') ? true : false;
		}

		newConversation.response = Conversation;

		return newConversation;
	}

	public bool CONVO_setNewPrerequisites(string characterName) {
		return false;
	}

	public bool CONVO_checkEndConversations(string characterName) {
		for (int i = 0; i < Settings.ConversationCharacterLimit; i++) {
			if (CONVO_Prerequisites [i]) {

				ExecuteSQLQuery ("select \"End Type (0/1)\" from " + characterName + " where \"Order\" like " + i);
				reader.Read ();
				int v = reader.GetInt32 (0);
				if (v == 1) {
					if (Settings.debugDATA)
						Debug.Log ("CONVO: " + i + " ends the conversation");
					return true;
				} else
					return false;
			}
		}
		return false;
	}

	public void CONVO_printAllPrerequisites() {
		if (!Settings.debugDATA)
			return;

		int pile = 0;
		string boolstring = "";
		for (int i = 1; i < Settings.ConversationCharacterLimit; i++) {
			if (CONVO_Prerequisites [i]) {
				pile = 0;
				boolstring += "1";
			} else {
				pile++;
				boolstring += "0";
			}
			if (pile > 10)
				break;
		}
		Debug.Log ("BOOL FLAGS: " + boolstring);
	}

}
