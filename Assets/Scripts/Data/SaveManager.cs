using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Data
{
    // The controller of the SaveManger controls
    //      AllSaveInformation - A xml list of all the saves
    //      Save - a data model of a save information
    public static partial class SaveManager
    {
        private static bool autoSerialize = GameSettings.autoSerializeGame;
        public static AllSaveInformation allSaveInformation;
        public static Save currentSave;

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
            if (name == "") name = "Save " + allSaveInformation.SaveCount;
            allSaveInformation.HighestSaveID++;

            currentSave = new Save(name);
            allSaveInformation.saveList.Add(currentSave);
            return currentSave;
        }

        // If the name is empty then get the last save used
        public static Save LoadSave(string name = "")
        {
            if (name == "") name =  allSaveInformation.lastSaveUsed;
            currentSave = allSaveInformation.saveList.Find(x => x.name == name);
            if (currentSave == null) throw new Exception("Save " + name + " not found");
            return currentSave;
        }

        // Deletes a save
        public static void DeleteSave(string name)
        {
            if(name == currentSave.name)
            {
                // TODO: Don't allow deletion when you are in the current save
                return;
            }

            Save s = allSaveInformation.saveList.Find(x => x.name == name);
            allSaveInformation.saveList.Remove(s);
            s = null;
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