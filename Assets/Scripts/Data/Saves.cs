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
		const string serializationURI = "/GameData/saves.xml";

		public Save current;
		public List<Save> saves;

		public Saves () {
			DeserializeSaveFile ();
			if (saves.Count != 0)
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

			XmlSerializer x = new XmlSerializer(typeof(List<Save>));
			using (StreamReader r = new StreamReader(Game.dataPath + serializationURI))
				saves = (List<Save>) x.Deserialize(r);
		}

		void SerializeSaveFile ()
		{
			File.Delete (Game.dataPath + serializationURI);

			XmlSerializer x = new XmlSerializer(typeof(List<Save>));
			using (StreamWriter w = new StreamWriter(Game.dataPath + serializationURI))
				x.Serialize(w, saves);
		}

		#endregion

		public void New (string name = "")
		{
			if(saves.Find(x => x.name == name) != null) {
				// Overwriting the file
				saves.Find(x => x.name == name).NewGame();
				return;
			}

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