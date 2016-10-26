using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

using Environment.Time;
using Objects.Inanimate.World;

namespace Data
{
    public class Save
    {
        private static string saveURI = Application.dataPath + "/Databases/Saves";
        private static string masterURI = Application.dataPath + "/Databases/Master";

        // Save information auto increments
        private int id;
        public System.DateTime time;
        public string name;

        // The location of the file on file
        public string fileLocation
        {
            get { return saveURI + "/Save " + id; }
        }
        public string databaseLocation
        {
            get { return fileLocation + "/MacabreDB.db"; }
        }

        // The information saved
        protected Save() { }
        public Save(string name = "")
        {
            id = SaveManager.allSaveInformation.HighestSaveID;
            time = System.DateTime.Now;
            if (name != "") this.name = name;
            else this.name = "Save " + System.DateTime.Now;
            InitializeDatabase();
            SaveGame();
        }

        ~Save()
        {
            Directory.Delete(fileLocation, true);
        }
        
        private void InitializeDatabase()
        {
            Directory.CreateDirectory(fileLocation);
            File.Copy(masterURI + "/MacabreDB.master.db3", fileLocation + "/MacabreDB.db3");
        }
        
        [XmlIgnore]
        private XmlSerializer gameDataSerializer = new XmlSerializer(typeof(MacabreWorld));
        [XmlIgnore]
        private string gameDataURI
        {
            get { return fileLocation + "/world.xml"; }
        }
        [XmlIgnore]
        public MacabreWorld world = new MacabreWorld();
        
        public void SaveGame()
        {
            Debug.Log("Saving Game...");

            // Delets the game data
            File.Delete(gameDataURI);

            // Make an attempt to serialize the Save class
            using (var stream = File.OpenWrite(gameDataURI))
                gameDataSerializer.Serialize(stream, world);
        }
        
        public void LoadGame()
        {
            Debug.Log("Loading Game...");

            // Serializes the game data
            if (!File.Exists(gameDataURI))
                throw new Exception("Attempting to deserialize save file, but Save File not found");

            // Make an attempt to serialize the Save class
            using (var stream = File.OpenRead(gameDataURI))
                world = (MacabreWorld)(gameDataSerializer.Deserialize(stream));
        }   
    }
}