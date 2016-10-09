using UnityEngine;
using System;
using System.Collections;

namespace SceneManagement
{

    /// <summary>
    /// A scene's exit point, can be inherited from door, portal etc.
    /// </summary>
    public class ExitPoint
    {

        // The exit location to display, displaying another exit location will disable the exiting one
        public string[] exit;
        public string defaultExit;

        // By default the exit location is only 1, however if we want to make some random doors, there might be multiple
        public Vector3[] exitLocation;
        public Vector3 defaultExitLocation;

        // This blocks the player from exiting
        public bool exitEnabled = true;

        // If the correct item from the inventory is used then it will be able to unlock the object
        public int keyCode;
        public bool consumeKey;


        /// <summary>
        /// The constructor is called while loading the database manager.
        /// The default constructor is disabled. This one is for multiple exits
        /// The default exit index is index 0
        /// </summary>
        /// <param name="exit_">Exit.</param>
        /// <param name="exitLocation_">Exit location.</param>
        /// <param name="exitEnabled_">Exit enabled.</param>
        /// <param name="keyCode_">Key code.</param>
        /// <param name="consumeKey_">Consume key.</param>
        public ExitPoint(string[] exit_, Vector3[] exitLocation_, bool exitEnabled_, int keyCode_, bool consumeKey_)
        {
            exit = exit_;
            exitLocation = exitLocation_;
            exitEnabled = exitEnabled_;
            keyCode = keyCode_;
            consumeKey = consumeKey_;

            // Set the default one is always index 0
            defaultExit = exit_[0];
            defaultExitLocation = exitLocation_[0];

        }

        /// <summary>
        /// The constructor is called while loading the database manager.
        /// The default constructor is disabled. This one is for multiple exits
        /// The default exit index is index 0
        /// </summary>
        /// <param name="defaultExit_">Default exit.</param>
        /// <param name="defaultExitLocation_">Default exit location.</param>
        /// <param name="exitEnabled_">Exit enabled.</param>
        /// <param name="keyCode_">Key code.</param>
        /// <param name="consumeKey_">Consume key.</param>
        public ExitPoint(string defaultExit_, Vector3 defaultExitLocation_, bool exitEnabled_, int keyCode_, bool consumeKey_)
        {
            defaultExit = defaultExit_;
            defaultExitLocation = defaultExitLocation_;
            exitEnabled = exitEnabled_;
            keyCode = keyCode_;
            consumeKey = consumeKey_;
        }
        // The booleon returns whether or not the key is consumed during the unlocking
        public bool UnlockExitLocation(MItem.Key key_, out bool consumeKey_)
        {
            if (key_.keyCode == keyCode)
            {
                if (exitEnabled == false)
                {
                    exitEnabled = true;
                    Debug.Log("Door opened");
                    consumeKey_ = consumeKey;
                    return true;
                }
                if (exitEnabled == true) Debug.Log("Door already opened");
            }

            // Does not consume the key if the key doesn't work
            Debug.Log("Key doesn't work");
            consumeKey_ = false;
            return false;
        }

        // Using the exit location, which has its own animation and such
        public ExitAnimation exitAnimation;

        // NEED TO find the character that invokes this
        public void UseExitLocation(Character c)
        {
            // Change the scenes
            MacabreSceneManager.main.ChangeScenes(defaultExit);

            // Move the player
            c.Movement(defaultExitLocation, exitAnimation);
        }
    }
}