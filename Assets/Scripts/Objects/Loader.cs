using System;
using System.Collections.Generic;
using UnityEngine;
using Exceptions;

namespace Objects
{
    public static class Loader
    {
        public static GameObject Load(string folder)
        {
            GameObject obj = Resources.Load(folder, typeof(GameObject)) as GameObject;
            if (obj == null) throw new MacabreException("Object Not Found: " + folder);
            GameObject worldInstance = GameObject.Instantiate(obj);
            worldInstance.name = obj.name;
            return worldInstance;
        }
    }
}
