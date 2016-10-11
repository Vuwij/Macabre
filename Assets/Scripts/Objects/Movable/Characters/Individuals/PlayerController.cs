using UnityEngine;
using System.Collections;

namespace Objects.Movable.Characters
{
    public class PlayerController : CharacterController
    {
        #region Initialize

        protected override void Start()
        {
            base.Start();
        }

        void Update()
        {
            if (mouseMovement) moveOnMouseClick();
            if (keyboardMovement) moveUsingKeyboard();

            AnimateMovement(isMoving);
        }

        #endregion

        #region SaveData
        public override void saveInfo()
        {
            base.saveInfo();

            GameData.main.Find<MData.Character>(this);
        }
        public override void loadInfo()
        {
            base.loadInfo();
            var g = GameData.main.Find<MData.Character>(this);
            Debug.Log(g.pathName);
        }

        #endregion

        #region MovementControl

        Vector2 inputDirection;
        private bool characterIsRunning;

        void moveUsingKeyboard()
        {

            if (Input.GetButton("SpeedUp"))
                characterIsRunning = true;
            else
                characterIsRunning = false;

            if (characterIsRunning) Settings.moveMovementSpeed = 6.0f;
            else Settings.moveMovementSpeed = 3.0f;

            if (lockMovement)
            {
                rb2D.velocity = Vector2.zero;
                isMoving = false;
                return;
            }
            float moveHorizontal = Input.GetAxisRaw("Horizontal") * Settings.moveMovementSpeed;
            float moveVertical = Input.GetAxisRaw("Vertical") * Settings.moveMovementSpeed;

            movementVelocity = new Vector2(moveHorizontal, moveVertical / 2.0f);
            if (!frozen) rb2D.velocity = movementVelocity;
            else rb2D.velocity = Vector2.zero;

            if (rb2D.velocity.sqrMagnitude > 0.1) isMoving = true;
            else if (isMoving) isMoving = false;
        }

        void moveOnMouseClick()
        {
            if (lockMovement)
                return;

            if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown(0)))
            {
#if (UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
                destinationPosition = PixelPerfectCamera.mouseToPixelCamera(Input.mousePosition);
                //destinationPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#elif (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
			destinationPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
#endif

                destinationPosition.z = zAxis; //NO MOVEMENT Z AXIS
                isMoving = true;
            }

            MoveToLocation();
        }

        #endregion

        #region Inventory

        public Inventory.Player inventory = new Inventory.Player();
        public override bool AddToInventory(MItem.Item m)
        {
            // Convert the object into a Macabre Inventory Object by wrapping it

            return inventory.Add(m);
        }

        #endregion
    }
}