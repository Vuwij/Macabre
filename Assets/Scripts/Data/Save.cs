using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Environment.Time;
using Objects.Inanimate.World;
using System.Linq;

namespace Data
{
    public class Save
    {
        public static string saveURI = Application.dataPath + "/Databases/Saves";
        public static string masterURI = Application.dataPath + "/Databases/Master";

        // Save information auto increments
        public System.DateTime time;
        public string name;

        // The location of the file on file
        public string fileLocation
        {
            get { return saveURI + "/" + name; }
        }
        public string databaseLocation
        {
            get { return fileLocation + "/MacabreDB.db"; }
        }
        public string worldXMLLocation
        {
            get { return fileLocation + "/world.xml"; }
        }

        // The information saved
        protected Save() { }
        public Save(string name = "")
        {
            time = System.DateTime.Now;
            if (name != "") this.name = name;
            else this.name = "Save " + System.DateTime.Now;
            InitializeData();
        }

        public void Delete()
        {
            Directory.Delete(fileLocation, true);
        }
        
        public void InitializeData()
        {
            Directory.CreateDirectory(fileLocation);
            File.Copy(masterURI + "/MacabreDB.master.db3", fileLocation + "/MacabreDB.db3");
        }

        [XmlIgnore]
        private DataContractSerializer gameDataSerializer = new DataContractSerializer(typeof(MacabreWorld));

        [XmlIgnore]
        private string gameDataURI
        {
            get { return fileLocation + "/world.xml"; }
        }
        [XmlIgnore]
        public MacabreWorld world;
        
        public void NewGame()
        {
            Debug.Log("Creating " + name + "...");

            // Creates the world from classes
            if (GameSettings.useSavedXMLConfiguration)
                File.Copy(masterURI + "/world.xml", worldXMLLocation);
            else
            {
                GameSettings.createNewGame = true;
                world = new MacabreWorld();
                world.LoadAll();
                GameSettings.createNewGame = false;
            }

            // Saves the world
            SaveGame();
        }

        public void SaveGame()
        {
            Debug.Log("Saving " + name + "...");

            // Deletes the game data
            File.Delete(gameDataURI);
            
            // Make an attempt to serialize the Save class
            using (var stream = File.OpenWrite(gameDataURI))
                gameDataSerializer.WriteObject(stream, world);
        }

        public void LoadGame()
        {
            Debug.Log("Loading " + name + "...");
            
            // Serializes the game data
            if (!File.Exists(gameDataURI))
                throw new Exception("Attempting to deserialize save file, but Save File not found");

            // Make an attempt to serialize the Save class
            using (var stream = File.OpenRead(gameDataURI))
                world = (MacabreWorld)(gameDataSerializer.ReadObject(stream));

            world.LoadAll();
        }   
    }
}