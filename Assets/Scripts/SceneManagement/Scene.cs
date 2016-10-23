//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//namespace SceneManagement
//{

//    /// <summary>
//    /// This Scene information will be explained in <see cref="SceneManager"/>
//    /// Please see Scene manager for this informaiton
//    /// </summary>
//    public class MacabreScene
//    {
//        // The list of objects that the Scene has,
//        public List<GameObject> sceneObjects;

//        // The boundaries of the scene is stored in the sceneCollider
//        public Collider[] sceneCollider;

//        // A list of exit points for that scene. Each ExitPoint points to a different scene within the door
//        public List<ExitPoint> exitPoints;

//        // The type of scene change on entry
//        public SceneChangeAnimation animation;

//        public void Load()
//        {

//        }

//        public void Close()
//        {

//        }

//        /// <summary>
//        /// This needs to iterate through all of the gameobjects within that scene and then store the data into the scene database
//        /// </summary>
//        public void SaveScene()
//        {

//        }

//        // TODO
//        /// <summary>
//        /// This needs to iterate through all of the gameobjects within that scene and then store the data into the scene database
//        /// <list type="Info">
//        /// <item>How are we going to be able to load the correct scene? And the game data from that. We have all the player data but not the game data</item>
//        /// <item>Jason: From considerations everytime the game loads that scene it will have to look at the MacabreThing data for that object</item>
//        /// <item>Jason: We can also have all MacabreObjects not inherit from Monobehavior. This will simplify the need to XML serialize everything</item>
//        /// <item>Jason: Thats a good idea. but what about objects that have a collision box like the floor?</item>
//        /// <item>Jason: There will be certain objects that can be saved and those who wont. Those who will will have an interface</item>
//        /// <item>Jason: And all those objects that have that interface will be saved and those that won't won't? This would be so much easier if we
//        /// just used JSON</item>
//        /// <item>Jason: We need to continue this discussion later. Why not just work on this for now</item>
//        /// </list>
//        /// </summary>
//        public void LoadScene()
//        {

//        }
//    }
//}