using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using Exceptions;


namespace Objects.Inanimate.Buildings
{
    // A list of all of the buildings in the game
    [DataContract]
    public sealed class Buildings : EntityList
    {
        [IgnoreDataMember]
        public static Buildings main
        {
            get { return (MacabreWorld.current != null) ? MacabreWorld.current.buildings : null; }
        }

        // Building Models
        [DataMember(IsRequired = true, Order = 0)]
        private Dictionary<string, Building> buildingDictionary = new Dictionary<string, Building>();
        [IgnoreDataMember]
        public static Dictionary<string, Building> BuildingDictionary
        {
            get { return main.buildingDictionary; }
            set { main.buildingDictionary = value; }
        }

        // Building Controllers
        [IgnoreDataMember]
        public List<BuildingController> buildingControllers = new List<BuildingController>();
        [IgnoreDataMember]
        public static List<BuildingController> BuildingControllers
        {
            get { return main.buildingControllers; }
        }

        public BuildingController this[string s]
        {
            get
            {
                return GetBuilding(s);
            }
        }

        public const string startBuildingDirectory = "Assets/Resources/Objects/Inanimate/Buildings/LoadAtStart";
        public const string allBuildingDirectory = "Assets/Resources/Objects/Inanimate/Buildings";

        // Finds the gameobject, then loads into the world or parent if it is not in the world
        public BuildingController GetBuilding(string name, GameObject parent = null)
        {
            BuildingController building = BuildingControllers.Where(x => x.name == name).SingleOrDefault();
            if (building != null) return building;

            if (!File.Exists(allBuildingDirectory + "/" + name + ".prefab")) throw new UnityException("Resource " + name + " does not exist in Buildings Directory");

            GameObject g = Loader.LoadToWorld("Objects/Inanimate/Buildings/" + name);

            if (parent != null)
            {
                g.transform.parent = parent.transform;
                g.transform.position = parent.transform.position;
            }

            var buildingController = g.GetComponent<BuildingController>();
            if (buildingController == null) throw new MacabreException("Building " + name + " does not contain BuildingController");
            buildingControllers.Add(buildingController);
            buildingDictionary.Add(buildingController.name, new Building(buildingController.name));
            return buildingController;
        }

        public override void CreateNew()
        {
            DirectoryInfo buildingsAtStart = new DirectoryInfo(startBuildingDirectory);

            // This loads everything in the resources folder
            var allBuildings = buildingsAtStart.GetFiles()
                .Where(x => x.Extension != ".meta");

            foreach (var file in allBuildings)
            {
                string name = Path.GetFileNameWithoutExtension(file.FullName);
                BuildingDictionary.Add(name, new Building(name));
            }
        }

        public override void LoadAll()
        {
            Debug.Log("Loading All");
            if (buildingControllers == null) buildingControllers = new List<BuildingController>();

            // Load all the Buildings on the screen
            foreach (KeyValuePair<string, Building> c in BuildingDictionary)
            {
                // Load the resources first
                // FIXME, loader.load should store the resource in this class somewhere
                GameObject buildingObject;
                if (File.Exists(startBuildingDirectory + "/" + c.Key + ".prefab"))
                    buildingObject = Loader.LoadToWorld("Objects/Inanimate/Buildings/LoadAtStart/" + c.Key);
                else
                    buildingObject = Loader.LoadToWorld("Objects/Inanimate/Buildings/" + c.Key);

                // Relocate the Building to the correct position
                buildingObject.transform.position = c.Value.position;

                // Add it to the list of Building controllers
                if (buildingObject.GetComponent<BuildingController>() == null) throw new UnityException(buildingObject.name + " doesn't have controller attached");
                buildingControllers.Add(buildingObject.GetComponent<BuildingController>());
            }
        }
    }
}
