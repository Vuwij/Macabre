using UnityEngine;
using System.Collections;

namespace Objects.Movable.Characters
{
	public class CharacterStatistics
	{
		[Range(1, 100)]
		public int health = 100;

		[Range(-50, 50)]
        public int sanity = 0;
	}
}
