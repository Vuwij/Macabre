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
		const string serializationURI = "/GameData/SaveInfo.xml";

		public Save current;

		public Saves () {
			DeserializeSaveFile ();
			current = saves.Last();
		}
        
		~Saves () {
			SerializeSaveFile ();
		}

		public List<Save> saves;

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

		#region Serialization

		XmlSerializer x = new XmlSerializer (typeof(List<Save>));

		void DeserializeSaveFile ()
		{
//			if (!File.Exists (Application.dataPath + serializationURI)) {
//				SerializeSaveFile ();
//			}
//
//			using (var stream = File.OpenRead (Application.dataPath + serializationURI))
//				saves = ()(x.Deserialize (stream));
//
//			if (saves == null) {
//				saves = new AllSaveInformation ();
//				SerializeSaveFile ();
//			}
		}

		void SerializeSaveFile ()
		{
			File.Delete (Application.dataPath + serializationURI);

			using (var stream = File.OpenWrite (Application.dataPath + serializationURI))
				x.Serialize (stream, saves);
		}

		#endregion
	}
}