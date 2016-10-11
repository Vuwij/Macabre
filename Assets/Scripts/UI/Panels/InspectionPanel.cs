using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    class InspectionPanel
    {
        /// <summary>
        /// Hides the inspect text box.
        /// </summary>
        /// <returns>The inspect text box.</returns>
        private IEnumerator HideInspectTextBox()
        {
            while (dialogueScreenGroup.alpha > 0.0f)
            {
                dialogueScreenGroup.alpha -= 0.01f;
                yield return null;
            }
            yield break;
        }

        /// <summary>
        /// Shows the inspect text box.
        /// </summary>
        /// <returns>The inspect text box.</returns>
        private IEnumerator ShowInspectTextBox()
        {
            while (dialogueScreenGroup.alpha < 0.8f)
            {
                dialogueScreenGroup.alpha += 0.01f;
                yield return null;
            }
            yield break;
        }

        private void ToggleResponseButtons(bool op)
        {
            if (!responseButtons)
                Debug.LogError("Reponse Buttons not found");
            if (op) responseButtons.GetComponent<CanvasGroup>().alpha = 1;
            else responseButtons.GetComponent<CanvasGroup>().alpha = 0;
        }

        private void ToggleReturnButton(bool op)
        {
            if (!returnButton)
                Debug.LogError("Return button not found");
            if (op)
            {
                returnButton.GetComponent<Button>().interactable = true;
                var color = returnButton.GetComponent<Image>().color;
                color.a = 255;
            }
            else
            {
                returnButton.GetComponent<Button>().interactable = false;
                var color = returnButton.GetComponent<Image>().color;
                color.a = 0;
            }
        }
    }
}
