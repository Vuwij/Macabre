using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

using Environment.Time;

//TODO fix saving system
namespace Data
{
    [System.Serializable]
    [XmlInclude(typeof(MacabreDateTime))]
    public class Save
    {
        private static string saveURI = Application.dataPath + "/Databases/Saves";
        private static string masterURI = Application.dataPath + "/Databases/Master";

        public static SaveInfo saveInfo;
        public static bool saveInfoLock = false;

        // The current save informaiton
        public int saveID
        {
            get
            {
                return Save.saveInfo.saveNumber;
            }
            set { }
        }
        public System.DateTime saveTime
        {
            get { return GetSaveTime(); }
            set { }
        }
        public string saveLocation
        {
            get { return GetSaveURI(); }
            set { }
        }
        public string saveName
        {
            get { return "Save" + saveID; }
            set { }
        }

        // The information saved
        [XmlIgnore]
        private XmlSerializer gameDataSerializer;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Save"/> class.
        /// </summary>
        private Save(string saveName_ = "", bool newSave = false)
        {
            Save.saveInfo.saveNumber++;
            Debug.Log("Getting information for " + saveID);

            if (saveName != "") saveName_ = saveName;

            if (!saveInfoLock) saveInfo.saveList.Add(this);
            if (!saveInfoLock) saveInfo.currentSave = this;
            if (!saveInfoLock) this.copyDatabases();
            if (newSave) this.newGameData();
        }
        
        #region Manipulation

        public static void lockSaveInfo()
        {
            saveInfoLock = true;
        }

        public static void unlockSaveInfo()
        {
            saveInfoLock = false;
        }

        public static Save GetSaveFromID(int saveID_)
        {
            return saveInfo.saveList.Find(s => s.saveID == saveID_);
        }

        #endregion

        #region Saving

        public static Save NewSave()
        {
            Save.saveInfo.saveList = new List<Save>();

            Debug.Log("New Save");
            return new Save();
        }

        public static void SaveFile()
        {
            saveInfo.currentSave.save();
        }

        public void save()
        {
            Debug.Log("Game " + saveName + " saved");

            // Retrieve all information and store into GameData
            GameData.SaveAllInfo();

            // Serializes the game data
            gameDataSerializer = new XmlSerializer(typeof(GameData));
            string saveDataURI = saveLocation + "/game.xml";
            File.Delete(saveDataURI);

            // Make an attempt to serialize the Save class
            try
            {
                using (var stream = File.OpenWrite(saveDataURI))
                {
                    gameDataSerializer.Serialize(stream, GameData.main);
                }
            }
            catch (IOException)
            {
                Debug.LogError("Error when serializing save file");
            }
        }

        private void copyDatabases()
        {
            Directory.CreateDirectory(saveLocation);
            File.Copy(masterURI + "/Conversations.master.db", saveLocation + "/Conversations.db");
            File.Copy(masterURI + "/Inspect.master.db", saveLocation + "/Inspect.db");
            File.Copy(masterURI + "/ItemCombine.master.db", saveLocation + "/ItemCombine.db");
            File.Copy(masterURI + "/Scenes.master.db", saveLocation + "/Scenes.db");
        }

        private void deleteDatabases()
        {
            File.Delete(saveLocation + "/Conversations.db");
            File.Delete(saveLocation + "/Inspect.db");
            File.Delete(saveLocation + "/ItemCombine.db");
            File.Delete(saveLocation + "/Scenes.db");
            Directory.Delete(saveLocation, true);
        }

        private void newGameData()
        {
            // Place the static gameData as the new data
            gameData = new GameData(this);
            Debug.Log("Loading game objects");
            Loader.LoadAllGameComponents();

            DatabaseManager.main.LoadDatabases();
        }

        private static System.DateTime GetSaveTime()
        {
            return System.DateTime.Now;
        }

        private static string GetSaveURI()
        {
            string fileURI = saveURI + "/" + string.Format("Save-{0:yyyy-MM-dd_hh-mm-ss-fff-tt}.bin", System.DateTime.Now);
            Debug.Log("Save location is: " + fileURI);
            return fileURI;
        }

        #endregion

        #region Loading

        public static void Load()
        {
            saveInfo.currentSave.load();
        }

        public void load()
        {
            Debug.Log("Game " + saveName + " loaded");

            // Serializes the game data
            gameDataSerializer = new XmlSerializer(typeof(GameData));
            string saveDataURI = saveLocation + "/game.xml";

            // Check if the file exists
            if (!File.Exists(saveDataURI))
            {
                Debug.LogWarning("Attempting to deserialize save file, but Save File not found");
                return;
            }

            // Make an attempt to serialize the Save class
            try
            {
                GameData.main.gameData.Clear();
                using (var stream = File.OpenRead(saveDataURI))
                {
                    GameData.main = (GameData)(gameDataSerializer.Deserialize(stream));
                }
            }
            catch (IOException)
            {
                Debug.LogError("Error when deserializing save file");
            }

            // Delete all the game data
            Loader.ResetAllGameComponents();

            // Retrieve all information and store into GameData
            GameData.LoadAllInfo();
        }

        #endregion

        #region Testing

        public static void TestSave()
        {
            // Create 10 saves
            new Save();
            new Save();
            new Save();
            new Save();
            new Save();
            new Save();
            new Save();
            new Save();
            new Save();
            new Save();
        }

        #endregion

    }
}