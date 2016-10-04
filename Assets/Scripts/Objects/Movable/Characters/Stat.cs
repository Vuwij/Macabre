using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Objects.Movable.Characters
{
    class Stat
    {
        public class CharacterStat
        {
            [Range(0, 100)]
            public int value = 0;

            public CharacterStat()
            {
                value = 100;
            }
            public CharacterStat(int value_)
            {
                value = value_;
            }

            public static CharacterStat operator +(CharacterStat a, CharacterStat b)
            {
                return new CharacterStat(a.value + b.value);
            }
            public static CharacterStat operator -(CharacterStat a, CharacterStat b)
            {
                return new CharacterStat(a.value - b.value);
            }
            public static CharacterStat operator ++(CharacterStat a)
            {
                a.value++;
                return a;
            }
            public static CharacterStat operator --(CharacterStat a)
            {
                a.value--;
                return a;
            }
        }
    }
}
