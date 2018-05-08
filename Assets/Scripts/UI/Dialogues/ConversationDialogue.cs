using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Extensions;
using Objects.Movable.Characters;
using Objects.Movable.Characters.Individuals;

namespace UI.Dialogues
{
    public sealed class ConversationDialogue : UIDialogue
    {
        public string mainText
        {
            set
            {
                Text t = this.GetComponentsInChildren<Transform>()
                    .Where(x => x.name == "Main Text").First()
                    .GetComponent<Text>();
                t.text = value;
            }
        }
        public string titleText
        {
            set
            {
                Text t = this.GetComponentsInChildren<Transform>()
                    .Where(x => x.name == "Title Text").First()
                    .GetComponent<Text>();
                t.text = value;
            }
        }
        public string continueText
        {
            set
            {
                Text t = this.GetComponentsInChildren<Transform>()
                    .Where(x => x.name == "Continue Button").First()
                    .GetComponent<Text>();
                t.text = value;
            }
        }
        public string[] responseTexts
        {
            set
            {
                if (value.Length > 4) throw new Exception("Array cannot be greater than 4");

                var t = from obj in this.GetComponentsInChildren<Transform>()
                        where obj.name.Contains("Response Text")
                        select obj.GetComponent<Text>();

                Text[] text = t.ToArray();
                for (int i = 0; i < value.Length; i++)
                    text[i].text = value[i];
            }
        }

        public Sprite mainImage
        {
            set {
                Image i = GetComponentsInChildren<Image>()
                    .Where(x => x.gameObject.name == "Image")
                    .FirstOrDefault();
                if (i != null)
                {
                    i.color = Color.white;
                    i.sprite = value;
                }
            }
        }
        public Button[] buttons
        {
            get {
                return GetComponentsInChildren<Button>();
            }
        }

        public void ResponsePressed(int i)
        {
			//GameObject.Find("Player").GetComponent<Character>().KeyPressed(i);
        }

        public void ContinuePressed()
        {
			GameObject.Find("Player").GetComponent<Character>().InspectionAction(GameObject.Find("Player").GetComponent<Player>(), new RaycastHit2D());
        }

        public void Reset()
        {
            mainText = "";
            titleText = "";
            responseTexts = new string[4] { "", "", "", "" };
            mainImage = null;
        }
    }
}
