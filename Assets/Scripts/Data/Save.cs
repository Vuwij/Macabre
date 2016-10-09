using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Data
{

    [XmlRoot("Save Info")]
    [XmlInclude(typeof(Save))]
    public struct SaveInfo
    {
        public int saveNumber { get; set; }
        public List<Save> saveList { get; set; }
        public Save currentSave { get; set; }       // The current save, last loaded
    }

    [System.Serializable]
    [XmlInclude(typeof(MacabreDateTime))]
    public class Save : IDisposable
    {
        private static string saveURI = Application.dataPath + "/Databases/Saves";
        private static string masterURI = Application.dataPath + "/Databases/Master";

        public static SaveInfo saveInfo;
        public static bool saveInfoLock = false;

        // The current save informaiton
        public int saveID;
        public System.DateTime saveTime;
        public MacabreDateTime gameTime;
        public string saveLocation;
        public string saveName;

        // The information saved
        [XmlIgnore]
        private XmlSerializer gameDataSerializer;

        [XmlIgnore]
        public GameData gameData;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Save"/> class.
        /// </summary>
        private Save()
        {
            Save.saveInfo.saveNumber++;
            saveID = Save.saveInfo.saveNumber;
            saveLocation = GetSaveURI();
            saveTime = GetSaveTime();
            gameTime = GameClock.time;
            saveName = "Save" + saveID;
            Debug.Log("Getting information for " + saveID);

            if (!saveInfoLock) saveInfo.saveList.Add(this);
            if (!saveInfoLock) saveInfo.currentSave = this;
            if (!saveInfoLock) this.copyDatabases();
        }

        /// <summary>
        /// Creates a new game
        /// </summary>
        public Save(bool newSave)
        {
            Save.saveInfo.saveNumber++;
            saveID = Save.saveInfo.saveNumber;
            saveLocation = GetSaveURI();
            saveTime = GetSaveTime();
            gameTime = new MacabreDateTime(true);
            saveName = "Save" + saveID;
            if (!saveInfoLock) saveInfo.saveList.Add(this);
            if (!saveInfoLock) saveInfo.currentSave = this;
            if (!saveInfoLock) this.copyDatabases();
            if (!saveInfoLock) this.newGameData();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Save"/> class.
        /// </summary>
        public Save(string saveName_)
        {
            Save.saveInfo.saveNumber++;
            saveID = Save.saveInfo.saveNumber;
            saveLocation = GetSaveURI();
            saveTime = GetSaveTime();
            gameTime = new MacabreDateTime(true);
            saveName = saveName_;

            if (!saveInfoLock) saveInfo.saveList.Add(this);
            if (!saveInfoLock) saveInfo.currentSave = this;
            if (!saveInfoLock) this.copyDatabases();
        }

        bool disposed = false;

        /// <summary>
        /// Releases all resource used by the <see cref="T:Save"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="T:Save"/>. The <see cref="Dispose"/> method
        /// leaves the <see cref="T:Save"/> in an unusable state. After calling <see cref="Dispose"/>, you must release all
        /// references to the <see cref="T:Save"/> so the garbage collector can reclaim the memory that the
        /// <see cref="T:Save"/> was occupying.</remarks>
        public void Dispose()
        {
            this.deleteDatabases();
            saveInfo.saveList.Remove(this);
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
            }
            // Free any unmanaged objects here.
            disposed = true;
        }


        #endregion

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

        public static Save NewGame()
        {
            if (Save.saveInfo.saveList.IsNullOrEmpty())
            {
                Save.saveInfo.saveList = new List<Save>();
            }
            Debug.Log("New Game");
            return new Save(true);
        }

        public static Save NewSave()
        {
            if (Save.saveInfo.saveList.IsNullOrEmpty())
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