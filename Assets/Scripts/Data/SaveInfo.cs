using System.Collections.Generic;
using System.Xml.Serialization;

namespace Data
{
    [XmlRoot("Save Info")]
    [XmlInclude(typeof(Save))]
    public struct SaveInfo
    {
        public int saveNumber { get; set; }
        public List<Save> saveList { get; set; }
        public Save currentSave { get; set; }       // The current save, last loaded
    }
}
