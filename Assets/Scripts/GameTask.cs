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
			if (taskCharacter is Player)
            {
                string locationString = actionStrings[1];
                PixelRoom room = GetObjectOfType<PixelRoom>(locationString);
				gameTask.arguments.Add(room);

                string objectString = actionStrings[2];
                PixelCollider pixelCollider = GetObjectOfType<PixelCollider>(objectString, room.transform);
				gameTask.arguments.Add(pixelCollider);
				gameTask.character = taskCharacter;
            }
			gameTasks.Add(gameTask);
        }
		else if (type == TaskType.PUTS)
        {
            // Navigate first
			GameTask gameTask = new GameTask();

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
			type = TaskType.ANIMATE;
		else if (actionStrings[1] == "attack")
			type = TaskType.ANIMATE;
		else if (actionStrings[1] == "updatestat")
			type = TaskType.ANIMATE;

		if(type != TaskType.NONE) {
			GameObject gameManagerObj = GameObject.Find("Game Manager");
            GameManager gameManager = gameManagerObj.GetComponent<GameManager>();
            string characterName = gameManager.characterNameTranslations[actionStrings[0]];
            GameObject characterObj = GameObject.Find(characterName);
            character = characterObj.GetComponent<Character>();

			Debug.Assert(character != null);
			Debug.Assert(type != TaskType.NONE);
			return TaskType.NONE;
		}

		if (actionStrings[0] == "puts")
			type = TaskType.PUTS;
        else if (actionStrings[0] == "takes")
			type = TaskType.TAKES;
        else if (actionStrings[0] == "goto")
			type = TaskType.NAVIGATE;
        else if (actionStrings[0] == "gives")
			type = TaskType.GIVES;
        else if (actionStrings[0] == "create")
			type = TaskType.CREATE;
        else if (actionStrings[0] == "animate")
			type = TaskType.ANIMATE;
        else if (actionStrings[0] == "teleport")
			type = TaskType.ANIMATE;
        else if (actionStrings[0] == "attack")
			type = TaskType.ANIMATE;
        else if (actionStrings[0] == "updatestat")
			type = TaskType.ANIMATE;

		GameObject player = GameObject.Find("Player");
        character = player.GetComponent<Character>();
        
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
		if(typeof(T) == typeof(PixelCollider)) {
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
				character.characterTasks.Enqueue(this);
				break;
			case TaskType.PUTS:
                
				break;
			default:
				break;
		}      
	}
}