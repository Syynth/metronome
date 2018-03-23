using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace Assets.Code.Editor
{
    [CustomEditor(typeof(GameState.GameState))]
    public class GameStateEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Save to file"))
            {
                var gameState = target as GameState.GameState;
                gameState.Save();
            }
        }

    }
}
