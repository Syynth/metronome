using UnityEngine;
using UnityEditor;

namespace Assets.Code.Editors
{
    [CustomEditor(typeof(GameState.GameState))]
    public class GameStateEditor : Editor
    {

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            var gameState = target as GameState.GameState;
            Texture2D texture = null;
            if (gameState.DefaultIntroScene == null)
            {
                texture = new Texture2D(width, height);
                EditorUtility.CopySerialized(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Gizmos/CustomAssetError Icon.png"), texture);
            }
            return texture;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var gameState = target as GameState.GameState;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Reset"))
            {
                gameState.Reinitialize();
            }
            if (GUILayout.Button("Load from file"))
            {
                string filePath = UnityEditor.EditorUtility.OpenFilePanel(
                    "Load save data",
                    Utils.SaveFileDirectory,
                    "json"
                );
                gameState.Load(filePath);
            }
            if (GUILayout.Button("Save to file"))
            {
                gameState.Save();
            }

            EditorGUILayout.EndHorizontal();
        }

    }
}
