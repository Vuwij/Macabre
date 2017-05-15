using System.Text;
using Conversation;
using Data;
using NUnit.Framework;
using Objects.Movable.Characters;
using UI;
using UI.Dialogues;
using UnityEditor;
using UnityEngine;
using T = System.Diagnostics.Trace;

public class ConversationTests {

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
        Objects.Movable.Characters.CharacterController.conversationState = null;
        UIManager.Find<ConversationDialogue>().Reset();
        UIManager.Find<ConversationDialogue>().TurnOff();
    }

    [Test]
	public void InitializeConversationWithHamen()
	{
        //Arrange
        Debug.Log("Initializing Conversation");
        Objects.Movable.Characters.CharacterController c = MacabreWorld.current.characters.characterControllers.Find(x => x.name.Contains("Innkeeper"));

        //Act
        c.Dialogue().Print();           // Hello stranger How may I help you
        c.Dialogue().Print();           // Display list of options
        c.Dialogue(0).Print();          // I'd like a room...
        c.Dialogue().Print();           // Sure I've got a room ...
        Assert.IsNull(c.Dialogue());    // END

        c.Dialogue().Print();           // Hello stranger, how may I help you
        c.Dialogue().Print();           // Display list of options
        c.Dialogue(1).Print();          // I'd like some ale and hot meal
        c.Dialogue().Print();           // Ah a traveller...
        Assert.IsNull(c.Dialogue());    // END

        c.Dialogue().Print();           // Hello stranger, how may I help you
        c.Dialogue().Print();           // Display list of options
        c.Dialogue(2).Print();          // I'm investigating
        c.Dialogue().Print();           // Can't say I do mister
        c.Dialogue().Print();           // Display list of options
        c.Dialogue(0).Print();          // Keep me informed
        c.Dialogue().Print();           // Of course, heres the key
        Assert.IsNull(c.Dialogue());    // END

        c.Dialogue().Print();           // Hello strange, how may I help you
        c.Dialogue().Print();           // Display list of options
        c.Dialogue(2).Print();          // I'm investigating
        c.Dialogue().Print();           // Can't say I do mister
        c.Dialogue().Print();           // Display list of options
        c.Dialogue(1).Print();          // I've been sent by the king
        c.Dialogue().Print();           // Look I've told you, don't know anything
        c.Dialogue().Print();           // Tell me the truth pops
        c.Dialogue().Print();           // Guards help!
        Assert.IsNull(c.Dialogue());    // END
    }

    [Test]
    public void InitializeConversationWithHamenCheckState()
    {
        //Arrange
        Debug.Log("Initializing Conversation");
        Objects.Movable.Characters.CharacterController c = MacabreWorld.current.characters.characterControllers.Find(x => x.name.Contains("Innkeeper"));

        //Act
        Assert.True(c.Dialogue().conversationViewStatus == ConversationViewStatus.CharacterResponse);
        Assert.True(c.Dialogue().conversationViewStatus == ConversationViewStatus.PlayerMultipleReponse);
        Assert.True(c.Dialogue(0).conversationViewStatus == ConversationViewStatus.PlayerResponse);
        Assert.True(c.Dialogue().conversationViewStatus == ConversationViewStatus.CharacterResponse);
        Assert.IsNull(c.Dialogue());

        Assert.True(c.Dialogue().conversationViewStatus == ConversationViewStatus.CharacterResponse);
        Assert.True(c.Dialogue().conversationViewStatus == ConversationViewStatus.PlayerMultipleReponse);
        Assert.True(c.Dialogue(1).conversationViewStatus == ConversationViewStatus.PlayerResponse);
        Assert.True(c.Dialogue().conversationViewStatus == ConversationViewStatus.CharacterResponse);
        Assert.IsNull(c.Dialogue());

        Assert.True(c.Dialogue().conversationViewStatus == ConversationViewStatus.CharacterResponse);
        Assert.True(c.Dialogue().conversationViewStatus == ConversationViewStatus.PlayerMultipleReponse);
        Assert.True(c.Dialogue(2).conversationViewStatus == ConversationViewStatus.PlayerResponse);
        Assert.True(c.Dialogue().conversationViewStatus == ConversationViewStatus.CharacterResponse);
        Assert.True(c.Dialogue().conversationViewStatus == ConversationViewStatus.PlayerMultipleReponse);
        Assert.True(c.Dialogue(0).conversationViewStatus == ConversationViewStatus.PlayerResponse);
        Assert.True(c.Dialogue().conversationViewStatus == ConversationViewStatus.CharacterResponse);
        Assert.IsNull(c.Dialogue());

        Assert.True(c.Dialogue().conversationViewStatus == ConversationViewStatus.CharacterResponse);
        Assert.True(c.Dialogue().
            conversationViewStatus == ConversationViewStatus.PlayerMultipleReponse);
        Assert.True(c.Dialogue(2).conversationViewStatus == ConversationViewStatus.PlayerResponse);
        Assert.True(c.Dialogue().conversationViewStatus == ConversationViewStatus.CharacterResponse);
        Assert.True(c.Dialogue().conversationViewStatus == ConversationViewStatus.PlayerMultipleReponse);
        Assert.True(c.Dialogue(1).conversationViewStatus == ConversationViewStatus.PlayerResponse);
        Assert.True(c.Dialogue().conversationViewStatus == ConversationViewStatus.CharacterResponse);
        Assert.True(c.Dialogue().conversationViewStatus == ConversationViewStatus.PlayerResponse);
        Assert.True(c.Dialogue().conversationViewStatus == ConversationViewStatus.CharacterResponse);
        Assert.IsNull(c.Dialogue());
    }

    [Test]
    public void InitializeConversationWithOwin()
    {
        //Arrange
        Debug.Log("Initializing Conversation");
        Objects.Movable.Characters.CharacterController c = MacabreWorld.current.characters.characterControllers.Find(x => x.name.Contains("Pardoner"));

        //Act
        c.Dialogue().Print();           // You look troubled
        c.Dialogue(0).Print();          // Sound fair
        c.Dialogue().Print();           // Find me in here
        Assert.IsNull(c.Dialogue());    // END

        c.Dialogue().Print();           // You look troubled
        c.Dialogue(1).Print();          // Sound fair
        c.Dialogue().Print();           // Find me in here
        Assert.IsNull(c.Dialogue());    // END

        c.Dialogue().Print();           // You look troubled
        c.Dialogue(2).Print();          // Sound fair
        c.Dialogue().Print();           // Find me in here
        Assert.IsNull(c.Dialogue());    // END
    }

    [Test]
    public void InitializeConversationWithGrannyGood()
    {
        //Arrange
        Debug.Log("Initializing Conversation");
        Objects.Movable.Characters.CharacterController c = MacabreWorld.current.characters.characterControllers.Find(x => x.name.Contains("GrannyGood"));

        //Act
        c.Dialogue().Print();           // You look troubled
        c.Dialogue(0).Print();          // Sound fair
        c.Dialogue().Print();           // Find me in here
        Assert.IsNull(c.Dialogue());    // END

        c.Dialogue().Print();           // You look troubled
        c.Dialogue(1).Print();          // Sound fair
        c.Dialogue().Print();           // Find me in here
        Assert.IsNull(c.Dialogue());    // END

        c.Dialogue().Print();           // You look troubled
        c.Dialogue(2).Print();          // Sound fair
        c.Dialogue().Print();           // Find me in here
        Assert.IsNull(c.Dialogue());    // END
    }
}