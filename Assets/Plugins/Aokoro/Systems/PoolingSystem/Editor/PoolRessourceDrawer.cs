using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Aokoro.Pooling.Editor
{
    [CustomPropertyDrawer(typeof(PoolResource), true)]
    public class PoolRessourceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = indent + 1;
            EditorGUILayout.BeginHorizontal();

            SerializedProperty resourceProperty = property.FindPropertyRelative("resourceType");
            EditorGUILayout.PropertyField(resourceProperty, GUIContent.none);

            switch (resourceProperty.enumValueIndex)
            {
                case (int)PoolResourceType.Reference:
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("sResource"), GUIContent.none);
                    break;
                case (int)PoolResourceType.ResourcePath:
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("sPath"), GUIContent.none);
                    break;
            };
            EditorGUI.indentLevel = indent;

            EditorGUILayout.EndHorizontal();
            EditorGUI.EndProperty();
        }
    }
}
