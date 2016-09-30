using UnityEngine;
using System.Collections;

public class DrawBorder : MonoBehaviour {

	public Vector3[] corners = new Vector3[4];
	public bool square = false;

	void Start () {
		LineRenderer lineRenderer = GetComponent<LineRenderer> ();
		int i = 0;
		while (i < 4) {
			corners[i].x -= 1;
			if(square) corners[i].y -= 0.5f;
			lineRenderer.SetPosition(i, corners[i]);
			i++;
		}
		lineRenderer.SetPosition (4, corners [0]);

		Debug.DrawLine (corners [0], corners [1], Color.green, 20, false);
		Debug.DrawLine (corners [1], corners [2], Color.green, 20, false);
		Debug.DrawLine (corners [2], corners [3], Color.green, 20, false);
		Debug.DrawLine (corners [3], corners [0], Color.green, 20, false);
	}
}
