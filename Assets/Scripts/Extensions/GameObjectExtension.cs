using System;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
    public static class GameObjectExtension
    {
        public static GameObject GetGameObjectWithinChildren(this GameObject gameObject, string name)
        {
            Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
            foreach (Transform item in children)
                if (item.name == name)
                    return item.gameObject;

            return null;
        }

        public static GameObject[] GetGameObjectsWithinChildren(this GameObject gameObject, string name)
        {
            List<GameObject> gameObjects = new List<GameObject>();

            Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
            foreach (Transform item in children)
                if (item.name == name)
                    gameObjects.Add(item.gameObject);

            return gameObjects.ToArray();
        }
    }
}
