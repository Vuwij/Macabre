using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Objects.Inventory.Individual;
using Objects.Inventory;
using Objects.Movable.Characters;
using UI.Panels.Inventory;
using Exceptions;

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
                return Characters.playerController.inventory;
            }
        }
        
        public ClassAItemStack[] classAStack = new ClassAItemStack[6];
        
        public ClassBItemStack[] classBStack = new ClassBItemStack[2];

        #region Display

        public void Start()
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
            if (classAStacks.Length != 6) throw new MacabreException("There are not 6 stacks in Class A Objects in the inventory");

            for (int i = 0; i < 6; i++)
                classAStack[i] = new Inventory.ClassAItemStack(classAStacks[i].gameObject);

            // Class B objects
            var classBStacks = classBObjectsParent.GetComponentsInChildren<Transform>()
                .Where(x => x.name.Contains("Stack"))
                .ToArray();
            if (classBStacks.Length != 2) throw new MacabreException("There are not 2 stacks in Class B Objects");

            for (int i = 0; i < 2; i++)
                classBStack[i] = new ClassBItemStack(classBStacks[i].gameObject);

        }
        
        public override void TurnOn()
        {
            UIManager.FadeBackground = true;
            base.TurnOn();
            RefreshItems();
        }

        public override void TurnOff()
        {
            base.TurnOff();
            UIManager.FadeBackground = false;
        }

        private void RefreshItems()
        {
            // Update all the images Class A
            for (int i = 0; i < 6; i++)
            {
                if (inventory.classAItems.Count > i)
                    classAStack[i].Update(inventory.classAItems[i]);
                else
                    classAStack[i].Update(null);
            }

            // Update all the images Class B
            for (int i = 0; i < 2; i++)
            {
                if (inventory.classBItems.Count > i)
                    classBStack[i].Update(inventory.classBItems[i]);
                else
                    classBStack[i].Update(null);
            }

            // Refresh Selected
            ItemStack.currentlySelected.Clear();
            ItemStack.RefreshSelectionUI();
        }

        #endregion

        #region Selection
        
        public void SelectClassA(int index)
        {
            if (index < 0 && index >= 6) throw new MacabreException("Invalid Index");
            DeselectAll();
            classAStack[index].Select();
            ItemStack.RefreshSelectionUI();
        }

        public void SelectClassB(int index)
        {
            if (index < 0 && index >= 2) throw new MacabreException("Invalid Index");
            DeselectAll();
            classBStack[index].Select();
            ItemStack.RefreshSelectionUI();
        }

        private void DeselectAll()
        {
            foreach (var item in classAStack)
                item.Unhighlight();
            foreach (var item in classBStack)
                item.Unhighlight();
        }
        
        #endregion

        #region Buttons

        public void Combine()
        {
            if (ItemStack.currentlySelected.Count != 2) return;

            ItemStack[] toCombine = ItemStack.currentlySelected.ToArray();
            if (toCombine[0].Count + toCombine[1].Count > 4) return;

            ClassAItemStack firstItem = null, secondItem = null;
            for (int i = 0; i < 6; i++)
            {
                if (classAStack[i] == toCombine[0])
                {
                    firstItem = (toCombine[0] as ClassAItemStack);
                    secondItem = (toCombine[1] as ClassAItemStack);
                    break;
                }
                if(classAStack[i] == toCombine[1])
                {
                    firstItem = (toCombine[1] as ClassAItemStack);
                    secondItem = (toCombine[0] as ClassAItemStack);
                    break;
                }
            }

            firstItem.inventoryItem += secondItem.inventoryItem;
            RefreshItems();
        }

        public void Seperate()
        {

        }

        public void Inspect()
        {

        }

        public void Drop()
        {

        }

        public void Return()
        {
            this.TurnOff();
        }

        #endregion
    }
}
