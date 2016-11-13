using UnityEngine;
using UnityEditor;
using NUnit.Framework;

using System;
using Data;
using Exceptions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveManagerTests {

    [SetUp]
    public void SetUp()
    {
        SaveManager.Reset();
    }

    [TearDown]
    public void TearDown()
    {
        SaveManager.Reset();
    }

    [Test]
	public void SaveManagerInitializeTest()
	{
        SaveManager.Initialize();
	}

    [Test]
    public void SaveManagerNewDeleteSaveTest()
    {
        SaveManager.Initialize();
        Assert.True(SaveManager.allSaveInformation.SaveCount == 0);
        SaveManager.NewSave("Save Test 1");
        SaveManager.NewSave("Save Test 2");
        SaveManager.NewSave("Save Test 3");
        Debug.Log(SaveManager.allSaveInformation.SaveCount);
        Assert.True(SaveManager.allSaveInformation.SaveCount == 3);
        AssertFail(() => { SaveManager.DeleteSave("Save Test 3"); });
        Debug.Log(SaveManager.allSaveInformation.SaveCount);
        Assert.True(SaveManager.allSaveInformation.SaveCount == 3);
        SaveManager.DeleteSave("Save Test 2");
        SaveManager.DeleteSave("Save Test 1");
        Assert.True(SaveManager.allSaveInformation.SaveCount == 1);
    }

    [Test]
    public void SaveManagerLoadSaveTest()
    {
        SaveManager.Initialize();
        GameSettings.useSavedXMLConfiguration = false;
        SaveManager.NewSave("Save Test");
        SaveManager.LoadSave("Save Test");
        AssertFail(() => { SaveManager.DeleteSave("Save Test"); });
    }

    [Test]
    public void SaveManagerLoadSaveFromXMLTest()
    {
        SaveManager.Initialize();
        GameSettings.useSavedXMLConfiguration = false;
        SaveManager.NewSave("Save Test");
        SaveManager.SaveCurrentAsMaster();
        GameSettings.useSavedXMLConfiguration = true;
        SaveManager.LoadSave("Save Test");
        AssertFail(() => { SaveManager.DeleteSave("Save Test"); });
    }

    [Test]
    public void SaveALotOfSaves()
    {
        SaveManager.Initialize();
        GameSettings.useSavedXMLConfiguration = false;
        for(int i = 0; i < 10; i++)
            SaveManager.NewSave("Save " + i);
        for (int i = 0; i < 10; i++)
            SaveManager.LoadSave("Save " + i);
        for (int i = 0; i < 9; i++)
            SaveManager.DeleteSave("Save " + i);
        Assert.True(SaveManager.allSaveInformation.SaveCount == 1);
    }

    [Test]
    public void SaveDataSameWhenResaved()
    {
        SaveManager.Initialize();
        GameSettings.useSavedXMLConfiguration = false;
        Save save1 = SaveManager.NewSave("Save Test");
        int save1Size = GetObjectSize((new FileInfo(save1.worldXMLLocation)).Length);
        Save save2 = SaveManager.NewSave("Save Test 2");
        int save2Size = GetObjectSize((new FileInfo(save2.worldXMLLocation)).Length);
        Save save3 = SaveManager.LoadSave("Save Test");
        int save3Size = GetObjectSize((new FileInfo(save3.worldXMLLocation)).Length);
        
        Assert.True(save1 == save3);
        Assert.True(save1Size == save2Size);
        Assert.True(save1Size == save3Size);
    }

    public void AssertFail(Action functionToTest)
    {
        try
        {
            functionToTest();
            Assert.Fail("Exception should be thrown");
        }
        catch (MacabreException) { }
    }

    private int GetObjectSize(object TestObject)
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        byte[] Array;
        bf.Serialize(ms, TestObject);
        Array = ms.ToArray();
        return Array.Length;
    }
}
