using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Data
{
    [XmlRoot("All Save Information")]
    [XmlInclude(typeof(Save))]
    public class AllSaveInformation
    {
        // The total number of saves so far
        public int SaveCount
        {
            get { return saveList.Count; }
        }

        // The current save ID
        public int HighestSaveID = 0;

        // A current list of all the saves
        public List<Save> saveList = new List<Save>();

        // The last save that is being used
        public string lastSaveUsed;
        
    }
}
