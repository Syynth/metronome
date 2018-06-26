using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using Assets.Code.GameState;

namespace Assets.Code.Editors
{

    [CustomEditor(typeof(Inventory))]
    public class InventoryEditor : Editor
    {

        Dictionary<ItemCategory, ReorderableList> categoryLists = new Dictionary<ItemCategory, ReorderableList>();
        ReorderableList InventoryList;

        public void OnEnable()
        {
            InventoryList = new ReorderableList(serializedObject, serializedObject.FindProperty("Items"), true, true, true, true);
            InventoryList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Items");
            InventoryList.onRemoveCallback = OnRemoveCallback;
            InventoryList.drawElementCallback = OnDrawCallback;
            InventoryList.onAddCallback = OnAddCallback;
            InventoryList.elementHeight = EditorGUIUtility.singleLineHeight * 2 + 6;
        }

        public void OnRemoveCallback(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
        }

        public void OnAddCallback(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoAddButton(list);
        }

        public void OnDrawCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var item = InventoryList.serializedProperty.GetArrayElementAtIndex(index);
            var itemRef = item.FindPropertyRelative("Item").objectReferenceValue as Item;

            var iconNull = itemRef?.DisplayIcon == null;
            var lh = EditorGUIUtility.singleLineHeight;
            var th = iconNull ? 0 : lh * 2 + 2;

            var quantityRect = new Rect(rect.x + th, rect.y + 1, rect.width - th, lh);
            var itemRect = new Rect(rect.x + th, rect.y + quantityRect.height + 3, rect.width - th, lh);
            var spriteRect = new Rect(rect.x, rect.y + 1, th, th);

            GUILayout.BeginHorizontal();

            if (!iconNull)
            {
                EditorGUI.DrawTextureTransparent(spriteRect, itemRef.DisplayIcon.texture, ScaleMode.ScaleToFit);
            }

            GUILayout.BeginVertical();
            EditorGUI.PropertyField(quantityRect, item.FindPropertyRelative("quantity"));
            EditorGUI.PropertyField(itemRect, item.FindPropertyRelative("Item"));
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

        }

        public override void OnInspectorGUI()
        {
            var inv = target as Inventory;
            InventoryList.DoLayoutList();

            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Sort Alphabetical"))
            {
                inv.SortItems(Inventory.InventorySortModes.Alphabetical);
            }
            if (GUILayout.Button("Sort By Category"))
            {
                inv.SortItems(Inventory.InventorySortModes.Category);
            }

            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

    }
}
