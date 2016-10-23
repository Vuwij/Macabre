using UnityEngine;
using System.Collections;

namespace Objects.Movable.Characters.Individuals
{
    public partial class PlayerController : CharacterController
    {
        void Update()
        {
            if (mouseMovement) moveOnMouseClick();
            if (keyboardMovement) moveUsingKeyboard();

            AnimateMovement();
        }
    }
}