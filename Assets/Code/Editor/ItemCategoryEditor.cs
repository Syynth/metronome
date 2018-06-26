using UnityEngine;
using UnityEditor;

using Assets.Code.GameState;

namespace Assets.Code.Editors
{
    [CustomEditor(typeof(ItemCategory))]
    public class ItemCategoryEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            var itemCategory = target as ItemCategory;

            EditorGUILayout.Space();
            GUILayout.Label($"Item Category: {itemCategory.DisplayName}", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            GUILayout.BeginVertical(EditorStyles.helpBox);
            
            serializedObject.FindProperty("DisplayName").stringValue = EditorGUILayout.TextField("Display Name", itemCategory.DisplayName);

            serializedObject.FindProperty("SortingIndex").intValue = EditorGUILayout.IntField("Sorting Index", itemCategory.SortingIndex);

            EditorGUILayout.Space();

            GUILayout.Label("Flavor Text");
            serializedObject.FindProperty("Description").stringValue = EditorGUILayout.TextArea(itemCategory.Description, GUILayout.Height(70));

            GUILayout.EndVertical();

            EditorGUILayout.Space();
        
            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            var newSprite = EditorGUILayout.ObjectField("Icon", itemCategory.DisplayIcon, typeof(Sprite), false) as Sprite;
            serializedObject.FindProperty("DisplayIcon").objectReferenceValue = newSprite;

            GUILayout.EndHorizontal();
        
            serializedObject.ApplyModifiedProperties();
        }

    }
}
