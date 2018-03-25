using UnityEngine;
using UnityEditor;
using Assets.Code.References;

namespace Assets.Code.Editor
{

    [CustomEditor(typeof(SceneVariable))]
    public class SceneVariableEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            var sceneVariable = target as SceneVariable;
            var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneVariable.Value);

            serializedObject.Update();

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
            serializedObject.ApplyModifiedProperties();
        }

    }

}