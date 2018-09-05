using UnityEngine;
using System.Collections;

namespace Objects.Movable.Characters
{
	[System.Serializable]
	public class CharacterStatistics
	{
		[Range(1, 100)]
		public int health = 100;

		[Range(-50, 50)]
        public int sanity = 0;

		public int attackDamage = 0;
        
	}
}
