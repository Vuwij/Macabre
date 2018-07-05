using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using Objects;
using Data;
using System.IO;

public class MacabreItem : EditorWindow {
    
	[MenuItem ("Macabre/Items/Create All Items")]
	static void CreateAllItems() {
		PixelItem[] existingItems = Resources.LoadAll<PixelItem>("Items");
		List<PixelItem> itemsList = existingItems.ToList();
		Debug.Assert(existingItems.Length != 0);

		PixelItem defaultObject = itemsList.Find(x => x.name == "Carrot");

		UnityEngine.Object[] sprites = AssetDatabase.LoadAllAssetsAtPath("Assets/Spritesheets/Items/Generic.png");

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
					GameObject newGameObject = null;
					if (item == null) {
						// Create new item
						newGameObject = Instantiate(defaultObject.gameObject);
						newGameObject.name = objname;
						item = newGameObject.GetComponent<PixelItem>();
					}

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
                    
					if(newGameObject != null) {
						SpriteRenderer sr = newGameObject.GetComponent<SpriteRenderer>();
						sr.sprite = sprites.Where((x) => x.name == "Generic_" + (id - 1)).First() as Sprite;

						UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/Resources/Items/" + objname + ".prefab");
						PrefabUtility.ReplacePrefab(newGameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
						DestroyImmediate(newGameObject);
					}
				}
			}
		}
	}
}