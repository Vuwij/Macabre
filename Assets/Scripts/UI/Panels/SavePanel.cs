using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using Data;
using Extensions.Buttons;
using System;
using UI.Dialogues;

namespace UI.Panels
{
    public class SavePanel : UIPanel
    {
        private const int saveIconWidth = 40;
        private int saveCount
        {
            get
            {
                int c = SaveManager.allSaveInformation.SaveCount;
                if (c <= 0) throw new Exception("Save count too small");
                return c;
            }
        }
        private List<Save> saveList
        {
            get { return SaveManager.allSaveInformation.saveList; }
        }

        public override string name
        {
            get { return "Save Panel"; }
        }
        
        private Transform saveIconParent
        {
            get { return GameObject.Find("Save Background").transform; }
        }
        private RectTransform saveBackgroundBox
        {
            get { return saveIconParent.GetComponent<RectTransform>(); }
        }
        
        private string selectedSaveName;
        public Save selectedSave
        {
            get { return saveList.Find(x => x.name == selectedSaveName); }
        }
        public void SelectSave(object obj, EventArgs args)
        {
            // TODO select save action attach to button
            throw new NotImplementedException();
        }

        private void Refresh()
        {
            saveBackgroundBox.sizeDelta = new Vector2(saveBackgroundBox.sizeDelta.x, saveCount * saveIconWidth);

            // Delete all the saves in the transform
            foreach (Transform child in saveIconParent)
                UnityEngine.Object.Destroy(child.gameObject);

            // Now load all the saves one by one
            int yPosition = -40;
            foreach (Save s in saveList)
            {
                // The position of the new position
                GameObject save = UnityEngine.Object.Instantiate(Resources.Load("Save")) as GameObject;
                save.transform.SetParent(saveBackgroundBox.transform);
                save.GetComponent<ButtonExtensionSave>().saveID = s.ID;

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
                        t.text = s.name;
                    if (t.gameObject.name == "Save Date")
                        t.text = s.time.ToString();
                }
            }
        }

        public void CreateNewSave()
        {
            SaveManager.NewSave();
            Refresh();
        }

        public void SaveSelectedSave()
        {
            if (selectedSave == null) return;
            selectedSave.SaveGame();
        }

        public void LoadSave()
        {
            if (selectedSave == null) return;
            selectedSave.LoadGame();
        }

        public void RenameSave()
        {
            if (selectedSave == null) return;
            throw new NotImplementedException();
        }

        public void DeleteSave()
        {
            if (selectedSave == null) return;
            
            string message = "Warning, Current Save being deleted, do you wish to continue";
            WarningDialogue.Button yes = new WarningDialogue.Button("Yes", () => { DeleteSaveConfirm(selectedSave); });
            WarningDialogue.Button no = new WarningDialogue.Button("Yes", () => {});

            WarningDialogue.Open(message, new List<WarningDialogue.Button>() { yes, no });
        }

        private void DeleteSaveConfirm(Save s)
        {
            SaveManager.DeleteSave(s.name);
            Refresh();
        }
    }
}
