using UnityEngine;
using System.Collections;

namespace Objects.Movable.Characters.Individuals
{
    public sealed partial class PlayerController : CharacterController
    {
        void Update()
        {
            if (mouseMovement) moveOnMouseClick();
            if (keyboardMovement) moveUsingKeyboard();

            AnimateMovement();
        }
    }
}