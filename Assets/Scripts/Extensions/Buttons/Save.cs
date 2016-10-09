using UnityEngine;
using System.Collections;

namespace Extensions.Buttons
{
    public class ButtonExtensionSave : ButtonExtension
    {
        public int saveID;
        public static int selectedSaveID = 0;
        public void selectCurrentSave()
        {
            selectedSaveID = saveID;
            Debug.Log("Current Save:" + selectedSaveID);
        }
    }
}