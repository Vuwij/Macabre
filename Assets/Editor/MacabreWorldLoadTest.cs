using UnityEngine;
using UnityEditor;
using NUnit.Framework;

using Data;

public class MacabreWorldLoadTest {

    [SetUp]
    public void SetUp()
    {
        SaveManager.Reset();
        SaveManager.Initialize();
        SaveManager.NewSave("Save Test");
        Assert.IsNotNull(MacabreWorld.current);
    }

    [TearDown]
    public void TearDown()
    {
    }

    [Test]
    public void LoadingTestOverworld()
    {
        SaveManager.OnApplicationQuit();
        SaveManager.LoadSave("Save Test");
        SaveManager.OnApplicationQuit();
    }

    [Test]
	public void LoadingTestCharacters()
	{
	}
}
