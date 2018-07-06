using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Objects.Movable.Characters;
using Objects.Movable.Characters.Individuals;
using Objects;

public class GameTask
{
    // All the task types are enumerated here
    // Tasks are stored in queues stored in every character in the game
    // The current implementation is asynchronous and all the characters in the game do their own thing
    // without waiting for other characters to complete their task

	public enum TaskType {
		PUTS,
		TAKES,
		NAVIGATE,   // Find other rooms
        WALKTO,     // Walk to a point within a room
        ENTERDOOR,
		GIVES,
		CREATE,
		ANIMATE,
		TELEPORT,
		ATTACK,
		UPDATESTAT,
        INSPECT,
        NONE
	}
	public TaskType taskType = TaskType.NONE;
	public List<object> arguments = new List<object>();
	public Character character;

	public float duration = 3.0f; // Wait for 3 seconds
    
	public GameTask() {}

	public GameTask(TaskType taskType, params object[] arguments) {
		this.taskType = taskType;
        for (int i = 0; i < arguments.Length; ++i)
			this.arguments.Add(arguments[i]);
	}

	public static List<GameTask> CreateGameTasks(string actionString) {
		List<GameTask> gameTasks = new List<GameTask>();
		List<string> actionStrings = BreakUpString(actionString);

		Character taskCharacter;
		TaskType type = IdentifyTaskType(actionStrings, out taskCharacter);

		if (type == TaskType.NAVIGATE)
        {
			GameTask gameTask = new GameTask();
			gameTask.taskType = TaskType.NAVIGATE;

			Debug.Assert(taskCharacter != null);

			string locationString = actionStrings[2];
            PixelRoom room = GetObjectOfType<PixelRoom>(locationString);
            gameTask.arguments.Add(room);

            string objectString = actionStrings[3];
            PixelCollider pixelCollider = GetObjectOfType<PixelCollider>(objectString, room.transform);
            gameTask.arguments.Add(pixelCollider);
            gameTask.character = taskCharacter;

			gameTasks.Add(gameTask);
        }
		else if (type == TaskType.PUTS)
        {
            // Navigate first         
			int number = 1;
			bool hasNumber = int.TryParse(actionStrings[2], out number);
            
			if (!hasNumber) {
				actionStrings.Insert(2, "1");
			}

			PixelInventory inv = taskCharacter.GetComponentInChildren<PixelInventory>();
            Debug.Assert(inv != null);
			bool hasItem = inv.HasItem(actionStrings[3]);
			if (!hasItem) {
				Debug.LogWarning("Insufficent number of items, Require " + number + " " + actionStrings[3]);
				return gameTasks;
			}
            
			string locationString = actionStrings[4];
            PixelRoom room = GetObjectOfType<PixelRoom>(locationString);
            
            string objectString = actionStrings[5];
            PixelCollider pixelCollider = GetObjectOfType<PixelCollider>(objectString, room.transform);
            
			PixelStorage storage = pixelCollider.transform.parent.GetComponent<PixelStorage>();
			if(storage == null) {
				Debug.LogWarning("Cannot Place Item in " + actionStrings[5]);
                return gameTasks;
			}

            // Navigate First
			GameTask navigateTask = new GameTask();
            navigateTask.taskType = TaskType.NAVIGATE;
			navigateTask.character = taskCharacter;
			navigateTask.arguments.Add(room);
			navigateTask.arguments.Add(pixelCollider);
            gameTasks.Add(navigateTask);

            // Then Put
			GameTask putsItemTask = new GameTask();
            putsItemTask.taskType = TaskType.PUTS;
			putsItemTask.arguments.Add(number);
            putsItemTask.arguments.Add(actionStrings[3]);
			putsItemTask.arguments.Add(storage);
			putsItemTask.character = taskCharacter;
			gameTasks.Add(putsItemTask);
        }
		else if (type == TaskType.CREATE)
		{
			GameTask createItemTask = new GameTask();
			createItemTask.taskType = TaskType.CREATE;
            
			GameObject itemObj = null;

			int number = 1;
            bool hasNumber = int.TryParse(actionStrings[2], out number);
            if (hasNumber)
            {
                Debug.Assert(number >= 1 && number <= 4);
				itemObj = Resources.Load("Items/" + actionStrings[3]) as GameObject;
            }
            else
            {
				itemObj = Resources.Load("Items/" + actionStrings[2]) as GameObject;
            }

			Debug.Assert(itemObj != null);
			createItemTask.arguments.Add(number);
			createItemTask.arguments.Add(itemObj);
			createItemTask.character = taskCharacter;

			gameTasks.Add(createItemTask);
		}

		return gameTasks;
	}

