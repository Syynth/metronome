using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.IO;
using Assets.Code.References;

namespace Assets.Code.Editors
{

    [CustomEditor(typeof(SceneLoadingZone))]
    public class SceneLoadingZoneEditor : Editor
    {

        private ReorderableList sceneList;

        public void OnEnable()
        {
            sceneList = new ReorderableList(serializedObject, serializedObject.FindProperty("Scenes"), true, true, true, true);
            sceneList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Scenes");
            sceneList.onRemoveCallback += RemoveCallback;
            sceneList.drawElementCallback += OnDrawCallback;
            sceneList.onAddCallback += AddCallback;
        }

        public void OnDisable()
        {
            
        }

        private void AddCallback(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoAddButton(list);
            serializedObject.ApplyModifiedProperties();
            var scenes = (target as SceneLoadingZone).Scenes;
            scenes[scenes.Count - 1] = null;
        }

        private void RemoveCallback(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
        }

        private void OnDrawCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var item = sceneList.serializedProperty.GetArrayElementAtIndex(index);
            var fieldRect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.ObjectField(fieldRect, item, typeof(SceneVariable));
        }

        public override void OnInspectorGUI()
        {
            sceneList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

    }

}