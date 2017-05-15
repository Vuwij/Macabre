using UnityEngine;
using System.Runtime.Serialization;

namespace Objects
{
    // An Entity List is a serializable class that gets loaded and saved
	public abstract class EntityList : ILoadable
    {
        public abstract void CreateNew();
        public abstract void LoadAll();
    }
}
