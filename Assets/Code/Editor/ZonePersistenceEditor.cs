using UnityEngine;
using UnityEditor;

using System;
using Assets.Code.Zones;

namespace Assets.Code.Editors
{

    [CustomEditor(typeof(ZonePersistence))]
    public class ZonePersistenceEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Regenerate GUID"))
            {
                var zp = target as ZonePersistence;
                zp.guid = Guid.NewGuid().ToString();
            }
        }

    }

}