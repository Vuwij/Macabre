using System;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public static class Loader
    {
        public static GameObject Load(string folder)
        {
            GameObject obj = Resources.Load(folder, typeof(GameObject)) as GameObject;
            GameObject worldInstance = GameObject.Instantiate(obj);
            worldInstance.name = obj.name;
            return worldInstance;
        }
    }
}
