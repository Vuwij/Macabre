using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace Objects
{
    /// <summary>
    /// The loader loads a default gameobject, it only uses static variables and classes
    /// </summary>
    public static partial class Loader
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
        
    }
}