using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Objects.Movable.Characters.Individuals;
using Data;
using System.Text;
using Objects.Movable.Characters;
using UI;
using UI.Dialogues;

public class InventoryTests {

    [SetUp]
    public void SetUp()
    {
        SaveManager.Reset();
        SaveManager.Initialize();
        SaveManager.NewSave("Save Test");
        Assert.IsNotNull(MacabreWorld.current);

        //foreach (var character in MacabreWorld.current.characters.characterControllers)
        //    Debug.Log(character.name);
    }

    [TearDown]
    public void TearDown()
    {
        UIManager.Find<ConversationDialogue>().Reset();
        UIManager.Find<ConversationDialogue>().TurnOff();
    }

    [Test]
	public void AddToPlayerInventory()
	{
        // Arrange
        Debug.Log("Initializing Conversation");
        //PlayerController player = Characters.playerController;
        
        // Assert

        
    }
}
