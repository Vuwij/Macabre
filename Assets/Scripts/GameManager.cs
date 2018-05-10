using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UI;
using Data;
using Objects;
using Environment;
using UI.Panels;
using UI.Dialogues;
using System.IO;
using System.Linq;

/// <summary>
/// The Game Manager is responsible for loading everything in the correct order. Individual files do not load themselves
/// </summary>
public class GameManager : MonoBehaviour {
	public static GameManager main = null;
	public static string dataPath;

	public Saves saves;
	public GameClock clock = new GameClock();
	public Database db;
	public EventList eventList;

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

    void LoadInventoryInformation() {

        PixelItem[] items = Resources.LoadAll<PixelItem>("Items");
        List<PixelItem> itemsList = items.ToList();
        Debug.Assert(items.Length != 0);

        // Item Descriptions
        using (var reader = new StreamReader(@"Assets/Configuration/Items.csv"))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                string[] values = line.Split(',');

                int id;
                int.TryParse(values[0], out id);
                Debug.Assert(id >= 0);

                string name = values[1];
                if (name == "Name")
                    continue;
                string description = values[2];
                string[] properties = values[3].Replace(" ", "").Split(',');

                PixelItem item = itemsList.Find((obj) => obj.name == name);
                if (item == null) continue;
                item.id = id;
                item.description = description;

                if(!((properties.Length == 0) || (properties[0] == ""))) {
                    item.properties = properties;
                } else {
                    item.properties = null;
                }
            }
        }

        // Item Combinations
        using (var reader = new StreamReader(@"Assets/Configuration/Item Combine.csv"))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                string[] values = line.Split(',');

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
        
    }

	public bool gamePaused = false;

    public void Pause() {
        gamePaused = true;
    }

    public void Resume()
    {
        gamePaused = false;
    }
    
	public void Quit () {
	}
}