using UnityEngine;
using System.Collections;
using Environment;

namespace Objects.Movable.Characters.Individuals
{
    public sealed partial class PlayerController : CharacterController
    {
        protected override void Start()
        {
            base.Start();
            MainCamera.TeleportToPlayer();
        }

        void Update()
        {
            if (mouseMovement) moveOnMouseClick();
            if (keyboardMovement) moveUsingKeyboard();

            AnimateMovement();
        }
    }
}