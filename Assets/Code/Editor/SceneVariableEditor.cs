using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Assets.Code.References;

namespace Assets.Code.Editors
{

    [CustomEditor(typeof(SceneVariable))]
    public class SceneVariableEditor : Editor
    {

        private ReorderableList sceneList;

        public void OnEnable()
        {
            sceneList = new ReorderableList(serializedObject, serializedObject.FindProperty("ConnectedScenes"), true, true, true, true);
            sceneList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Connected Scenes");
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
        }

        private void RemoveCallback(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
        }

        private void OnDrawCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var item = sceneList.serializedProperty.GetArrayElementAtIndex(index);
            var fieldRect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);
            var oldSceneVariable = item.objectReferenceValue as SceneVariable;
            var targetVar = target as SceneVariable;
            var zone = targetVar.LoadingZone;

            var newSceneVariable = EditorGUI.ObjectField(fieldRect, "Scene #" + (index + 1).ToString(), oldSceneVariable, typeof(SceneVariable), false) as SceneVariable;

            newSceneVariable?.SetLoadingZone(zone);

            if (newSceneVariable == null || targetVar.ConnectedScenes.Find(scene => scene != null && newSceneVariable.Value == scene.Value) == null)
            {
                item.objectReferenceValue = newSceneVariable;
            }

            serializedObject.ApplyModifiedProperties();
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            var scene = target as SceneVariable;
            Texture2D texture = null;
            if (scene.LoadingZone == null)
            {
                texture = new Texture2D(width, height);
                EditorUtility.CopySerialized(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Gizmos/CustomAssetWarning Icon.png"), texture);
            }
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.Value) == null)
            {
                texture = new Texture2D(width, height);
                EditorUtility.CopySerialized(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Gizmos/CustomAssetError Icon.png"), texture);
            }
            return texture;
        }

        public override void OnInspectorGUI()
        {
            var sceneVariable = target as SceneVariable;
            var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneVariable.Value);
            var oldZone = sceneVariable.LoadingZone;

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Scene reference data", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            var newScene = EditorGUILayout.ObjectField("Scene", oldScene, typeof(SceneAsset), false) as SceneAsset;

            if (EditorGUI.EndChangeCheck())
            {
                var newPath = AssetDatabase.GetAssetPath(newScene);
                var scenePathProperty = serializedObject.FindProperty("Value");
                scenePathProperty.stringValue = newPath;

            }
            else
            {
                if (oldScene == null)
                {
                    EditorGUILayout.HelpBox("This scene variable is invalid.", MessageType.Error);
                }
            }

            EditorGUI.BeginChangeCheck();

            var newZone = EditorGUILayout.ObjectField("Loading Zone", oldZone, typeof(SceneLoadingZone), false) as SceneLoadingZone;
            if (EditorGUI.EndChangeCheck())
            {
                var zoneProperty = serializedObject.FindProperty("LoadingZone");
                zoneProperty.objectReferenceValue = newZone;
                oldZone?.RemoveScene(sceneVariable);
                newZone?.AddScene(sceneVariable);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            sceneList.DoLayoutList();


            serializedObject.ApplyModifiedProperties();
        }

    }

}