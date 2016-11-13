using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Objects.Inanimate.Buildings
{
    // A list of all of the items in the game
    [DataContract]
    public sealed class Buildings : EntityList
    {
        [IgnoreDataMember]
        public static Buildings main
        {
            get { return (MacabreWorld.current != null) ? MacabreWorld.current.buildings : null; }
        }

        // Models
        [DataMember(IsRequired = true, Order = 0)]
        private Dictionary<string, Building> buildingDictionary = new Dictionary<string, Building>();
        [IgnoreDataMember]
        public static Dictionary<string, Building> BuildingDictionary
        {
            get { return main.buildingDictionary; }
            set { main.buildingDictionary = value; }
        }

        // Controllers
        [IgnoreDataMember]
        public List<BuildingController> buildingController = new List<BuildingController>();
        [IgnoreDataMember]
        public static List<BuildingController> BuildingController
        {
            get { return main.buildingController; }
        }

        // FIXME Database name must match resource name
        public override void CreateNew()
        {
        }

        public override void LoadAll()
        {
        }
    }
}
