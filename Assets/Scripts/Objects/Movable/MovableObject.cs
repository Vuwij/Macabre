using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Objects;
using Objects.Immovable;

namespace Objects.Movable
{
    public abstract class MovableObject : PixelObject
    {
		protected override void Start() {
			base.Start();
		}
    }
}