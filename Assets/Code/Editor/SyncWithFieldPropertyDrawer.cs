using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Assets.Code.Editors
{

    [CustomPropertyDrawer(typeof(SyncWithFieldAttribute))]
    public class SyncWithFieldPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var sync = attribute as SyncWithFieldAttribute;
            var previousRef = property.objectReferenceValue;

            EditorGUI.PropertyField(position, property, label);

            var newRef = property.objectReferenceValue;
            var type = newRef?.GetType() ?? previousRef?.GetType();

            if (type == null) return;

            var fieldIsList = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);

            if (previousRef != newRef && newRef != null)
            {
                var field = type.GetField(sync.Field);
                field.SetValue(newRef, property.serializedObject.targetObject);
                if (previousRef != null)
                {
                    field.SetValue(previousRef, null);
                }
            }
            else if (newRef == null)
            {
                type.GetField(sync.Field).SetValue(previousRef, null);
            }

        }

    }
}
