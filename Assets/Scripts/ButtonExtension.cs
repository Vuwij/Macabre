using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonExtension : UnityEngine.UI.Button
{

	private Graphic [] m_graphics;
	protected Graphic [] Graphics {
		get {
			if (m_graphics == null) {
				m_graphics = targetGraphic.transform.GetComponentsInChildren<Graphic> ();
			}
			return m_graphics;
		}
	}

	protected override void DoStateTransition (SelectionState state, bool instant)
	{
		Color color;
		switch (state) {
		case Selectable.SelectionState.Normal:
			color = this.colors.normalColor;
			break;
		case Selectable.SelectionState.Highlighted:
			color = this.colors.highlightedColor;
			break;
		case Selectable.SelectionState.Pressed:
			color = this.colors.pressedColor;
			break;
		case Selectable.SelectionState.Disabled:
			color = this.colors.disabledColor;
			break;
		default:
			color = Color.black;
			break;
		}

		if (base.gameObject.activeInHierarchy) {
			switch (this.transition) {
			case Selectable.Transition.ColorTint:
				ColorTween (color * this.colors.colorMultiplier, instant);
				break;
			default:
				throw new System.NotSupportedException ();
			}
		}
	}

	private void ColorTween (Color targetColor, bool instant)
	{
		if (this.targetGraphic == null) {
			return;
		}

		foreach (Graphic g in this.Graphics) {
			g.CrossFadeColor (targetColor, (!instant) ? this.colors.fadeDuration : 0f, true, true);
		}
	}

	#region selectedExtension
	public bool selectable = false;
	private bool isSelected = false;
	public void toggle ()
	{
		if (isSelected == false) {
			isSelected = true;
		} else {
			isSelected = false;
		}
	}

	public void turnOff ()
	{
		isSelected = false;
	}

	public void toggleThisInGroup ()
	{
		var gameObject = GameObject.Find ("Save Background");

		// Turns off all other gameobjects in the environment
		if (gameObject) {
			var buttons = gameObject.GetComponentsInChildren<ButtonExtension> ();
			foreach (ButtonExtension b in buttons) {
				if (b.selectable) {
					b.turnOff ();
				}
			}
		}

		// Turn on this one
		this.isSelected = true;
	}

	#endregion
}