using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UI;
using UI.Screens;
using UnityEngine.SceneManagement;

namespace Data
{
	[XmlInclude (typeof(Save))]
	public class Saves
	{
		const string serializationURI = "/GameData/Saves.json";

		public Save current;
		public List<Save> saves;

		public Saves () {
			DeserializeSaveFile ();
			current = saves.Last();
		}
        
		~Saves () {
			SerializeSaveFile ();
		}

		#region Serialization

		void DeserializeSaveFile ()
		{
			if (!File.Exists (Game.dataPath + serializationURI)) {
				saves = new List<Save>();
				SerializeSaveFile ();
			}

			string json = File.ReadAllText(Game.dataPath + serializationURI);
			saves = JsonUtility.FromJson<List<Save>>(json);
		}

		void SerializeSaveFile ()
		{
			File.Delete (Game.dataPath + serializationURI);
			string json = JsonUtility.ToJson(saves, true);
			File.WriteAllText(Application.dataPath, json);
		}

		#endregion

		public void New (string name = "")
		{
			if (name == "")
				name = "Save " + (saves.Count + 1);
            
			current = new Save (name);
			saves.Add (current);
			SerializeSaveFile ();
			current.NewGame ();
		}

		public void Load (string name = "")
		{
			Game.main.UI.Find<LoadingScreen> ().TurnOn ();

			if (name == "")
				current = saves.Last();
			else
				current = saves.Find (x => x.name == name);
			if (current == null)
				throw new Exception ("Save " + name + " not found");

			SerializeSaveFile ();
			current.LoadGame ();
			Game.main.UI.Find<LoadingScreen> ().TurnOff ();
		}

		public void Delete (string name)
		{
			if (name == current.name)
				throw new Exception ("You are not allowed to delete the save when you are in the game");

			Save s = saves.Find (x => x.name == name);
			saves.Remove (s);
			SerializeSaveFile ();
			s = null;
		}

		public Save Find (string name)
		{
			return this.saves.Find (x => x.name == name);
		}

		public void DeleteAll ()
		{
			DirectoryInfo di = new DirectoryInfo (Application.dataPath + serializationURI);
			foreach (DirectoryInfo dir in di.GetDirectories())
				dir.Delete (true);
			File.Delete (serializationURI);
		}
	}
}