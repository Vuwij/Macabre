using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace SceneManagement
{

    /// <summary>
    /// Normal: The scene change is just a teleportation of the camera and player location
    /// ObjectOnly: This will move the player only, the camera will slowly follow
    /// </summary>
    public enum SceneChangeAnimation
    {
        Normal,
        ObjectOnly
    }

    /// <summary>
    /// The scene manager manages the different locations that the player is in, when those locations appear enter the real world,
    /// the player itself moves into that scene and the rule is that he is not able to escape that scene. The scenes that the player
    /// is able to be will restrain the player from moving into other scenes. The scene will also include multiple exit and entry points
    /// to other scenes in the world.
    /// 
    /// During the creating of the game, the player enters the overworld scene and all other scenes are disabled by default. The scenes
    /// themselves will be loaded using the loader script and they will attach into a dictionary within the Scene Manager. The scenes
    /// themselves are loaded in the highest level hierarchy within the folder.
    /// 
    /// PROS: This will make each scene loadable within the game, however the difficulty is savin gthe data within the scenes.
    /// ++ we do not need to update the scene. The scene information will be loaded and the SceneManager script within the game will update
    /// the scene information of the current scene. So there is no error. This only thing stored within each individual scene is the map elements
    /// 
    /// Further considerations is to include mutliple scenes as importable from within the scene folder. The scene folder will contain
    /// a series of scenes in folders. The most difficult part is saving those scenes. Considering that. Each scene  that is loaded
    /// would need its data serialized, and that data needs to be presented in a form of script within that scene.
    /// 
    /// Scene data will be stored in a database. Another thing to consider is that there could be different scene configurations,
    /// sometimes the scene may be a horror induced scene and sometimes it is just a normal scene. The solution to this is that
    /// the horror induced scene will be a complete different scene that the original scene, and it will need to be custom built
    /// 
    /// The only scene data that needs to be changed is the exit points, which are saved within the game.
    /// 
    /// Considering AI. The AI characters will be able to move to a different scene through looking at its current scenes datapoints. A MAP
    /// with a breadth first search might be needed for AI to navigate the scene. Thus a Network must be created in the future
    /// 
    /// </summary>
    public class MacabreSceneManager
    {
        // The Dictionary of Scenes, all the Scenes have a string
        public static Dictionary<string, MacabreScene> sceneList;

        // The current scene that is being used
        [HideInInspector]
        public static string currentScene;

        public static void Start()
        {
            LoadAllScenes();
            currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == "Start")
            {
                UIManager.main.TurnOnScreen("Start Screen");
            }
            else
            {
                UIManager.main.TurnOffScreen("Start Screen");
            }
        }

        public static void LoadAllScenes()
        {
        }

        /// <summary>
        /// Changes the scenes.
        /// </summary>
        /// <returns>The scenes.</returns>
        /// <param name="s">The scene that is changed to</param>
        public static void ChangeScenes(string s)
        {
            // Close the existing scene first
            sceneList[currentScene].Close();

            switch (sceneList[s].animation)
            {
                case SceneChangeAnimation.Normal:
                    //StartCoroutine ("ChangeSceneNormal");
                    break;
                case SceneChangeAnimation.ObjectOnly:
                    //StartCoroutine ("ChangeSceneObjectOnly");
                    break;
            }

            // Open the new scene
            currentScene = s;
            sceneList[s].Load();

            ChangeSceneSpecial(s);
        }

        private IEnumerable ChangeSceneNormal()
        {
            yield break;
        }

        private IEnumerable ChangeSceneObjectOnly()
        {
            yield break;
        }

        private static void ChangeSceneSpecial(string s)
        {
            switch (s)
            {
                case "Start":
                    UIManager.main.TurnOnScreen("Start Screen");
                    break;
            }
        }
    }
}