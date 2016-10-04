using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Xml.Serialization;

/// <summary>
/// Everything object that you want to save will first
/// 1. Inherit the ISavable Interface. So you can load, save and get the save data
/// 2. If the object is a moving object it will have a name, an objectTransform
/// 3. loadData and saveData must be virtual methods
/// 4. to save the game. The save manager will go through every ISavable, and get component T, and serialize that gameobject
/// 5. Save Data will be the class
/// </summary>

public interface ISavable
{
	void saveInfo ();
	void loadInfo ();
}

/*
[System.Serializable]
[XmlInclude (typeof (MData.Component))]
public struct GameData
{
	public static GameData main;

	// List of all the gameData
	public List<MData.Component> gameData;

	public GameData (Save s)
	{
		gameData = new List<MData.Component> ();
		main = this;
	}

	public T Find<T> (MacabreThing m)
		where T : MData.Component
	{
		return gameData.Find (x => (x.gameObjectData == m)) as T;
	}

	public void Set<T> (T t)
		where T : MData.Component
	{
		var v = gameData.Find (x => (x.gameObject == t.gameObject)) as T;

		Debug.Log (v.GetType ());

		v = t;
	}

	public static void SaveAllInfo ()
	{
		Debug.Log ("Saving All Info");
		foreach (var d in GameData.main.gameData) {
			var s = d.gameObjectData.GetComponent<ISavable> ();
			if (s != null) {
				s.saveInfo ();
			}
		}
	}

	public static void LoadAllInfo ()
	{
		Debug.Log ("Loading All Info");

		foreach (var d in GameData.main.gameData) {
			var s = d.gameObjectData.GetComponent<ISavable> ();
			if (s != null) {
				s.loadInfo ();
			}
		}
	}

	public static void printGameObjectData ()
	{
		Debug.Log ("Print all Game Objects");

		foreach (var d in GameData.main.gameData) {
			Debug.Log (d.gameObject.name);
		}
	}
}
#endregion

namespace Data
{

/// <summary>
/// The basic of a gameobject
/// <term> gameObject: 		The gameobject it is attached to</term>
/// <term> gameObjectData: 	The script that is attached to the gameobject</term>
/// </summary>

[XmlInclude (typeof (MovingObject))]
public abstract class Component
{
    [XmlIgnore] public GameObject gameObject;
    [XmlIgnore] public MacabreThing gameObjectData;
    [XmlAttribute] public string pathName;

    public Component () { }
    public Component (GameObject gameObject_, string pathName_)
    {
        AttachGameObject (gameObject_);
        pathName = pathName_;

        var s = gameObject.GetComponents<MacabreThing> ();

        GameData.main.gameData.Add (this);

        foreach (MacabreThing m in s) {
            if (m is ISavable) {
                ((ISavable)m).saveInfo ();
            }
        }
    }

    public void AttachGameObject (GameObject gameObject_)
    {
        gameObject = gameObject_;
        var s = gameObject.GetComponents<MacabreThing> ();

        // Save the data initally
        foreach (MacabreThing m in s) {
            if (m is ISavable) {
                gameObjectData = m;
            }
        }

    }

}

[XmlInclude (typeof (Character))]
[XmlInclude (typeof (Vector3))]
public class MovingObject : Component
{
    public MovingObject () { }
    public MovingObject (GameObject gameObject_, string name_) : base (gameObject_, name_) { }
    public Vector3 currentPosition = new Vector3 ();
}

[XmlInclude (typeof (Player))]
[XmlInclude (typeof (CharacterStat))]
public class Character : MovingObject
{
    public Character () { }
    public Character (GameObject gameObject_, string name_) : base (gameObject_, name_) { }
    public string location;
    public string name;
    public CharacterStat sanity;
}


public class Player : Character
{
    public Player () { }
    public Player (GameObject gameObject_, string name_) : base (gameObject_, name_) {}
}
}
*/