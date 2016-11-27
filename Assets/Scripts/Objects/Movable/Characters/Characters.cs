﻿using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Objects.Movable.Characters.Individuals;
using Data;

namespace Objects.Movable.Characters
{
    [DataContract]
    public sealed class Characters : EntityList
    {
        [IgnoreDataMember]
        public static Characters main
        {
            get { return (MacabreWorld.current != null) ? MacabreWorld.current.characters : null; }
        }

        // Character Models
        [DataMember(IsRequired = true, Order = 0)]
        private Dictionary<string, Character> characterDictionary = new Dictionary<string, Character>();
        [IgnoreDataMember]
        public static Dictionary<string, Character> CharacterDictionary
        {
            get { return main.characterDictionary; }
            set { main.characterDictionary = value; }
        }

        // Character Controllers
        [IgnoreDataMember]
        public List<CharacterController> characterControllers = new List<CharacterController>();
        [IgnoreDataMember]
        public static List<CharacterController> CharacterControllers
        {
            get { return main.characterControllers; }
        }

        [IgnoreDataMember]
        public static PlayerController playerController
        {
            get {
                if (SaveManager.CurrentSave == null) return null;
                return CharacterControllers.Find(x => (x is PlayerController)) as PlayerController;
            }
        }

        public const string startCharacterDirectory = @"Assets/Resources/Objects/Movable/Characters/LoadAtStart/";
        public const string allCharacterDirectory = @"Assets/Resources/Objects/Movable/Characters/";

        // FIXME Database name must match resource name
        public override void CreateNew()
        {
            DirectoryInfo itemsAtStart = new DirectoryInfo(startCharacterDirectory);

            // This loads everything in the resources folder
            var allItems = itemsAtStart.GetFiles()
                .Where(x => x.Extension != ".meta");

            foreach (var file in allItems)
            {
                string name = Path.GetFileNameWithoutExtension(file.FullName);
                AddPlayer(name);
            }
        }

        void AddPlayer(string name)
        {
            CharacterDictionary.Add(name, new Character { name = name });
        }
        
        public override void LoadAll()
        {
            Debug.Log("Loading All");
            if (characterControllers == null) characterControllers = new List<CharacterController>();

            // Load all the characters on the screen
            foreach (KeyValuePair<string, Character> c in CharacterDictionary)
            {
                // Load the resources first
                GameObject charObject;
                if (File.Exists(startCharacterDirectory + "/" + c.Key + ".prefab"))
                    charObject = Loader.LoadToWorld("Objects/Movable/Characters/LoadAtStart/" + c.Key);
                else
                    charObject = Loader.LoadToWorld("Objects/Movable/Characters/" + c.Key);

                // Relocate the character to the correct position
                charObject.transform.position = c.Value.position;

                // Add it to the list of character controllers
                if (charObject.GetComponent<CharacterController>() == null) throw new UnityException(charObject.name + " doesn't have controller attached");
                characterControllers.Add(charObject.GetComponent<CharacterController>());
            }
        }
    }
}
