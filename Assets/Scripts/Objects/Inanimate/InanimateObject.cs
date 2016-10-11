using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Objects
{
    public abstract class InanimateObject : MacabreObject
    {
        private SpriteRenderer spriteRenderer;

        public bool hasInspectText;
        public bool hasAction = false;

        private int baseSortingLayer;
        private int viewCount;

        [HideInInspector]
        public string inspectText;
        public string objectLocation;

        protected void Awake()
        {
            viewCount = 0;
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            baseSortingLayer = spriteRenderer.sortingLayerID;

            if (objectLocation == "")
            {
                objectLocation = FindObjectLocation(transform);
            }

            if (objectLocation == "") Debug.LogError("No ObjectLocation Tag found on " + gameObject.name);
        }

        protected void Start()
        {
        }

        string FindObjectLocation(Transform instance)
        {
            while (instance.parent != null)
            {
                if (instance.parent.tag == "ObjectLocation")
                {
                    return instance.parent.gameObject.name;
                }
                instance = instance.parent.transform;
            }
            return null; // Could not find a parent with given tag.
        }

        void DisplayInspectText(bool toggle)
        {
            if (!toggle)
            {
                string inspectText;
                inspectText = DatabaseManager.main.GetInspectText(objectLocation, gameObject.name, out viewCount);
                Debug.Log(gameObject.name + ": " + inspectText);
                UIManager.main.DisplayDialogueScreen(gameObject.GetComponent<SpriteRenderer>().sprite, gameObject.name, inspectText, true);

            }
            else
            {
                UIManager.main.CloseDialogueScreen();
            }
        }

        public void Hide()
        {
            var c = spriteRenderer.color;
            c.a = 0;
        }

        public void Show()
        {
            var c = spriteRenderer.color;
            c.a = 1;
        }

        void OnTriggerStay2D(Collider2D other)
        {

            if (other.tag == "PlayerSprite" || other.tag == "CharacterSprite")
            {

                if (other.GetComponent<SpriteRenderer>() == null) return;
                var sortLayer = other.GetComponent<SpriteRenderer>();

                if (sortLayer.sortingLayerName == "Character - Back 1") spriteRenderer.sortingLayerName = "Objects - Back 2";
                else if (sortLayer.sortingLayerName == "Character - Back 2") spriteRenderer.sortingLayerName = "Objects - Back 3";
                else if (sortLayer.sortingLayerName == "Character - Middle 1") spriteRenderer.sortingLayerName = "Objects - Middle 2";
                else if (sortLayer.sortingLayerName == "Character - Middle 2") spriteRenderer.sortingLayerName = "Objects - Middle 3";
                else if (sortLayer.sortingLayerName == "Character - Front 1") spriteRenderer.sortingLayerName = "Objects - Front 2";
                else if (sortLayer.sortingLayerName == "Character - Front 2") spriteRenderer.sortingLayerName = "Objects - Front 3";
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "PlayerSprite" || other.tag == "CharacterSprite")
            {
                spriteRenderer.sortingLayerID = baseSortingLayer;
            }
        }
    }
}