using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using Objects;
using System.Collections.Generic;
using System.Linq;

public class ConversationTests {
	[UnityTest]
    [Timeout(50000)]
    public IEnumerator GivesTest1()
    {
        SceneManager.LoadScene("Game");
        yield return null;

		// Ping pong lol
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "player create 'Gold Key'");
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "player gives hamen 'Gold Key'");
		yield return new WaitForSeconds(5.0f);
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen gives player 'Gold Key'");
        yield return new WaitForSeconds(5.0f);
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "player gives hamen 'Gold Key'");
        yield return new WaitForSeconds(5.0f);
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen gives player 'Gold Key'");
        yield return new WaitForSeconds(5.0f);
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "player gives hamen 'Gold Key'");
        yield return new WaitForSeconds(5.0f);
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen gives player 'Gold Key'");
        yield return new WaitForSeconds(5.0f);
    }

	[UnityTest]
    [Timeout(50000)]
    public IEnumerator StealsTest1()
    {
        SceneManager.LoadScene("Game");
        yield return null;

        // Ping pong lol
        GameObject.Find("Game Manager").SendMessage("AddGameTask", "player create 'Gold Key'");
        GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen steals player 'Gold Key'");
        yield return new WaitForSeconds(5.0f);
        GameObject.Find("Game Manager").SendMessage("AddGameTask", "player steals hamen 'Gold Key'");
        yield return new WaitForSeconds(5.0f);
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen steals player 'Gold Key'");
        yield return new WaitForSeconds(5.0f);
        GameObject.Find("Game Manager").SendMessage("AddGameTask", "player steals hamen 'Gold Key'");
        yield return new WaitForSeconds(5.0f);
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen steals player 'Gold Key'");
        yield return new WaitForSeconds(5.0f);
        GameObject.Find("Game Manager").SendMessage("AddGameTask", "player steals hamen 'Gold Key'");
        yield return new WaitForSeconds(5.0f);
    }
}
