using System;
using UnityEngine.UI;
using System.Linq;

namespace UI.Dialogues
{
	public class GameDialogue : UIDialogue
	{
		Text textObject {
			get {
				return gameObject.GetComponentsInChildren<Text>().Single(x => x.name == "Text");
			}
		}
		public string text {
			get {
				return textObject.text;
			}
			set {
				textObject.text = value;
			}
		}
		public float brightness {
			get {
				return textObject.color.a;
			}
			set {
				var color = textObject.color;
				color.a = value;
				textObject.color = color;
			}
		}


		public void ShowForXSeconds(int x) {
			throw new NotImplementedException();
		}

		public void DisplayWithBrightness(string text, int alpha) {

		}
	}
}

