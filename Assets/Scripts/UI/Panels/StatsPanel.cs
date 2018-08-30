using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    public sealed class StatsPanel : UIPanel
    {
		public string Date
        {
			set {
				GameObject date = null;
				for (int i = 0; i < gameObject.transform.childCount; ++i) {
					if (gameObject.transform.GetChild(i).name == "Date")
					{
						date = gameObject.transform.GetChild(i).gameObject;
						break;
					}
				}
				Text text = date.GetComponent<Text>();
				text.text = value;
			}
		}

		public string Time
		{
			set
            {
                GameObject time = null;
                for (int i = 0; i < gameObject.transform.childCount; ++i)
                {
                    if (gameObject.transform.GetChild(i).name == "Time")
                    {
                        time = gameObject.transform.GetChild(i).gameObject;
                        break;
                    }
                }
                Text text = time.GetComponent<Text>();
                text.text = value;
            }
		}

    }
}
