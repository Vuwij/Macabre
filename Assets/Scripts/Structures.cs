using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// A single conversation, a simple statement said by a single person
/// </summary>
public struct singleConversation {
	
	/// <param name="char1_"> The name of the person who is speaking </param>
	/// <param name="char2_"> The name of the person who is spoken to </param>
	/// <param name="conversation_"> The actual conversation</param>
	public singleConversation(string speaker_, string spoken_, string conversation_, bool options_, string[] response_) {
		speaker = speaker_;
		spoken = spoken_;
		conversation = conversation_;
		options = options_;
		response = response_;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="singleConversation"/> struct. Do not use the default contructor for the struct
	/// </summary>
	/// <param name="reset">If set to <c>true</c> reset.</param>
	public singleConversation(bool reset) {
		speaker = "";
		spoken = "";
		conversation = "";
		options = false;
		response = new string[4];
	}

	public string speaker, spoken;
	public string conversation;
	public bool options;
	public string[] response;

	public void print() {
		if (!Settings.debugCONVO)
			return;

		if (speaker == "")
			Debug.Log ("CONVERSATION EMPTY");
		else Debug.Log ("CONVERSATION| " + speaker + " - " + spoken + ": " + conversation);
	}
}

public struct OverWorldMap {
	public Vector2 topleft;
	public Vector2 bottomright;
	public Vector2 topright;
	public Vector2 bottomleft;

	public Vector2 top;
	public Vector2 bottom;
	public Vector2 left;
	public Vector2 right;

	public Vector2 center;
	public float width;
	public float height;

	/// <summary>
	/// Initializes a new instance of the <see cref="overWorldMap"/> struct.
	/// </summary>
	/// <param name="topleft_">Topleft Corner</param>
	/// <param name="bottomright_">Bottomright Corner</param>
	public OverWorldMap(Vector2 topleft_, Vector2 bottomright_) {
		this.topleft = topleft_;
		this.bottomright = bottomright_;

		this.center = topleft + Vector2.Scale((bottomright - topleft), (new Vector2(0.5f, 0.5f)));
		this.width = Mathf.Abs (bottomright.x - topleft.x);
		this.height = Mathf.Abs (bottomright.y - topleft.x);

		this.top = center - new Vector2 (0, height / 2.0f);
		this.bottom = center + new Vector2 (0, height / 2.0f);

		this.left = center - new Vector2 (width / 2.0f, 0);
		this.right = center + new Vector2 (width / 2.0f, 0);

		this.topright = center + new Vector2 (width / 2.0f, -height / 2.0f);
		this.bottomleft = center + new Vector2 (-width / 2.0f, height / 2.0f);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="overWorldMap"/> struct.
	/// </summary>
	/// <param name="center_">X/Y coordinates of the center</param>
	/// <param name="width_">Width.</param>
	/// <param name="height_">Height.</param>
	public OverWorldMap(Vector2 center_, float width_, float height_) {
		this.center = center_;
		this.width = width_;
		this.height = height_;

		this.top = center - new Vector2 (0, height / 2.0f);
		this.bottom = center + new Vector2 (0, height / 2.0f);

		this.left = center - new Vector2 (width / 2.0f, 0);
		this.right = center + new Vector2 (width / 2.0f, 0);

		this.topleft = center + new Vector2 (-width / 2.0f, -height / 2.0f);
		this.bottomright = center + new Vector2 (width / 2.0f, height / 2.0f);

		this.topright = center + new Vector2 (width / 2.0f, -height / 2.0f);
		this.bottomleft = center + new Vector2 (-width / 2.0f, height / 2.0f);

	}

	public void print() {
		Debug.Log ("Center: " + center + ", Width: " + width + ", Height:" + height);
	}
}

public class CharacterStatList
{
	Dictionary<string, CharacterStat> characterStat;

	public CharacterStatList (List<string> characterStatList)
	{
		characterStat = new Dictionary<string, CharacterStat> ();
		foreach (string s in characterStatList) {
			characterStat.Add (s, new CharacterStat ());
		}
	}

	public CharacterStat getStat (string s)
	{
		CharacterStat c;
		characterStat.TryGetValue (s, out c);
		if (c != null) return c;
		return null;
	}

	public void setStat (string s, int i)
	{
		CharacterStat c;
		characterStat.TryGetValue (s, out c);
		if (c != null) c.value = i;
	}

}

public class CharacterStat {

	[Range(0, 100)]
	public int value = 0;

	public CharacterStat() {
		value = 100;
	}
	public CharacterStat(int value_) {
		value = value_;
	}

	public static CharacterStat operator + (CharacterStat a, CharacterStat b) {
		return new CharacterStat(a.value + b.value);
	}
	public static CharacterStat operator - (CharacterStat a, CharacterStat b) {
		return new CharacterStat (a.value - b.value);
	}
	public static CharacterStat operator ++ (CharacterStat a) {
		a.value++;
		return a;
	}
	public static CharacterStat operator -- (CharacterStat a) {
		a.value--;
		return a;
	}
}