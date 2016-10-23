using UnityEngine;
using Environment;
using Environment.Time;
using Data;
using Objects;
using Objects.Inanimate;
using Objects.Inanimate.Items;
using Objects.Inanimate.World;
using Objects.Movable;
using Objects.Movable.Characters;
using System.Xml.Serialization;
/**
 *  This is it, the class that contains everything
 */
public class MacabreWorld
{
    // The current instance of the MacabreWorld
    public static MacabreWorld current
    {
        get { return SaveManager.currentSave.world; }
    }

    // The overworld contains the world, the maps and all of the buildings
    public Overworld overWorld = new Overworld();

    // The gameclock contains informations about the date and time
    public Environment.Time.Time gameTime = new Environment.Time.Time();

    // The list of characters contains all information about characters
    public CharacterControllers characterControllers = new CharacterControllers();
}