	static List<string> BreakUpString(string actionString) {
		// Remove the ' ' delimiters
		string s = actionString;
		s.Replace("''", "' '"); // adds spaces in case they are not together
		string[] ss = s.Split('\'');
		List<string> sss = new List<string>();
		for (int i = 0; i < ss.Length; ++i) {
			if(i % 2 == 0) {
				string[] ssss = ss[i].Split(' ');
				foreach (string sssss in ssss) {
					if (sssss == "" || sssss == " ")
						continue;
					sss.Add(sssss.Trim());
				}
			}
			else {
				sss.Add(ss[i].Trim());
			}
		}

		return sss;
		//foreach(string z in argumentsString) {
		//	Debug.Log(z);
		//}
	}

    // Finds the character and the task type
	static TaskType IdentifyTaskType(List<string> actionStrings, out Character character) {
		TaskType type = TaskType.NONE;

        // Identify the character
		GameObject gameManagerObj = GameObject.Find("Game Manager");
        GameManager gameManager = gameManagerObj.GetComponent<GameManager>();

		bool hasCharacter = gameManager.characterNameTranslations.ContainsKey(actionStrings[0]);

		if (hasCharacter) {
			string characterName = gameManager.characterNameTranslations[actionStrings[0]];
            GameObject characterObj = GameObject.Find(characterName);
            character = characterObj.GetComponent<Character>();
		}
		else {
			GameObject player = GameObject.Find("Player");
            character = player.GetComponent<Character>();
            actionStrings.Insert(0, "Player");
		}
  
        Debug.Assert(character != null);
              
		// Command always 2nd otherwise its player
		if (actionStrings[1] == "puts")
			type = TaskType.PUTS;
		else if (actionStrings[1] == "takes")
			type = TaskType.TAKES;
		else if (actionStrings[1] == "goto")
			type = TaskType.NAVIGATE;
		else if (actionStrings[1] == "gives")
			type = TaskType.GIVES;
		else if (actionStrings[1] == "create")
			type = TaskType.CREATE;
		else if (actionStrings[1] == "animate")
			type = TaskType.ANIMATE;
		else if (actionStrings[1] == "teleport")
			type = TaskType.TELEPORT;
		else if (actionStrings[1] == "attack")
			type = TaskType.ATTACK;
		else if (actionStrings[1] == "updatestat")
			type = TaskType.UPDATESTAT;
            
		Debug.Assert(character != null);
        Debug.Assert(type != TaskType.NONE);
		return type;
	}

	static T GetObjectOfType<T> (string objectName, Transform parent = null) 
		where T : MonoBehaviour {

		GameObject obj = null;
		if (parent == null)
		{
			GameObject[] objects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
			foreach(GameObject o in objects) {
				if (o.name == objectName)
					obj = o.gameObject;
			}
		}
		else
		{
			for (int i = 0; i < parent.childCount; ++i)
			{
				if (parent.GetChild(i).name == objectName)
					obj = parent.GetChild(i).gameObject;
			}
		}

		if (obj == null)
        {
			Debug.LogWarning(objectName + " not found");
			return default(T);
        }

		T t = null;
		if (typeof(T) == typeof(PixelCollider)) {
			for (int i = 0; i < obj.transform.childCount; ++i)
            {
				t = obj.transform.GetChild(i).GetComponent<PixelCollider>() as T;
				if (t != null)
					break;
            }
		}
		else {
			t = obj.GetComponent<T>();
		}

		if (t == null)
        {
			Debug.LogWarning(objectName + " does not contain a correct script");
			return default(T);         
        }

		return t;      
	}

	public void Execute() {
		Debug.Log("Executing " + taskType.ToString() + " for " + character.name);
		switch (taskType) {
			case TaskType.NAVIGATE:
			case TaskType.PUTS:
			case TaskType.CREATE:
				character.characterTasks.Enqueue(this);
				break;
			default:
				break;
		}      
	}
}