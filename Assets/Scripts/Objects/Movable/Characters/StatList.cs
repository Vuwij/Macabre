using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.Scripts.Objects.Movable.Characters
{
    public class StatList
    {
        Dictionary<string, Stat> Stat;

        public StatList(List<string> StatList)
        {
            Stat = new Dictionary<string, Stat>();
            foreach (string s in StatList)
            {
                Stat.Add(s, new Stat());
            }
        }

        public Stat getStat(string s)
        {
            Stat c;
            Stat.TryGetValue(s, out c);
            if (c != null) return c;
            return null;
        }

        public void setStat(string s, int i)
        {
            Stat c;
            Stat.TryGetValue(s, out c);
            if (c != null) c.value = i;
        }
    }
}
