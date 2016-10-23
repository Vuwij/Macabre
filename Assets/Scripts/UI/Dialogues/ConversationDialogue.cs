using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;
using Extensions;

namespace UI.Dialogues
{
    public class ConversationDialogue : UIDialogue
    {
        public override string name
        {
            get
            {
                return "UI Screen";
            }
        }

        public string mainText
        {
            set
            {
                Text t = GameObject.Find("Main Text").GetComponent<Text>();
                t.text = value;
            }
        }
        public string titleText
        {
            set
            {
                Text t = GameObject.Find("Title Text").GetComponent<Text>();
                t.text = value;
            }
        }
        public string continueText
        {
            set
            {
                Text t = gameObject.GetGameObjectWithinChildren("Continue").GetComponent<Text>();
                t.text = value;
            }
        }
        
        // HACK need to validate this, doesn't work all the time
        public string[] responseButtonText
        {
            set
            {
                GameObject[] objs = gameObject.GetGameObjectsWithinChildren("Response");
                var t = from obj in objs
                            select obj.GetComponent<Text>();
                Text[] texts = t.ToArray();
                try
                {
                    for (int i = 0; i < 4; i++)
                        texts[i].text = value[i];
                }
                catch (Exception) { };
            }
        }

        public Image mainImage
        {
            get { return GameObject.Find("Main Image").GetComponent<Image>(); }
        }
        public Button[] buttons
        {
            get { return gameObject.GetComponentsInChildren<Button>(); }
        }
    }
}
