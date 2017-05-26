using UnityEngine;
using System.Collections;
using Extensions.Buttons;
using UnityEngine.UI;
using Data;
using System;

namespace UI.Panels
{
    public class SaveSlot : MonoBehaviour
    {
        public Toggle toggle
        {
            get { return gameObject.GetComponent<Toggle>(); }
        }

        public SavePanel savePanel
        {
            get { return GameUI.Find<SavePanel>(); }
        }

        public Save save
        {
            get { return Saves.Find(name); }
        }

        private void Start()
        {
            toggle.onValueChanged.AddListener(OnClick);
        }

        private void OnClick(bool arg0)
        {
            if (!arg0) savePanel.selectedSave = null;
            else
            {
                savePanel.selectedSave = save;
                savePanel.UntoggleAll();
            }
        }
        
        public new string name
        {
            get
            {
                return GetTextWithName("Name");
            }
            set
            {
                SetTextWithName("Name", value);
            }
        }

        public string date
        {
            get
            {
                return GetTextWithName("Date");
            }
            set
            {
                SetTextWithName("Date", value);
            }
        }

        private string GetTextWithName(string name)
        {
            var texts = GetComponentsInChildren<Text>();
            foreach (Text t in texts)
                if (t.name == name)
                    return t.text;
            throw new MacabreUIException("Text missing on Save Slot");
        }

        private void SetTextWithName(string name, string setValue)
        {
            var texts = GetComponentsInChildren<Text>();
            foreach (Text t in texts)
            {
                if (t.name == name)
                {
                    t.text = setValue;
                    return;
                }
            }
            throw new MacabreUIException("Text missing on Save Slot");
        }
    }
}