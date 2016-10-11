using UnityEngine;
using UnityEngine.UI;

using Data;
using Extensions.Buttons;

namespace UI.Panels
{
    public static class SavePanel
    {
        public static void ToggleSaveMenu()
        {
            UIManager.ToggleScreen("Save Screen");
            UIManager.ToggleScreen("Pause Screen");
            DisplaySaveScreen();
        }

        /// <summary>
        /// Displays the entire save screen
        /// </summary>
        private static void DisplaySaveScreen()
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
                saveBox.sizeDelta = new Vector2(saveBox.sizeDelta.x, Save.saveInfo.saveList.Count * 40);
            
            // Delete all the saves
            foreach (Transform child in SaveBackground.transform)
                GameObject.Destroy(child.gameObject);

            // Now load all the saves one by one
            int yPosition = -40;
            foreach (Save s in Save.saveInfo.saveList)
            {

                // The position of the new position
                GameObject save = (GameObject)GameObject.Instantiate(Resources.Load("Save"));
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
                        t.text = s.saveName;
                    if (t.gameObject.name == "Save Date")
                        t.text = s.saveTime.ToString();
                }
            }
        }

        /// <summary>
        /// Create a new save, does not override
        /// </summary>
        //TODO Start the save numbering by 1
        public static void CreateSavePauseScreen()
        {
            var currentSave = Save.NewSave();
            currentSave.save();

            DisplaySaveScreen();
        }

        /// <summary>
        /// Saves the game data into the current save
        /// </summary>
        public static void SaveSavePauseScreen()
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
        public static void LoadSavePauseScreen()
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
        public static void RenameSavePauseScreen()
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
        public static void DeleteSavePauseScreen()
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
            WarningPanel.Warning("Warning, Current Save being deleted, do you wish to continue", options, new UnityEngine.Events.UnityAction[] {
            () => { DeleteSavePauseScreenConfirm(s); },
            () => { Debug.Log("Game Not Saved"); return; }
        });

            ButtonExtensionSave.selectedSaveID = 0;
        }

        private static void DeleteSavePauseScreenConfirm(Save s)
        {
            Debug.Log("Disposing Game");
            s.Dispose();
            DisplaySaveScreen();
            if (Save.saveInfo.currentSave.saveID == s.saveID)
            {
                s.Dispose();
            }
        }
    }
}
