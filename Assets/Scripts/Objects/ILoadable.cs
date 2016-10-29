using System;
using System.Runtime.Serialization;

namespace Objects
{
    public interface ILoadable
    {
        /// <summary>
        /// Creates the new game data from scratch, happens then a new game is created
        /// </summary>
        [OnSerialized]
        void CreateNew();
        
        /// <summary>
        /// Loads everything from existing game, happens when a game is loaded and new game created
        /// </summary>
        [OnDeserialized]
        void LoadAll();
    }
}