using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using Environment;
using Objects.Immovable;
using System.Linq;
using Objects.Movable.Characters;
using System.Xml.Serialization;

namespace Data
{
	[Serializable]
	public class Save
	{
		public string fileLocation
		{
			get { return saveURI + "/" + name; }
		}
		public string database
		{
			get { return fileLocation + "/MacabreDB.db3"; }
		}
		public string gameData
		{
			get { return fileLocation + "/gameData.json"; }
		}
		public string saveURI {
			get {
				return Game.dataPath + "/GameData/Saves";
			}
		}
		public string masterURI {
			get {
				return Game.dataPath + "/GameData/Master";
			}
		}

		// Save information
		public float time;
		public string name;

		// Game information
		[XmlIgnore]
		public List<Character> characters;
		protected Save(){}

		public Save(string name = "")
		{
			time = Time.time;
			if (name != "") this.name = name;
			else this.name = "Save " + System.DateTime.Now;

			Directory.CreateDirectory(fileLocation);
		}

		void Add(Save s) {}

		#region New, Load, Save, Delete 

		public void NewGame()
		{
			File.Copy(masterURI + "/MacabreDB.master.db3", fileLocation + "/MacabreDB.db3", true);
		}

		public void LoadGame()
		{
		}

		public void SaveGame()
		{
		}

		public void DeleteGame()
		{
		}

		#endregion
	}
}