using Objects.Immovable.Items;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

namespace Objects.Inventory
{
	[Serializable]
	public abstract class InventoryItem : List<Item>
    {
		[HideInInspector]
		public Inventory inventory;
		public Vector2 tableOffset;

        public InventoryItem(Item itemController, Inventory inventory)
        {
            this.inventory = inventory;
        }
    }

	#if UNITY_EDITOR
	[CustomEditor(typeof(InventoryItem))]
	public class InventoryItemEditor : Editor {
		SerializedProperty items;
		SerializedProperty tableOffset;

		override public void OnInspectorGUI() {
			FindSerializedProperties();
			DrawInspector();
		}

		void FindSerializedProperties() {
			items	= serializedObject.FindProperty("inventory");
			tableOffset	= serializedObject.FindProperty("base");
		}

		void DrawInspector() {
			EditorGUILayout.PropertyField(items);
			EditorGUILayout.PropertyField(tableOffset);
			serializedObject.ApplyModifiedProperties();
		}
	}
	#endif

}