using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

using Data;
using System;

namespace UI.Screens
{
    public class StartScreen : UIScreen
    {
        private Slider loadingSlider;

        public override string name
        {
            get { return "Start Screen"; }
        }

        public void NewGame()
        {
            TurnOff();
            // The scene contains the barebones of the scene
            SceneManager.LoadScene("Game", LoadSceneMode.Additive);
            
            // New save creates all the events and information on the game
            SaveManager.NewSave();
        }

        public void ResumeGame()
        {
            TurnOff();

            // The scene contains the barebones of the scene
            SceneManager.LoadScene("Game", LoadSceneMode.Additive);

            // Loads the save from the information
            SaveManager.LoadSave(SaveManager.allSaveInformation.lastSaveUsed);
        }

        // TODO Quit Game and Open Settings
        public void QuitGame()
        {
            throw new NotImplementedException();
        }

        public void OpenSettings()
        {
            throw new NotImplementedException();
        }
    }
}
