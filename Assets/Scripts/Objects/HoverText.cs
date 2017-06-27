using System;
using UnityEngine;

namespace Objects
{

	[RequireComponent (typeof(TextMesh))]
	public class HoverText : MonoBehaviour
	{
		TextMesh textMesh {
			get {
				return GetComponent<TextMesh> ();
			}
		}
		MeshRenderer meshRenderer {
			get {
				return GetComponent<MeshRenderer>();
			}
		}
		Objects.Object o {
			get {
				return GetComponentInParent<Objects.Object>();
			}
		}
		void Start ()
		{
			textMesh.text = o.name;
			textMesh.anchor = TextAnchor.MiddleCenter;
			textMesh.fontSize = 102;
			meshRenderer.sortingLayerName = "GameUI";
		}

		void Update() {
			textMesh.font = Game.main.UI.fonts["Munro_small"];
		}
	}
}