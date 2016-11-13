using UnityEngine;
using System;
using System.IO;
using System.Xml.Serialization;
using Exceptions;
using UI;
using UI.Screens;
using UnityEngine.SceneManagement;

namespace Data
{
    // The controller of the SaveManger controls
    //      AllSaveInformation - A xml list of all the saves
    //      Save - a data model of a save information
    public static partial class SaveManager
    {
        private static bool autoSerialize = GameSettings.autoSerializeGame;
        public static AllSaveInformation allSaveInformation;
        private static Save currentSave;

        public static Save CurrentSave
        {
            get {
                return currentSave;
            }
            set
            {
                currentSave = value;
                allSaveInformation.lastSaveUsed = currentSave.name;
            }
        }
        
        // Sets up and deserializes the save file
        public static void Initialize()
        {
            if (!GameSettings.enableSaving) return;
            DeserializeSaveFile();
        }
        
        // Auto Serializes the save file when you quit
        public static void OnApplicationQuit()
        {
            if (!autoSerialize) return;
            SerializeSaveFile();
        }
        
        // Creates a new save
        public static Save NewSave(string name = "")
        {
            allSaveInformation.HighestSaveID++;
            if (name == "") name = "Save " + (allSaveInformation.SaveCount + 1);
            
            CurrentSave = new Save(name);
            allSaveInformation.saveList.Add(CurrentSave);
            SerializeSaveFile();

            CurrentSave.NewGame();
            return CurrentSave;
        }

        // If the name is empty then get the last save used
        // HACK : Loading not working
        public static Save LoadSave(string name = "")
        {
            UIManager.Find<LoadingScreen>().TurnOn();

            // Deletes the world from game
            currentSave = null;
            //SceneManager.UnloadScene("Game");
            //SceneManager.LoadScene("Game");

            // Loads the last save
            if (name == "") name = allSaveInformation.lastSaveUsed;
            CurrentSave = allSaveInformation.saveList.Find(x => x.name == name);
            if (CurrentSave == null) throw new Exception("Save " + name + " not found");
            SerializeSaveFile();

            CurrentSave.LoadGame();

            UIManager.Find<LoadingScreen>().TurnOff();
            return CurrentSave;
        }

        // Deletes a save
        public static void DeleteSave(string name)
        {
            if(name == CurrentSave.name)
                throw new MacabreUIException("You are not allowed to delete the save when you are in the game");

            Save s = allSaveInformation.saveList.Find(x => x.name == name);
            allSaveInformation.saveList.Remove(s);
            SerializeSaveFile();
            s = null;
        }

        // Finds a save
        public static Save FindSave(string name)
        {
            return allSaveInformation.saveList.Find(x => x.name == name);
        }

        // Delete all the saves, including the folder
        public static void Reset()
        {
            DirectoryInfo di = new DirectoryInfo(Save.saveURI);
            foreach (DirectoryInfo dir in di.GetDirectories()) dir.Delete(true);
            File.Delete(serializationURI);
        }

        // Saves the current as Master (WARNING Do not Use)
        public static void SaveCurrentAsMaster()
        {
            File.Copy(CurrentSave.worldXMLLocation, Save.masterURI + "/world.xml", true);
        }

        #region Save Serializer portion

        private static XmlSerializer x = new XmlSerializer(typeof(AllSaveInformation));
        private static string serializationURI
        {
            get
            {
                return Application.dataPath + "/Databases/SaveInfo.xml";
            }
        }
        
        private static void DeserializeSaveFile()
        {
            // Check if the file exists
            if (!File.Exists(serializationURI))
            {
                allSaveInformation = new AllSaveInformation();
                SerializeSaveFile();
            }

            // Deserialize the Save class
            using (var stream = File.OpenRead(serializationURI))
                allSaveInformation = (AllSaveInformation)(x.Deserialize(stream));

            if (allSaveInformation == null)
            {
                allSaveInformation = new AllSaveInformation();
                SerializeSaveFile();
            }
        }

        private static void SerializeSaveFile()
        {
            // Delete the save file
            File.Delete(serializationURI);

            // Make an attempt to serialize the Save class
            using (var stream = File.OpenWrite(serializationURI))
                x.Serialize(stream, allSaveInformation);
        }
        
        #endregion
    }
}