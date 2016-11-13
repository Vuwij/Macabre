using UnityEngine;
using System.Runtime.Serialization;

namespace Objects
{
    public abstract class EntityList : ILoadable
    {
        public abstract void CreateNew();
        public abstract void LoadAll();
    }
}
