using System;
using System.Collections.Generic;
using UnityEngine;
using Exceptions;
using System.Linq;

namespace Objects
{
    public static class Loader
    {
        public static GameObject LoadToWorld(string folder)
        {
            GameObject obj = Resources.Load(folder, typeof(GameObject)) as GameObject;
            if (obj == null) throw new MacabreException("Object Not Found: " + folder);
            GameObject worldInstance = GameObject.Instantiate(obj);
            worldInstance.name = obj.name;
            return worldInstance;
        }

        public static GameObject LoadToMemory(string folder)
        {
            GameObject obj = Resources.Load(folder, typeof(GameObject)) as GameObject;
            return obj;
        }

        public static List<GameObject> LoadAllToMemory(string folder)
        {
            UnityEngine.Object[] objArray = Resources.LoadAll(folder);
            var newArray = Array.ConvertAll(objArray, item => (GameObject)item);
            return newArray.ToList();
        }
    }
}
