using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Objects.Inventory.Individual;
using Objects.Inventory;
using Objects.Movable.Characters;
using UI.Panels.Inventory;
using Data.Database;
using Objects.Immovable.Items;
using Objects.Movable.Characters.Individuals;

namespace UI.Panels
{
    public sealed class InventoryPanel : UIPanel, UIGameObject
    {
        public override string name
        {
            get { return "Inventory Panel"; }
        }

        public CharacterInventory inventory
        {
            get
            {
				return GameObject.Find("Player").GetComponent<Player>().inventory;
            }
        }
        
        public ItemStackUIClassA[] itemStackUIClassAs = new ItemStackUIClassA[6];
        
        public ItemStackUIClassB[] itemStackUIClassBs = new ItemStackUIClassB[2];

        #region Display

        private void Start()
        {
            Transform classAObjectsParent = gameObject.GetComponentsInChildren<Transform>()
                                .Where(x => x.name == "Class A Objects")
                                .FirstOrDefault();
            Transform classBObjectsParent = gameObject.GetComponentsInChildren<Transform>()
                    .Where(x => x.name == "Class B Objects")
                    .FirstOrDefault();

            // Class A objects
            var classAStacks = classAObjectsParent.GetComponentsInChildren<Transform>()
                .Where(x => x.name.Contains("Stack"))
                .ToArray();
			if (classAStacks.Length != 6) throw new UnityException("There are not 6 stacks in Class A Objects in the inventory");

            for (int i = 0; i < 6; i++)
                this.itemStackUIClassAs[i] = new Inventory.ItemStackUIClassA(classAStacks[i].gameObject);

            // Class B objects
            var classBStacks = classBObjectsParent.GetComponentsInChildren<Transform>()
                .Where(x => x.name.Contains("Stack"))
                .ToArray();
			if (classBStacks.Length != 2) throw new UnityException("There are not 2 stacks in Class B Objects");

            for (int i = 0; i < 2; i++)
                this.itemStackUIClassBs[i] = new ItemStackUIClassB(classBStacks[i].gameObject);

        }
        
        public override void TurnOn()
        {
			Game.main.UI.FadeBackground = true;
            base.TurnOn();
            RefreshItems();
        }

        public override void TurnOff()
        {
            base.TurnOff();
			Game.main.UI.FadeBackground = false;
        }

        private void RefreshItems()
        {
            // Update all the images Class A
            for (int i = 0; i < 6; i++)
            {
                if (inventory.classAItems.Count > i)
                    itemStackUIClassAs[i].Update(inventory.classAItems[i]);
                else
                    itemStackUIClassAs[i].Update(null);
            }

            // Update all the images Class B
            for (int i = 0; i < 2; i++)
            {
                if (inventory.classBItems.Count > i)
                    itemStackUIClassBs[i].Update(inventory.classBItems[i]);
                else
                    itemStackUIClassBs[i].Update(null);
            }

            // Refresh Selected
            ItemStackUI.currentlySelected.Clear();
            foreach (var item in itemStackUIClassAs) item.Unhighlight();
            ItemStackUI.RefreshSelectionUI();
        }

        #endregion

        #region Selection
        
        public void SelectClassA(int index)
        {
			if (index < 0 && index >= 6) throw new UnityException("Invalid Index");
            DeselectAll();
            itemStackUIClassAs[index].Select();
            ItemStackUI.RefreshSelectionUI();
        }

        public void SelectClassB(int index)
        {
			if (index < 0 && index >= 2) throw new UnityException("Invalid Index");
            DeselectAll();
            itemStackUIClassBs[index].Select();
            ItemStackUI.RefreshSelectionUI();
        }

        private void DeselectAll()
        {
            foreach (var item in itemStackUIClassAs)
                item.Unhighlight();
            foreach (var item in itemStackUIClassBs)
                item.Unhighlight();
        }
        
        #endregion

        #region Buttons

        public void Combine()
        {
            if (ItemStackUI.currentlySelected.Count != 2) return;
            
            ItemStackUI[] toCombine = ItemStackUI.currentlySelected.ToArray();
            if (toCombine[0].Count + toCombine[1].Count > 4) return;

            ItemStackUIClassA itemUI1 = null, itemUI2 = null;
            for (int i = 0; i < 6; i++)
            {
                if (itemStackUIClassAs[i] == toCombine[0])
                {
                    itemUI1 = (toCombine[0] as ItemStackUIClassA);
                    itemUI2 = (toCombine[1] as ItemStackUIClassA);
                    break;
                }
                if(itemStackUIClassAs[i] == toCombine[1])
                {
                    itemUI1 = (toCombine[1] as ItemStackUIClassA);
                    itemUI2 = (toCombine[0] as ItemStackUIClassA);
                    break;
                }
            }

            // Attempt a merge only if the item is single
            if (itemUI1.inventoryItem == null || itemUI2.inventoryItem == null) return;
            itemUI1.inventoryItem += itemUI2.inventoryItem;
            
            RefreshItems();
        }

        public void Seperate()
        {
            // Select the currently selected item
            if (ItemStackUI.currentlySelected.Count != 1) return;
            var itemStack = ItemStackUI.currentlySelected.Peek();

            // Check if a seperation is possible and remove one from the bundle
            if (!(itemStack is ItemStackUIClassA)) return;
            if (itemStack.Count <= 1) return;

            // Check if there is available space
            if (!(itemStackUIClassAs.Any(x => x.Count == 0))) return;

            ItemStackUIClassA itemStackUIClassARemoveFrom = itemStack as ItemStackUIClassA;

            // The actual transfer
            var itemToBeTransferred = itemStackUIClassARemoveFrom.inventoryItem.Last();
            inventory.Add(itemToBeTransferred);
            itemStackUIClassARemoveFrom.inventoryItem.Remove(itemToBeTransferred);
            
            RefreshItems();
        }

        public void Inspect()
        {

        }

        public void Drop()
        {
            // Select the currently selected item
            if (ItemStackUI.currentlySelected.Count == 0) return;
            var itemStack = ItemStackUI.currentlySelected.Dequeue();

            // Obtain the inventoryItem from the stack
            InventoryItem inventoryItem;
            if (itemStack is ItemStackUIClassA)
                inventoryItem = (itemStack as ItemStackUIClassA).inventoryItem;
            else
                inventoryItem = (itemStack as ItemStackUIClassB).inventoryItem;

            // Drop the inventory Item (from PlayerInventory)
            inventory.Drop(inventoryItem);

            // Refresh the UI
            RefreshItems();
        }

        public void Return()
        {
            TurnOff();
        }

        #endregion
    }
}
