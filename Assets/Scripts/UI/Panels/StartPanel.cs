using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

using Data;

namespace UI.Panels
{
    /// <summary>
    /// Start Game Screen is the UI Screen at the beginning of the game
    /// </summary>
    class StartPanel : UIPanel
    {
        private Slider loadingSlider;

        public void NewGame()
        {
            UIManager.TurnOffScreen("Start Screen");
            SceneManager.LoadScene("Game", LoadSceneMode.Additive);
            //		SceneManager.SetActiveScene (SceneManager.GetSceneByName ("Game"));
            //		UIManager.main.TurnOnScreen ("Loading Screen");
            //
            //		// Set up new game data
            //		MacabreSceneManager.NewGame ();
            SaveManager.NewGame();
            //
            //		UIManager.main.FindGameObjects ();
            //		UIManager.main.TurnOffScreen ("Loading Screen");
            //		Debug.Log ("Game Loaded");
        }

        public void ResumeGame()
        {
            SceneManager.LoadScene("Game", LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Game"));
            UIManager.main.TurnOnScreen("Loading Screen");

            // Set up new game data
            MacabreSceneManager.NewGame();
            SaveManager.NewGame();

            UIManager.main.FindGameObjects();
            UIManager.main.TurnOffScreen("Start Screen");
            UIManager.main.TurnOffScreen("Loading Screen");
            Debug.Log("Game Loaded");
        }

        public void QuitGame()
        {
        }

        public void OpenSettings()
        {
        }
    }
}
