using System;
using System.Linq;

using UnityEditor;
using UnityEngine;

using Assets.Code.GameState;

namespace Assets.Code.Editors
{

    [CustomEditor(typeof(Item))]
    public class ItemEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            var item = target as Item;

            var categoryGuids = AssetDatabase.FindAssets("t:ItemCategory");
            var categoryPaths = categoryGuids.Select(guid => AssetDatabase.GUIDToAssetPath(guid));
            var categoryAssets = categoryPaths.Select(path => AssetDatabase.LoadAssetAtPath<ItemCategory>(path));

            var categoryPath = AssetDatabase.GetAssetPath(item.Category);
            var categoryIndex = item.Category != null ? Array.FindIndex(categoryPaths.ToArray(), path => path == categoryPath) : 0;

            EditorGUILayout.Space();

            GUILayout.Label($"Item: {item.DisplayName}", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            GUILayout.BeginVertical(EditorStyles.helpBox);

            serializedObject.FindProperty("DisplayName").stringValue = EditorGUILayout.TextField("Display Name", item.DisplayName);

            EditorGUILayout.Space();

            serializedObject.FindProperty("Category").objectReferenceValue = categoryAssets.ToArray()[EditorGUILayout.Popup("Category", categoryIndex, categoryAssets.Select(c => c.DisplayName).ToArray())];

            EditorGUILayout.Space();

            GUILayout.Label("Flavor Text");
            serializedObject.FindProperty("Description").stringValue = EditorGUILayout.TextArea(item.Description, GUILayout.Height(70));

            GUILayout.EndVertical();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            var newSprite = EditorGUILayout.ObjectField("Item Display Icon", item.DisplayIcon, typeof(Sprite), false) as Sprite;
            serializedObject.FindProperty("DisplayIcon").objectReferenceValue = newSprite;

            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

    }
}
