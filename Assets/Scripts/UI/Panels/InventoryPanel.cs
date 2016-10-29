using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Panels
{
    public sealed class InventoryPanel : UIPanel, UIGameObject
    {
        public override string name
        {
            get { return "Inventory Panel"; }
        }
    }
}
