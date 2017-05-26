using UnityEngine;
using UnityEditor;
using NUnit.Framework;

using Data;

public class MacabreWorldLoadTests {

    [SetUp]
    public void SetUp()
    {
        Saves.Reset();
        Saves.Initialize();
        Saves.New("Save Test");
        Assert.IsNotNull(World.current);
    }

    [TearDown]
    public void TearDown()
    {
    }

    [Test]
    public void LoadingTestOverworld()
    {
        Saves.OnApplicationQuit();
        Saves.Load("Save Test");
        Saves.OnApplicationQuit();
    }

    [Test]
	public void LoadingTestCharacters()
	{
	}
}
