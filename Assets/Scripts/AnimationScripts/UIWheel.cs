using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIWheel : MonoBehaviour {
	float timeLeft;
	Color targetColor;

	void Update () {
		var image = this.GetComponent<Image> ();

		if (timeLeft <= Time.deltaTime) {
			// transition complete
			// assign the target color
			image.color = targetColor;

			// start a new transition
			targetColor = new Color (Random.value, Random.value, Random.value);
			timeLeft = 5.0f;
		} else {
			// transition in progress
			// calculate interpolated color
			image.color = Color.Lerp (image.color, targetColor, Time.deltaTime / timeLeft);

			// update the timer
			timeLeft -= Time.deltaTime;
		}
	}
}
