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

	[UnityTest]
    [Timeout(50000)]
    public IEnumerator ConversationTest1()
    {
        SceneManager.LoadScene("Game");
        yield return null;

        // Difficult one
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "player puts 2 'Gold' 'Inn Floor 1 Room 1' 'Bar Front'");
        yield return new WaitForSeconds(5.0f);
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen takes 2 'Gold' 'Inn Floor 1 Room 1' 'Bar Front'");
        yield return new WaitForSeconds(5.0f);
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "player goto 'Table'");
        yield return new WaitForSeconds(5.0f);
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen gives Player 'Gold Key'");
        yield return new WaitForSeconds(5.0f);
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen create 'Bowl of Stew'");
        yield return new WaitForSeconds(5.0f);
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen goto 'Bar Side'");
        yield return new WaitForSeconds(5.0f);
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen create 'Mug of Ale'");
        yield return new WaitForSeconds(5.0f);
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen goto Player");
        yield return new WaitForSeconds(5.0f);
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen puts 'Mug of Ale' 'Table'");
        yield return new WaitForSeconds(5.0f);
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen goto 'Bar Side'");
        yield return new WaitForSeconds(5.0f);
    }
}
