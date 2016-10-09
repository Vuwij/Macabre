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
    public class SaveManager
    {
        public static SaveManager main;

        // New Game and Load game are set in the GameManager
        [HideInInspector]
        public bool newGame;
        [HideInInspector]
        public bool loadGame;
        [HideInInspector]

        // Auto Serialize serializes the game when the ng
        public bool autoSerialize = true;

        // Important statistics for the save manager

        #region Initialize

        void OnEnable()
        {
            if (!main) main = this;
            else Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }

        void Awake()
        {
            // Sets up and deserializes the save file
            SetUpSerialization();
            if (Settings.enableSaving)
            {
                Debug.Log("Saving Enabled ... Attempting to retrieve save file");

                // Make an attempt to deserialize the save file, otherwise create a new save
                if (DeserializeSaveFile())
                {
                    Debug.Log("Save file retrieved");
                }
                else
                {
                    Debug.Log("Save file doesn't exist or is corrupt, please fix code");
                }
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

        void OnApplicationQuit()
        {
            if (autoSerialize)
            {
                SerializeSaveFile();
            }
        }

        #endregion

        #region Serialization

        private XmlSerializer x;
        private string serializationURI;

        /// <summary>
        /// Sets up serialization.
        /// </summary>
        /// <returns>The up serialization.</returns>
        private void SetUpSerialization()
        {
            x = new XmlSerializer(typeof(SaveInfo));
            serializationURI = Application.dataPath + "/Databases/SaveInfo.xml";
        }

        /// <summary>
        /// Deserializes the save file.
        /// </summary>
        /// <returns><c>true</c>, if save file exists, <c>false</c> If save file is corrupt or doesn't exist</returns>
        private bool DeserializeSaveFile()
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
        private bool SerializeSaveFile()
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

        #endregion

        #region Save Method

        private Save GetCurrentSave()
        {
            return Save.saveInfo.currentSave;
        }

        public void ViewAllSaves()
        {

        }

        #endregion

        #region Pause Screen

        public void ToggleSaveMenu()
        {
            UIManager.main.ToggleScreen("Save Screen");
            UIManager.main.ToggleScreen("Pause Screen");
            DisplaySaveScreen();
        }

        /// <summary>
        /// Displays the entire save screen
        /// </summary>
        private void DisplaySaveScreen()
        {
            GameObject SaveBackground = GameObject.Find("Save Background");
            if (SaveBackground == null)
            {
                Debug.LogError("Error: Save Content not found, unable to load saves on save manager");
            }

            // Resize the size of the save background based on the number of entries
            var saveBox = SaveBackground.GetComponent<RectTransform>();
            Debug.Log(Save.saveInfo.saveList.Count);
            if (Save.saveInfo.saveList.Count != 0)
            {
                saveBox.sizeDelta = new Vector2(saveBox.sizeDelta.x, Save.saveInfo.saveList.Count * 40);
            }
            else
            {
            }

            // Delete all the saves
            foreach (Transform child in SaveBackground.transform)
            {
                Destroy(child.gameObject);
            }

            // Now load all the saves one by one
            int yPosition = -40;
            foreach (Save s in Save.saveInfo.saveList)
            {

                // The position of the new position
                GameObject save = (GameObject)Instantiate(Resources.Load("Save"));
                save.transform.SetParent(SaveBackground.transform);
                save.GetComponent<ButtonExtensionSave>().saveID = s.saveID; // Matches the two saveIDs

                // Set the position of the save
                var rectTransform = save.GetComponent<RectTransform>();
                rectTransform.offsetMax = new Vector2(0, 0);
                rectTransform.offsetMin = new Vector2(0, 0);
                rectTransform.sizeDelta = new Vector2(0, 40);
                rectTransform.anchoredPosition = new Vector2(
                    save.GetComponent<RectTransform>().anchoredPosition.x,
                    yPosition
                );

                yPosition -= 40;

                // The name and date
                var textList = save.GetComponentsInChildren<Text>();
                foreach (Text t in textList)
                {
                    if (t.gameObject.name == "Save Name")
                    {
                        t.text = s.saveName;
                    }
                    if (t.gameObject.name == "Save Date")
                    {
                        t.text = s.saveTime.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Create a new save, does not override
        /// </summary>
        //TODO Start the save numbering by 1
        public void CreateSavePauseScreen()
        {
            var currentSave = Save.NewSave();
            currentSave.save();

            DisplaySaveScreen();
        }

        /// <summary>
        /// Saves the game data into the current save
        /// </summary>
        public void SaveSavePauseScreen()
        {
            // Checks if the no save has been selected
            if (ButtonExtensionSave.selectedSaveID == 0)
            {
                Debug.Log("No save selected");
                return;
            }
            Save s = Save.GetSaveFromID(ButtonExtensionSave.selectedSaveID);
            if (s == null) Debug.LogError("Error - Save does not exist");
            s.save();
        }

        /// <summary>
        /// Takes the current save and load it into the game
        /// </summary>
        public void LoadSavePauseScreen()
        {
            // Checks if the no save has been selected
            if (ButtonExtensionSave.selectedSaveID == 0)
            {
                Debug.Log("No save selected");
                return;
            }
            Save s = Save.GetSaveFromID(ButtonExtensionSave.selectedSaveID);
            if (s == null) Debug.LogError("Error - Save does not exist");
            s.load();
        }

        /// <summary>
        /// Takes a save and renames it
        /// </summary>
        //	HACK Need to add prompt where user inputs the save name
        //	HACK Currently used to create a new save
        public void RenameSavePauseScreen()
        {
            // Checks if the no save has been selected
            if (ButtonExtensionSave.selectedSaveID == 0)
            {
                Debug.Log("No save selected");
                return;
            }
            SaveSavePauseScreen();
            DisplaySaveScreen();
        }

        /// <summary>
        /// Deletes the current save selected
        /// </summary>
        public void DeleteSavePauseScreen()
        {
            // Checks if the no save has been selected
            if (ButtonExtensionSave.selectedSaveID == 0)
            {
                Debug.Log("No save selected");
                return;
            }
            Save s = Save.GetSaveFromID(ButtonExtensionSave.selectedSaveID);
            if (s == null) Debug.LogError("Error - Save does not exist");

            //TODO need stuff here lol, put s.Dispose into itå
            string[] options = { "Yes", "No" };
            UIManager.Warning("Warning, Current Save being deleted, do you wish to continue", options, new UnityEngine.Events.UnityAction[] {
            () => { DeleteSavePauseScreenConfirm(s); },
            () => { Debug.Log("Game Not Saved"); return; }
        });

            ButtonExtensionSave.selectedSaveID = 0;
        }

        private void DeleteSavePauseScreenConfirm(Save s)
        {
            Debug.Log("Disposing Game");
            s.Dispose();
            DisplaySaveScreen();
            if (Save.saveInfo.currentSave.saveID == s.saveID)
            {
                s.Dispose();
            }
        }

        #endregion

    }
}