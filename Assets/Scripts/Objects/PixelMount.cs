using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Objects.Movable.Characters;

namespace Objects
{
    /// <summary>
	/// Class for mounting onto objects, such as beds, chairs and boxes. This is only for Character so you must ASSERT 
	/// using Debug.Assert() to make sure that the object is mountable
    /// </summary>
	public class PixelMount : PixelStorage
	{
		public void Mount(Character character)
		{
            // TODO 4. finish this
			base.AddObject(character.gameObject);
		}

		public void Unmount(Character character)
		{
			// TODO 5. finish this for unmounting of the bed. 
			base.TakeObject(character.name);
		}
	}
}