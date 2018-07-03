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
        NONE
	}
	public TaskType taskType = TaskType.NONE;
	public string actionString = "";
	public List<string> argumentsString = new List<string>();
	public List<object> arguments = new List<object>();
	public Character character;

	public float duration = 3.0f; // Wait for 3 seconds
    
	public GameTask() {}

	public GameTask(string actionString) {
		this.actionString = actionString;
		BreakUpString();
		IdentifyTaskType();
		FindArguments();
	}

	public GameTask(TaskType taskType, params object[] arguments) {
		this.taskType = taskType;
        for (int i = 0; i < arguments.Length; ++i)
			this.arguments.Add(arguments[i]);
	}

	void BreakUpString() {
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

		argumentsString = sss;
		//foreach(string z in argumentsString) {
		//	Debug.Log(z);
		//}
	}

    // Finds the character and the task type
	void IdentifyTaskType() {

        // Command always 2nd otherwise its player
		if (argumentsString[1] == "puts")
			taskType = TaskType.PUTS;
		else if (argumentsString[1] == "takes")
			taskType = TaskType.TAKES;
		else if (argumentsString[1] == "goto")
			taskType = TaskType.NAVIGATE;
		else if (argumentsString[1] == "gives")
			taskType = TaskType.GIVES;
		else if (argumentsString[1] == "create")
			taskType = TaskType.CREATE;
		else if (argumentsString[1] == "animate")
			taskType = TaskType.ANIMATE;
		else if (argumentsString[1] == "teleport")
            taskType = TaskType.ANIMATE;
		else if (argumentsString[1] == "attack")
            taskType = TaskType.ANIMATE;
		else if (argumentsString[1] == "updatestat")
            taskType = TaskType.ANIMATE;

		if(taskType != TaskType.NONE) {
			GameObject gameManagerObj = GameObject.Find("Game Manager");
            GameManager gameManager = gameManagerObj.GetComponent<GameManager>();
            string characterName = gameManager.characterNameTranslations[argumentsString[0]];
            GameObject characterObj = GameObject.Find(characterName);
            character = characterObj.GetComponent<Character>();

			Debug.Assert(character != null);
            Debug.Assert(taskType != TaskType.NONE);
			return;
		}

		if (argumentsString[0] == "puts")
            taskType = TaskType.PUTS;
        else if (argumentsString[0] == "takes")
            taskType = TaskType.TAKES;
        else if (argumentsString[0] == "goto")
            taskType = TaskType.NAVIGATE;
        else if (argumentsString[0] == "gives")
            taskType = TaskType.GIVES;
        else if (argumentsString[0] == "create")
            taskType = TaskType.CREATE;
        else if (argumentsString[0] == "animate")
            taskType = TaskType.ANIMATE;
        else if (argumentsString[0] == "teleport")
            taskType = TaskType.ANIMATE;
        else if (argumentsString[0] == "attack")
            taskType = TaskType.ANIMATE;
        else if (argumentsString[0] == "updatestat")
            taskType = TaskType.ANIMATE;

		GameObject player = GameObject.Find("Player");
        character = player.GetComponent<Character>();
        
		Debug.Assert(character != null);
        Debug.Assert(taskType != TaskType.NONE);
	}

	void FindArguments() {
		if (taskType == TaskType.NAVIGATE)
        {
            Debug.Assert(character != null);
            if (character is Player)
            {
                string locationString = argumentsString[1];
                PixelRoom room = GetObjectOfType<PixelRoom>(locationString);
                arguments.Add(room);

                string objectString = argumentsString[2];
                PixelCollider pixelCollider = GetObjectOfType<PixelCollider>(objectString, room.transform);
                arguments.Add(pixelCollider);
            }
        }    
	}

	T GetObjectOfType<T> (string objectName, Transform parent = null) 
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
			default:
				break;
		}      
	}
}