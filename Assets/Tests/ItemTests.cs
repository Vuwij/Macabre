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
        yield return new WaitForSeconds(10.0f);
		GameObject barFront = GameObject.Find("Bar Front");
		PixelStorage storage = barFront.GetComponent<PixelStorage>();
		Assert.True(storage.HasObject("Gold", 4));
    }

	[UnityTest]
    public IEnumerator PutTest2()
    {
        SceneManager.LoadScene("Game");
        yield return null;
        GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen create 4 'Silver'");
        yield return new WaitForSeconds(1.0f);
        GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen puts 4 'Silver' 'Inn Floor 1 Room 1' 'Bar Front'");
        yield return new WaitForSeconds(10.0f);
		GameObject barFront = GameObject.Find("Bar Front");
        PixelStorage storage = barFront.GetComponent<PixelStorage>();
		Assert.True(storage.HasObject("Silver", 4));
    }

	[UnityTest]
    public IEnumerator PutTest3()
    {
        SceneManager.LoadScene("Game");
        yield return null;
        
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen create 2 'Gold'");
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "player create 2 'Silver'");
		yield return new WaitForSeconds(1.0f);
        GameObject.Find("Game Manager").SendMessage("AddGameTask", "player puts 2 'Silver' 'Inn Floor 1 Room 1' 'Bar Front'");
		yield return new WaitForSeconds(1.0f);
        GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen puts 2 'Gold' 'Inn Floor 1 Room 1' 'Bar Front'");
        yield return new WaitForSeconds(10.0f);
		GameObject barFront = GameObject.Find("Bar Front");
        PixelStorage storage = barFront.GetComponent<PixelStorage>();
        Assert.True(storage.HasObject("Silver", 2));
		Assert.True(storage.HasObject("Gold", 2));   
    }

	[UnityTest]
	public IEnumerator TakesTest1()
	{
		yield return PutTest1();

		GameObject.Find("Game Manager").SendMessage("AddGameTask", "player takes 4 'Gold' 'Inn Floor 1 Room 1' 'Bar Front'");
        yield return new WaitForSeconds(10.0f);

		GameObject barFront = GameObject.Find("Bar Front");
        PixelStorage storage = barFront.GetComponent<PixelStorage>();
		Assert.False(storage.HasObject("Gold", 4));
	}

	[UnityTest]
	[Timeout(50000)]
    public IEnumerator TakesTest2()
    {
		SceneManager.LoadScene("Game");
        yield return null;
        GameObject.Find("Game Manager").SendMessage("AddGameTask", "player create 2 'Gold'");
        GameObject.Find("Game Manager").SendMessage("AddGameTask", "player puts 2 'Gold' 'Inn Floor 1 Room 1' 'Bar Front'");
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen create 2 'Silver'");
        GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen puts 2 'Silver' 'Inn Floor 1 Room 1' 'Bar Front'");
		yield return new WaitForSeconds(10.0f);
        GameObject.Find("Game Manager").SendMessage("AddGameTask", "player takes 1 'Gold' 'Inn Floor 1 Room 1' 'Bar Front'");
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "player takes 1 'Silver' 'Inn Floor 1 Room 1' 'Bar Front'");
        GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen takes 1 'Silver' 'Inn Floor 1 Room 1' 'Bar Front'");      
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen takes 1 'Gold' 'Inn Floor 1 Room 1' 'Bar Front'");      
        yield return new WaitForSeconds(10.0f);
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "player puts 1 'Gold' 'Inn Floor 1 Room 1' 'Bar Front'");
        GameObject.Find("Game Manager").SendMessage("AddGameTask", "player puts 1 'Silver' 'Inn Floor 1 Room 1' 'Bar Front'");
        GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen puts 1 'Silver' 'Inn Floor 1 Room 1' 'Bar Front'");
        GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen puts 1 'Gold' 'Inn Floor 1 Room 1' 'Bar Front'");
        yield return new WaitForSeconds(10.0f);
		GameObject.Find("Game Manager").SendMessage("AddGameTask", "player takes 2 'Gold' 'Inn Floor 1 Room 1' 'Bar Front'");
        GameObject.Find("Game Manager").SendMessage("AddGameTask", "hamen takes 2 'Silver' 'Inn Floor 1 Room 1' 'Bar Front'");
        yield return new WaitForSeconds(10.0f);
        GameObject barFront = GameObject.Find("Bar Front");
        PixelStorage storage = barFront.GetComponent<PixelStorage>();
		Assert.False(storage.HasObject("Gold", 1));
		Assert.False(storage.HasObject("Silver", 1));
    }
}
