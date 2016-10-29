using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

namespace UI.Dialogues
{
    public sealed class InspectionDialogue : UIDialogue, UIGameObject
    {
        public override string name
        {
            get { return "Inspection Panel"; }
        }

        public Image inspectionImage
        {
            set {
                Image obj = gameObject.GetComponentInChildren<Image>();
                obj = value;
                obj.ToString();
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
