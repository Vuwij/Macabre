using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

namespace UI.Dialogues
{
    public class InspectionDialogue : UIDialogue
    {
        public override string name
        {
            get { return "Inspection Panel"; }
        }

        public Image inspectionImage
        {
            set {
                var obj = gameObject.GetComponentInChildren<Image>();
                obj = value;
            }
        }
        public string inspectionText
        {
            set
            {
                Text text = gameObject.GetComponentInChildren<Text>();
                text.text = value;
            }
        }
    }
}
