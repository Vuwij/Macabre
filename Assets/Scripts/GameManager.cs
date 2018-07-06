using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Objects.Movable.Characters;
using Data;
using Objects;
using Environment;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CsvHelper;

/// <summary>
/// The Game Manager is responsible for loading everything in the correct order. Individual files do not load themselves
/// </summary>
public class GameManager : MonoBehaviour {
	public static GameManager main = null;
	public static string dataPath;

	public Saves saves;
	public GameClock clock = new GameClock();
	public Database db;
	public Dictionary<string, string> gameEvents = new Dictionary<string, string>();
	public Dictionary<string, string> characterNameTranslations = new Dictionary<string, string>();
	public Queue<GameTask> gameTasks = new Queue<GameTask>();
	public List<GameTask> activeTasks = new List<GameTask>();

	void Awake()
    {
        if (main == null) main = this;
        else if (main != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

		dataPath = Application.dataPath;
		saves = new Saves();

        LoadInventoryInformation();
        LoadConversationInformation();
    }

	void Start()
	{
		// Update game tasks every second
		StartCoroutine(GameTaskUpdate());
	}

	void LoadInventoryInformation() {

        PixelItem[] items = Resources.LoadAll<PixelItem>("Items");
        List<PixelItem> itemsList = items.ToList();
        Debug.Assert(items.Length != 0);

        // Item Descriptions
		using (var reader = new StreamReader(@"Assets/Configuration/Items.csv"))
        {
			using (var csvreader = new CsvReader(reader))
            {
                csvreader.Configuration.HasHeaderRecord = true;

				while (csvreader.Read())
				{
					int id;
					int.TryParse(csvreader.GetField(0), out id);
					Debug.Assert(id >= 0);

					string objname = csvreader.GetField(1);
					if (objname == "Name") continue;
					if (objname == "") continue;
                    
					string description = csvreader.GetField(2);
					string[] properties = csvreader.GetField(3).Replace(" ", "").Split(',');

					PixelItem item = itemsList.Find((obj) => obj.name == objname);
					if (item == null) continue;
					item.id = id;
					item.description = description;

					if (!((properties.Length == 0) || (properties[0] == "")))
					{
						item.properties = properties;
					}
					else
					{
						item.properties = null;
					}
				}
            }
        }

        // Item Combinations
        using (var reader = new StreamReader(@"Assets/Configuration/Item Combine.csv"))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                string[] values = CsvSplit(line);

                int combinedid, id1, id2;
                int.TryParse(values[0], out combinedid);
                int.TryParse(values[1], out id1);
                int.TryParse(values[2], out id2);
                Debug.Assert(combinedid >= 0);
                Debug.Assert(id1 >= 0);
                Debug.Assert(id2 >= 0);
                if (combinedid == 0 && id1 == 0 && id2 == 0)
                    continue;

                PixelItem item1 = itemsList.Find((obj) => obj.id == id1);
                PixelItem item2 = itemsList.Find((obj) => obj.id == id2);
                PixelItem combinedItem = itemsList.Find((obj) => obj.id == combinedid);

                if (item1 == null) continue;
                if (item2 == null) continue;
                if (combinedItem == null) continue;

                item1.combinations.Clear();
                item2.combinations.Clear();

                PixelItem.Combination combination1 = new PixelItem.Combination();
                combination1.with = item2;
                combination1.result = combinedItem;
                item1.combinations.Add(combination1);

                PixelItem.Combination combination2 = new PixelItem.Combination();
                combination2.with = item1;
                combination2.result = combinedItem;
                item2.combinations.Add(combination2);

                combinedItem.breakapart.Clear();
                combinedItem.breakapart.Add(item1);
                combinedItem.breakapart.Add(item2);
            }
        }
    }

    void LoadConversationInformation() {
        Character[] characters = Resources.LoadAll<Character>("Characters");
        List<Character> characterList = characters.ToList();
        Debug.Assert(characters.Length != 0);

        // Character information
        using (var reader = new StreamReader(@"Assets/Configuration/Characters.csv")) {

			using (var csvreader = new CsvReader(reader))
			{
				while (csvreader.Read())
				{
					string characterName = csvreader.GetField(0);
					string description = csvreader.GetField(1);
					string attackdamage = csvreader.GetField(2);
					string health = csvreader.GetField(3);
					string shortnames = csvreader.GetField(4);

					if (characterName == "Name")
						continue;

					Character character = characterList.Find((obj) => obj.name == characterName);
					if (character != null)
					{
						character.description = description;
					}

					string[] snames = shortnames.Replace(" ", "").Split();
					foreach (string s in snames)
					{
						if (s == "") continue;
						characterNameTranslations.Add(s, characterName);
					}
					characterNameTranslations.Add(characterName, characterName);
				}
			}
        }

        // Conversations
        foreach (Character character in characterList)
        {
			if (character.name == "Player") continue;

			try
			{
				using (var reader = new StreamReader(@"Assets/Configuration/" + character.name + ".csv"))
				{
					using (var csvreader = new CsvReader(reader))
					{
						csvreader.Configuration.HasHeaderRecord = true;
						character.conversationStates.Clear();

						while (csvreader.Read())
						{
							string stateName = csvreader.GetField(0);
							string nextState = csvreader.GetField(1);
							string updateCondition = csvreader.GetField(2);
							string requireCondition = csvreader.GetField(3);
							string speaker = csvreader.GetField(4);
							string dialogue = csvreader.GetField(5);
							string action = csvreader.GetField(6);

							if (stateName == "") continue;
							if (stateName == "State Name") continue;

							ConversationState conversationState = new ConversationState();
							conversationState.character = character;

							conversationState.stateName = stateName;
							conversationState.updateCondition = updateCondition;
							conversationState.requireCondition = requireCondition;

							Character speakerCharacter = characterList.Find((obj) => obj.name == speaker);
							if (stateName != "Silent")
							{
								if (speakerCharacter == null)
									Debug.LogError(speaker + " not found on Character sheet " + character.name + " State: " + stateName);
								Debug.Assert(speakerCharacter != null);
							}
							conversationState.speaker = speakerCharacter;
							conversationState.dialogue = dialogue;

							character.conversationStates.Add(stateName, conversationState);
						}
					}
				}
			} catch (FileNotFoundException ex) {
			    Debug.LogWarning(ex.ToString());
				continue;
            }

			// Connnect the states
			try
			{
				using (var reader = new StreamReader(@"Assets/Configuration/" + character.name + ".csv"))
				{
					using (var csvreader = new CsvReader(reader))
					{
						csvreader.Configuration.HasHeaderRecord = true;

						while (csvreader.Read())
						{
							string stateName = csvreader.GetField(0);
							string nextState = csvreader.GetField(1);

							string[] nextStateSplit = nextState.Replace(" ", "").Split(',');
							if (nextStateSplit[0] == "") nextStateSplit[0] = "Silent";

							if (stateName == "State Name") continue;
							if (stateName == "") continue;

							if (!character.conversationStates.ContainsKey(stateName))
							{
								Debug.LogError(character.name + "is missing state " + stateName);
							}
							Debug.Assert(character.conversationStates.ContainsKey(stateName));
							foreach (string s in nextStateSplit)
							{
								Debug.Assert(s != "");
								Debug.Assert(character.conversationStates[s] != null);
								character.conversationStates[stateName].nextStates.Add(character.conversationStates[s]);
							}
						}
					}
				}
			} catch (FileNotFoundException ex) {
				Debug.LogWarning(ex.ToString());
				continue;
			}

            Debug.Assert(character.conversationStates["Silent"] != null);
            character.currentConversationState = character.conversationStates["Silent"];
        }
    }

	public bool gamePaused = false;

    public void Pause() {
        gamePaused = true;
    }

    public void Resume() {
        gamePaused = false;
    }
    
	public void Quit () {
	}

    public string[] CsvSplit(string input) {
        string pattern = "(?:^|,)(?=[^\"]|(\")?)\"?((?(1)[^\"]*|[^,\"]*))\"?(?=,|$)";

        List<string> result = new List<string>();

        MatchCollection collection = Regex.Matches(input, pattern);
        foreach(Match m in collection) {
            Group g = m.Groups[2];
            if(g != null) {
                result.Add(g.Value);
            }
        }

        return result.ToArray();
    }

    // Command Line Add Game Task
	public void AddGameTask(string commandString) {
		List<GameTask> tasks = GameTask.CreateGameTasks(commandString);
		foreach(GameTask task in tasks) {
			gameTasks.Enqueue(task);
		}
	}

	IEnumerator GameTaskUpdate() {
		while(true) {
			if (gameTasks.Count == 0) {
				yield return new WaitForSeconds(0.1f);
				continue;
			}

    		GameTask gameTask = gameTasks.Peek();
            gameTask.Execute();
            
			yield return new WaitForSeconds(gameTask.duration);
			//Debug.Log("Finished Task: " + gameTask.actionString);
			     
			gameTasks.Dequeue();
		}
	}
}