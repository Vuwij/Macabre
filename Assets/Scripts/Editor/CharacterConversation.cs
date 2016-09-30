using System;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class CharacterConversation {

	[Test]
	public void Test ()
	{
		Console.WriteLine ("Test 1");
	}

	[Test]
    public void EditorTest()
    {
		Console.WriteLine ("Hello World");
		//Arrange
        var gameObject = new GameObject();

        //Act
        //Try to rename the GameObject
        var newGameObjectName = "My game object";
        gameObject.name = newGameObjectName;
        //Assert
        //The object has a new name
        Assert.AreEqual(newGameObjectName, gameObject.name);
    }
	[Test]
	public void EditorTest2 ()
	{
		//Arrange
		var gameObject = new GameObject ();

		//Act
		//Try to rename the GameObject
		var newGameObjectName = "My game object";
		gameObject.name = newGameObjectName;

		//Assert
		//The object has a new name
		Assert.AreEqual (newGameObjectName, gameObject.name);
	}
}
