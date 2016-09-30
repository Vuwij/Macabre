using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MacabreTest {
	public void Start ()
	{
		var x = SceneManager.GetSceneByName ("Start");
		Debug.Log(x.ToString ());
	}
}
