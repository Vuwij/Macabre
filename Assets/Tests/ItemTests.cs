using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using Objects;
using System.Collections.Generic;
using System.Linq;

public class ItemTest {
       
	[UnityTest]
    public IEnumerator PutTest1()
    {
        SceneManager.LoadScene("Game");
        yield return null;
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "player create 4 'Gold'");
		yield return new WaitForSeconds(1.0f);
        GameObject.Find("Game Manager").SendMessage("AddGameTask", "player puts 4 'Gold' 'Inn Floor 1 Room 1' 'Bar Front'");
        yield return new WaitForSeconds(20.0f);
		GameObject barFront = GameObject.Find("Bar Front");
    }   
}
