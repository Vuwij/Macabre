using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using Objects;
using System.Collections.Generic;
using System.Linq;

public class NavigationTest {
       
    [UnityTest]
    public IEnumerator PlayerMovement1() {
		SceneManager.LoadScene("Game");
		yield return null;
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "goto 'Blacksmith Floor 2 Room 1' 'Bed'");
		yield return new WaitForSeconds(20.0f);
        string parent = GameObject.Find("Player").transform.parent.name;
		Assert.AreEqual(parent, "Blacksmith Floor 2 Room 1");
    }

	[UnityTest]
    public IEnumerator PlayerMovement2() {
        SceneManager.LoadScene("Game");
        yield return null;
        GameObject.Find("Game Manager").SendMessage("AddGameTask", "goto 'Inn Floor 2 Balcony' 'Railings Left'");
        yield return new WaitForSeconds(15.0f);
        string parent = GameObject.Find("Player").transform.parent.name;
		Assert.AreEqual(parent, "Inn Floor 2 Balcony");
    }

	[UnityTest]
    public IEnumerator PlayerMovement3() {
        SceneManager.LoadScene("Game");
        yield return null;
        GameObject.Find("Game Manager").SendMessage("AddGameTask", "goto 'Bakery Floor 2 Room 1' 'Bed'");
        yield return new WaitForSeconds(15.0f);
        string parent = GameObject.Find("Player").transform.parent.name;
		Assert.AreEqual(parent, "Bakery Floor 2 Room 1");
    }

	[UnityTest]
	[Timeout(100000)]
	public IEnumerator PlayerMovementTwice() {
		SceneManager.LoadScene("Game");
        yield return null;
        GameObject.Find("Game Manager").SendMessage("AddGameTask", "goto 'Bakery Floor 2 Room 1' 'Bed'");
        yield return new WaitForSeconds(15.0f);
        string parent = GameObject.Find("Player").transform.parent.name;
        Assert.AreEqual(parent, "Bakery Floor 2 Room 1");

		GameObject.Find("Game Manager").SendMessage("AddGameTask", "goto 'Blacksmith Floor 2 Room 1' 'Bed'");
        yield return new WaitForSeconds(15.0f);
        parent = GameObject.Find("Player").transform.parent.name;
		Assert.AreEqual(parent, "Blacksmith Floor 2 Room 1");
	}
    
	[UnityTest]
    [Timeout(10000000)]
    public IEnumerator PlayerMovementAll()
    {
        SceneManager.LoadScene("Game");
        yield return null;

		GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
		List<PixelRoom> allRooms = new List<PixelRoom>();
		foreach(GameObject go in rootObjects) {
			PixelRoom room = go.GetComponent<PixelRoom>();
			if(room != null) {
				allRooms.Add(room);
			}
		}
        
        // Reorganize list
        System.Random rand = new System.Random();
		List<PixelRoom> randomizedRooms = allRooms.OrderBy(c => rand.Next()).Select(c => c).ToList();

		foreach(PixelRoom room in randomizedRooms) {
			string name = room.name;

			GameObject.Find("Game Manager").SendMessage("AddGameTask", "goto '" + name + "' 'Bed'");
            yield return new WaitForSeconds(15.0f);
            string parent = GameObject.Find("Player").transform.parent.name;
			Assert.AreEqual(parent, name);
		}
    }
}
