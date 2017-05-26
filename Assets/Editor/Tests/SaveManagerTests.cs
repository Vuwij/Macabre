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
        Saves.Reset();
    }

    [TearDown]
    public void TearDown()
    {
        Saves.Reset();
    }

    [Test]
	public void SaveManagerInitializeTest()
	{
        Saves.Initialize();
	}

    [Test]
    public void SaveManagerNewDeleteSaveTest()
    {
        Saves.Initialize();
        Assert.True(Saves.allSaveInformation.SaveCount == 0);
        Saves.New("Save Test 1");
        Saves.New("Save Test 2");
        Saves.New("Save Test 3");
        Debug.Log(Saves.allSaveInformation.SaveCount);
        Assert.True(Saves.allSaveInformation.SaveCount == 3);
        AssertFail(() => { Saves.Delete("Save Test 3"); });
        Debug.Log(Saves.allSaveInformation.SaveCount);
        Assert.True(Saves.allSaveInformation.SaveCount == 3);
        Saves.Delete("Save Test 2");
        Saves.Delete("Save Test 1");
        Assert.True(Saves.allSaveInformation.SaveCount == 1);
    }

    [Test]
    public void SaveManagerLoadSaveTest()
    {
        Saves.Initialize();
        GameSettings.useSavedXMLConfiguration = false;
        Saves.New("Save Test");
        Saves.Load("Save Test");
        AssertFail(() => { Saves.Delete("Save Test"); });
    }

    [Test]
    public void SaveALotOfSaves()
    {
        Saves.Initialize();
        GameSettings.useSavedXMLConfiguration = false;
        for(int i = 0; i < 10; i++)
            Saves.New("Save " + i);
        for (int i = 0; i < 10; i++)
            Saves.Load("Save " + i);
        for (int i = 0; i < 9; i++)
            Saves.Delete("Save " + i);
        Assert.True(Saves.allSaveInformation.SaveCount == 1);
    }

    [Test]
    public void SaveDataSameWhenResaved()
    {
        Saves.Initialize();
        GameSettings.useSavedXMLConfiguration = false;
        Save save1 = Saves.New("Save Test");
        int save1Size = GetObjectSize((new FileInfo(save1.worldXMLLocation)).Length);
        Save save2 = Saves.New("Save Test 2");
        int save2Size = GetObjectSize((new FileInfo(save2.worldXMLLocation)).Length);
        Save save3 = Saves.Load("Save Test");
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
