using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace Data
{
    /// <summary>
    /// The loader loads a default gameobject, it only uses static variables and classes
    /// </summary>
    public static class Loader
    {
        public static GameObject LoadFolder(string folder)
        {
            if (GameObject.Find(folder) != null)
            {
                return GameObject.Find(folder);
            }
            var newFolder = new GameObject(folder);
            Debug.Log("Folder: " + folder + " loaded");
            newFolder.transform.name = folder;
            return newFolder;
        }
        /*
        /// <summary>
        /// The Data
        /// </summary>
        /// <returns>The Gameobject</returns>
        /// <param name="gameObjectName">The Name of the GameObject</param>
        /// <param name="folder">The Folder of the GameObject</param>
        /// <typeparam name="T">The type of game data under namespace MData</typeparam>
        public static GameObject LoadWithData<T>(string gameObjectName, string folder)
            where T : MData.Component, new()
        {
            GameObject g = Load(gameObjectName, folder);

            // Make a new Data_component, automatically stored in gameData list
            Activator.CreateInstance(typeof(T), new object[] { g, folder + "/" + gameObjectName });

            return g;
        }
        */
        public static GameObject Load(string gameObjectName, string folder)
        {
            GameObject theFolder;
            theFolder = LoadFolder(folder);

            // Checks if the gameobject has already been loaded in the game
            if (GameObject.Find(gameObjectName) != null)
            {
                Debug.LogWarning(gameObjectName + " already loaded into the " + folder);
            }

            // Checks if the gameobject exists in the resources folder

            GameObject g;
            try
            {
                g = UnityEngine.Object.Instantiate(Resources.Load<GameObject>(folder + "/" + gameObjectName) as GameObject);
            }
            catch (UnityException e)
            {
                Debug.Log(e);
                return null;
            }
            g.transform.SetParent(theFolder.transform);
            g.transform.name = gameObjectName;
            return g;
        }
        /*
        /// <summary>
        /// Finds all the Gamecomponents attached
        /// </summary>
        public static void ResetAllGameComponents()
        {
            foreach (MData.Component m in GameData.main.gameData)
            {
                string[] folderName = m.pathName.Split('/');
                if (folderName.Length != 2) Debug.LogException(new Exception("Folder name needs to be Folder/Object format"));
                string folder = folderName[0];
                string name = folderName[1];

                var g = GameObject.Find(name);
                m.AttachGameObject(g);
            }
        }
        
        /// <summary>
        /// Loads all of the game components
        /// </summary>
        public static void LoadAllGameComponents()
        {
            LoadWithData<MData.Player>("Player", "Characters");
            LoadWithData<MData.Character>("Merchant", "Characters");
            LoadWithData<MData.Character>("Innkeeper", "Characters");
            LoadWithData<MData.Character>("HoodedFarmer", "Characters");
            LoadWithData<MData.Character>("Guard", "Characters");
            LoadWithData<MData.Character>("Elismi", "Characters");
        }
        */
    }
}