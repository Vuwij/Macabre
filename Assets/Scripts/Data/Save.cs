using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using Environment;
using Objects.Unmovable;
using System.Linq;
using Objects.Movable.Characters;

namespace Data
{
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

		public string saveURI = Application.dataPath + "/GameData/Saves";
		public string masterURI = Application.dataPath + "/GameData/Master";
		public System.DateTime time;
		public string name;

		#region Game Data

		public List<Character> characters;

		#endregion

		protected Save(){}

		public Save(string name = "")
		{
			time = System.DateTime.Now;
			if (name != "") this.name = name;
			else this.name = "Save " + System.DateTime.Now;

			Directory.CreateDirectory(fileLocation);
			File.Copy(masterURI + "/MacabreDB.master.db3", fileLocation + "/MacabreDB.db3");
		}

		#region New, Load, Save, Delete 

		public void NewGame()
		{
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