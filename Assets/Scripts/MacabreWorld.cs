using UnityEngine;
using Environment;
using Environment.Time;
using System;
using Data;
using Objects;
using Objects.Inanimate.Items;
using Objects.Inanimate.World;
using Objects.Inanimate.Buildings;
using Objects.Movable.Characters;
using System.Runtime.Serialization;
/**
 *  This is it, the class that contains everything
 */

[DataContract(Namespace = "")]
[Serializable]
public class MacabreWorld
{
    // The current instance of the MacabreWorld
    [IgnoreDataMember]
    public static MacabreWorld current
    {
        get { return SaveManager.CurrentSave.world; }
    }

    // The overworld contains the world background and natural elements
    [DataMember(IsRequired = true, Order = 0)]
    public Overworld overWorld = new Overworld();

    // Buildings contain paths and furniture
    [DataMember(IsRequired = true, Order = 1)]
    public Buildings buildings = new Buildings();

    // Items located in the world
    [DataMember(IsRequired = true, Order = 2)]
    public Items items = new Items();

    // The gameclock contains informations about the date and time
    [DataMember(IsRequired = true, Order = 3)]
    public Environment.Time.Time gameTime = new Environment.Time.Time();

    // The characters is the one you want to 
    [DataMember(IsRequired = true, Order = 4)]
    public Characters characters = new Characters();
    
    public void LoadAll()
    {
        Load(overWorld);
        Load(characters);
        Load(items);
    }

    ~MacabreWorld()
    {
        overWorld = null;
        gameTime = null;
        characters = null;

        GC.Collect();
    }

    private void Load(ILoadable loadingObject)
    {
        if (GameSettings.createNewGame) loadingObject.CreateNew();
        loadingObject.LoadAll();
    }
}
