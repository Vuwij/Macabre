using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

namespace UI.Panels.Inventory
{
    public abstract class ItemStackUI
    {
        public GameObject imageParent;
        
        public ItemStackUI(GameObject imageParent)
        {
            this.imageParent = imageParent;
        }

        public abstract int Count { get; set; }

        public static Queue<ItemStackUI> currentlySelected = new Queue<ItemStackUI>(2);
        public bool isSelected
        {
            get { return currentlySelected.Contains(this); }
        }
        public void Select()
        {
            if (currentlySelected.Contains(this))
            {
                ItemStackUI otherInQueue = currentlySelected.Where(x => x != this)
                    .FirstOrDefault();

                currentlySelected.Clear();
                if (otherInQueue != null) currentlySelected.Enqueue(otherInQueue);
            }
            else if (currentlySelected.Count == 0) currentlySelected.Enqueue(this);
            else if (currentlySelected.Count == 1)
            {
                if (currentlySelected.Peek() is ItemStackUIClassB || this is ItemStackUIClassB) currentlySelected.Dequeue();
                currentlySelected.Enqueue(this);
            }
            else if (currentlySelected.Count == 2)
            {
                if (this is ItemStackUIClassB)
                {
                    currentlySelected.Clear();
                    currentlySelected.Enqueue(this);
                }
                else
                {
                    currentlySelected.Dequeue();
                    currentlySelected.Enqueue(this);
                }
            }
			else if (currentlySelected.Count > 2) throw new UnityException("Cannot have more than 2 items selected");
        }

        public void Unhighlight()
        {
            if(backgroundColor != null)
            {
                backgroundColor.color = unhighlightedColor;
            }
        }

        public void Highlight()
        {
            if (backgroundColor != null)
            {
                backgroundColor.color = highlightedColor;
            }
        }

        private static Color highlightedColor = new Color(0.73f, 0.71f, 0.44f, 1.0f);
        private static Color unhighlightedColor = new Color(0.15f, 0.15f, 0.15f, 1.0f);

        private Image backgroundColor
        {
            get {
                return imageParent.GetComponentsInChildren<Image>()
                    .Where(x => x.gameObject.name == "Background")
                    .SingleOrDefault();
            }
        }

        public static void RefreshSelectionUI()
        {
            foreach (var item in currentlySelected)
            {
                item.Highlight();
            }
        }
    }
}
