using UnityEngine;
using System.Collections;
using System;

namespace Environment
{

    public class CameraScript : MonoBehaviour
    {
        public static CameraScript main = null;

        public bool EnableCameraDrag;
        public bool EnableCameraFollow;

        public float dragSpeed = 1;
        private Vector3 dragOrigin;

        private Transform player;

        private Vector3 offset;

        private Vector3 startLocation, stopLocation;
        private bool followSlowly = false;

        #region Setup

        void Awake()
        {
            if (main == null)
                main = this;
            else if (main != null)
                Destroy(gameObject);
            DontDestroyOnLoad(this);
        }

        void FindGameObjects()
        {
            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                player = GameObject.FindGameObjectWithTag("Player").transform;
                offset = transform.position - player.transform.position;
                CancelInvoke("FindGameObjects");
            }
        }

        public void Start()
        {
            InvokeRepeating("AdjustCameraPosition", 0f, 0.5f);
            SetScreenLimits();
        }

        void OnEnable()
        {
            InvokeRepeating("FindGameObjects", 0, 1);
        }

        void Update()
        {
            if (player == null) return;
            if (EnableCameraDrag) CameraDrag();
            if (EnableCameraFollow && player) CameraFollow();
        }

        #endregion

        #region Camera Movement

        public void AdjustCameraPosition()
        {
            try
            {
                player = GameObject.FindWithTag("Player").transform;
                gameObject.transform.position.Set(player.position.x, player.position.y, -10);
            }
            catch (NullReferenceException)
            {
                return;
            }
            CancelInvoke("AdjustCameraPosition");
        }

        private void CameraDrag()
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragOrigin = Input.mousePosition;
                return;
            }

            if (!Input.GetMouseButton(0)) return;

            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0.0f);

            transform.Translate(move, Space.World);
        }

        private void CameraFollow()
        {
            try
            {
                player = GameObject.FindWithTag("Player").transform;
            }
            catch (MissingComponentException)
            {
                return;
            }

            float distance = Vector3.Distance(transform.position, player.transform.position + offset);
            Debug.DrawLine(transform.position, player.transform.position, Color.red);

            if (distance > 0.5f && !followSlowly)
            {
                StartCoroutine(SmoothMoveCamera());
                followSlowly = true;
            }
        }

        private float screenMaxX, screenMinX, screenMaxY, screenMinY;

        private void SetScreenLimits()
        {
            float cameraHeight = Camera.main.orthographicSize * 2.0f;
            cameraHeight /= GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PixelPerfectCamera>().cameraZoom;
            float cameraWidth = cameraHeight * Screen.width / Screen.height;


            if (Settings.debugCAMERA) Debug.Log(cameraHeight + ", " + cameraWidth);

            GameObject overWorld = GameObject.FindGameObjectWithTag("Overworld");
            if (overWorld == null)
            {
                if (Settings.debugCAMERA) Debug.LogError("Overworld not found");
                return;
            }

            Vector2 mapCenter = overWorld.transform.position;
            float mapWidth = overWorld.GetComponent<SpriteRenderer>().bounds.max.x - overWorld.GetComponent<SpriteRenderer>().bounds.min.x;
            float mapHeight = overWorld.GetComponent<SpriteRenderer>().bounds.max.y - overWorld.GetComponent<SpriteRenderer>().bounds.min.y;


            // Checks if it is within the values
            screenMaxX = mapCenter.x + mapWidth / 2.0f - cameraWidth / 2.0f;
            screenMinX = mapCenter.x - mapWidth / 2.0f + cameraWidth / 2.0f;
            screenMaxY = mapCenter.y + mapHeight / 2.0f - cameraHeight / 2.0f;
            screenMinY = mapCenter.y - mapHeight / 2.0f + cameraHeight / 2.0f;

            if (Settings.debugCAMERA) Debug.Log("Screen Coordinates: " + screenMaxX + ", " + screenMaxY + ", " + screenMinX + ", " + screenMinY);

        }

        private IEnumerator SmoothMoveCamera()
        {

            stopLocation = new Vector2(
                Mathf.Clamp((player.transform.position + offset).x, screenMinX, screenMaxX),
                Mathf.Clamp((player.transform.position + offset).y, screenMinY, screenMaxY));

            while ((transform.position - stopLocation).sqrMagnitude > 0.01f)
            {
                Vector3 newPosition = Vector3.MoveTowards(transform.position, stopLocation, Time.deltaTime * Settings.moveMovementSpeed);
                transform.position = newPosition;

                stopLocation = new Vector3(
                    Mathf.Clamp((player.transform.position + offset).x, screenMinX, screenMaxX),
                    Mathf.Clamp((player.transform.position + offset).y, screenMinY, screenMaxY), -13);

                yield return null;
            }
            followSlowly = false;
            yield break;
        }

        #endregion

    }
}
