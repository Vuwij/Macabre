using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Objects.Movable
{
    public abstract class MovingObject : MacabreObject
    {
        public Vector3 currentPosition;          // Starting Point
        public Vector3 destinationPosition;      // Destination Point

        public bool isMoving = false;
    }
}