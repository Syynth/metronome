using UnityEngine;
using UnityEditor;

using System;

namespace Assets.Code.Editors
{

    [CustomEditor(typeof(Spawn))]
    public class SpawnEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Regenerate GUID"))
            {
                var zp = target as Spawn;
                zp.guid = Guid.NewGuid().ToString();
            }
        }

    }

}