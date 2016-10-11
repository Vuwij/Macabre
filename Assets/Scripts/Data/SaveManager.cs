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
    public static class SaveManager
    {
        public static bool autoSerialize = GameSettings.autoSerializeGame;

        public static void Initialize()
        {
            // Sets up and deserializes the save file
            if (GameSettings.enableSaving)
            {
                Debug.Log("Saving Enabled ... Attempting to retrieve save file");

                // Make an attempt to deserialize the save file, otherwise create a new save
                if (DeserializeSaveFile()) Debug.Log("Save file retrieved");
                else Debug.Log("Save file doesn't exist or is corrupt, please fix code");
            }
        }

        public static void NewGame()
        {
            Save.NewGame();
        }

        public static void LoadGame()
        {
            Save.saveInfo.currentSave.load();
        }

        public static void OnApplicationQuit()
        {
            if (autoSerialize)
            {
                SerializeSaveFile();
            }
        }
        
        private static XmlSerializer x = new XmlSerializer(typeof(SaveInfo));
        private static string serializationURI
        {
            get
            {
                return Application.dataPath + "/Databases/SaveInfo.xml";
            }
        }
        
        /// <summary>
        /// Deserializes the save file.
        /// </summary>
        /// <returns><c>true</c>, if save file exists, <c>false</c> If save file is corrupt or doesn't exist</returns>
        private static bool DeserializeSaveFile()
        {

            // Check if the file exists
            if (!File.Exists(serializationURI))
            {
                Debug.LogWarning("Attempting to deserialize save file, but Save File not found");
                return false;
            }

            // Make an attempt to deserialize the Save class
            try
            {
                Save.lockSaveInfo();
                using (var stream = File.OpenRead(serializationURI))
                {
                    Debug.Log("Save File Deserialized successfully at: " + serializationURI);
                    Save.saveInfo = (SaveInfo)(x.Deserialize(stream));
                }
                Save.unlockSaveInfo();
                return true;
            }
            catch (IOException)
            {
                Debug.LogError("Error when deserializing save file");
                return false;
            }
        }

        /// <summary>
        /// Serializes the save file.
        /// </summary>
        /// <returns><c>true</c>, if save file was serialized, <c>false</c> otherwise.</returns>
        private static bool SerializeSaveFile()
        {
            // Delete the save file for assurance
            File.Delete(serializationURI);

            // Make an attempt to serialize the Save class
            try
            {
                using (var stream = File.OpenWrite(serializationURI))
                {
                    x.Serialize(stream, Save.saveInfo);
                }
                return true;
            }
            catch (IOException)
            {
                Debug.LogError("Error when serializing save file");
                return false;
            }
        }
        
        private static Save GetCurrentSave()
        {
            return Save.saveInfo.currentSave;
        }

    }
}