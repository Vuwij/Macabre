using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Database
{
    public class ClassData
    {
        public int ItemID;
        public string ItemName;
        public string ItemDescription;
        public string[] ItemProperties;
    }

    public class ClassAData : ClassData { }
    public class ClassBData : ClassData { }

    public class ItemCombineData
    {
        public int ItemID;
        public string AClassCombinedID;
        public int AClassID1;
        public int AClassID2;
        public string CombinationName;
        public string CombinationText;
    }
}
