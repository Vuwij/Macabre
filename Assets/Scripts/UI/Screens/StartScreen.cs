using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

using Data;
using System;

namespace UI.Screens
{
    public sealed class StartScreen : UIScreen
    {
        public void NewGame()
        {
            gameObject.SetActive(false);
            // The scene contains the barebones of the scene
//            SceneManager.LoadScene("Game", LoadSceneMode.Additive);
//            Scene s = SceneManager.GetSceneByName("Game");
//            SceneManager.SetActiveScene(s);
//            SceneManager.GetActiveScene();

            // New save creates all the events and information on the game
//			Game.main.saves.New();
        }

        public void ContinueGame()
        {
            gameObject.SetActive(false);

            // The scene contains the barebones of the scene
            SceneManager.LoadScene("Game", LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Game"));

            // Loads the save from the information
//            Saves.Load(Saves.allSaveInformation.lastSaveUsed);
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
