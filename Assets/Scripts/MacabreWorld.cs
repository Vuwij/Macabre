using UnityEngine;
using Environment;
using Environment.Time;
using System;
using Data;
using Objects;
using Objects.Inanimate;
using Objects.Inanimate.Items;
using Objects.Inanimate.World;
using Objects.Movable;
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

    // The overworld contains the world, the maps and all of the buildings
    [DataMember(IsRequired = true, Order = 0)]
    public Overworld overWorld = new Overworld();

    // The gameclock contains informations about the date and time
    [DataMember(IsRequired = true, Order = 1)]
    public Environment.Time.Time gameTime = new Environment.Time.Time();

    // The characters is the one you want to 
    [DataMember(IsRequired = true, Order = 2)]
    public Characters characters = new Characters();
    
    public void LoadAll()
    {
        Load(overWorld);
        Load(characters);
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
