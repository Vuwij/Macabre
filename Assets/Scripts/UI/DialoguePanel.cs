using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    class DialoguePanel
    {
        private GameObject dialogueScreen;
        private CanvasGroup dialogueScreenGroup;
        private Text mainText;
        private Text titleText;
        private Text returnButtonText;
        private Image mainImage;
        private GameObject returnButton;
        private GameObject responseButtons;
        private Text[] responseTexts;

        private string returnButtonTextString = "Press Space to Continue";

        /// <summary>
        /// Closes the dialogue screen.
        /// </summary>
        /// <returns><c>true</c>, if dialogue screen was closed, <c>false</c> otherwise.</returns>
        public bool CloseDialogueScreen()
        {
            mainText.text = "";
            titleText.text = "";
            mainImage.sprite = null;
            ToggleResponseButtons(false);
            ToggleReturnButton(false);
            for (int i = 0; i < 4; i++) responseButtons.GetComponentsInChildren<Text>()[i].text = "";
            StartCoroutine("HideInspectTextBox");
            return true;
        }

        ///<summary>For displaying the dialogue screen</summary>
        public bool DisplayDialogueScreen(Sprite mainImageEntry, string titleTextEntry, string mainTextEntry, bool continueText)
        {
            ResetDialogueScreen();

            mainText.text = mainTextEntry;
            titleText.text = titleTextEntry;
            mainImage.sprite = mainImageEntry;
            if (continueText)
                ToggleReturnButton(true);
            returnButtonText.text = "Return";
            StartCoroutine("ShowInspectTextBox");
            return true;
        }

        ///<summary>Dialogue Screen for Character Speaking</summary>
        public bool DisplayCharacterText(Sprite mainImageEntry, string characterName, string textEntry, bool continueText)
        {
            ResetDialogueScreen();

            if (mainImageEntry != null) mainImage.sprite = mainImageEntry;
            returnButtonText.text = returnButtonTextString;
            titleText.text = characterName;
            mainText.text = textEntry;
            StartCoroutine("ShowInspectTextBox");
            return true;
        }

        ///<summary>Dialogue Screen for Character Speaking Single</summary>
        public bool DisplaySingleResponse(Sprite mainImageEntry, string characterName, string textEntry, bool continueText)
        {
            ResetDialogueScreen();

            if (mainImageEntry != null) mainImage.sprite = mainImageEntry;
            returnButtonText.text = returnButtonTextString;

            titleText.text = characterName;
            Debug.LogWarning(textEntry);
            mainText.text = textEntry;
            StartCoroutine("ShowInspectTextBox");
            return true;
        }

        ///<summary>Dialogue Screen for Character Speaking Multiple Respones</summary>
        public bool DisplayMultipleResponse(Sprite mainImageEntry, string characterName, string[] textEntry, bool continueText)
        {
            ResetDialogueScreen();

            if (mainImageEntry != null) mainImage.sprite = mainImageEntry;
            returnButtonText.text = returnButtonTextString;

            ToggleResponseButtons(true);
            Text[] texts = responseButtons.GetComponentsInChildren<Text>();
            for (int i = 0; i < 4; i++)
            {
                texts[i].text += (textEntry[i] == null) ? "" : (i + 1).ToString();
                texts[i].text += ". " + textEntry[i];
            }

            StartCoroutine("ShowInspectTextBox");
            return true;
        }

        /// <summary>
        /// Finds the dialogue objects
        /// </summary>
        /// <returns><c>true</c>, if dialogue objects was found, <c>false</c> otherwise.</returns>
        private bool FindDialogueObjects()
        {
            if (dialogueScreen == null) dialogueScreen = GameObject.Find("Dialogue Screen");
            if (dialogueScreen == null)
                Debug.LogWarning("Warning: Dialogue Screen Not Found");
            mainText = GameObject.Find("Main Text").GetComponent<Text>();
            titleText = GameObject.Find("Title Text").GetComponent<Text>();
            mainImage = GameObject.Find("Main Image").GetComponent<Image>();
            returnButton = GameObject.Find("Return Button");
            returnButtonText = GameObject.Find("Return Text").GetComponent<Text>();
            dialogueScreenGroup = dialogueScreen.GetComponent<CanvasGroup>();
            responseButtons = GameObject.Find("Response Buttons");
            if (!responseButtons)
                Debug.LogError("Reponse Buttons Not Found");
            responseTexts = new Text[4];

            return true;
        }

        /// <summary>
        /// Resets the dialogue screen.
        /// </summary>
        private void ResetDialogueScreen()
        {
            mainImage.sprite = null;
            returnButtonText.text = "";
            titleText.text = "";
            mainText.text = "";
            ToggleResponseButtons(false);
            ToggleReturnButton(false);
            Component[] texts = responseButtons.GetComponentsInChildren<Text>();
            foreach (Text t in texts)
            {
                t.text = "";
            }
        }
    }
}
