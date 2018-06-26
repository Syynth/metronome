using System.Linq;

using UnityEngine;
using UnityEditor;

using Assets.Code.References;

namespace Assets.Code.Editors
{

    [CustomPropertyDrawer(typeof(IntReference))]
    [CustomPropertyDrawer(typeof(FloatReference))]
    public class ReferencePropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PrefixLabel(position, label);

            var ddBtnRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y + 4, 20, EditorGUIUtility.singleLineHeight - 4);
            var fieldRect = new Rect(position.x + EditorGUIUtility.labelWidth + 20, position.y, position.width - 20 - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            var literal = property.FindPropertyRelative("literal");

            if (EditorGUI.DropdownButton(ddBtnRect, EditorGUIUtility.IconContent("pane options"), FocusType.Passive, GUIStyle.none))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Literal"), literal.boolValue, () =>
                {
                    literal.boolValue = true;
                    literal.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("Variable"), !literal.boolValue, () =>
                {
                    literal.boolValue = false;
                    literal.serializedObject.ApplyModifiedProperties();
                });
                menu.ShowAsContext();
            }

            if (literal.boolValue)
            {
                EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("m_Value"), GUIContent.none);
            }
            else
            {
                EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("m_Variable"), GUIContent.none);
            }

            EditorGUI.EndProperty();
        }

    }
}
