#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

namespace Aokoro.Editor
{
    [CustomPropertyDrawer(typeof(Aokoro.ComplexeCondition))]
    public class ComplexeConditionDrawer : PropertyDrawer
    {
        private bool opened;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            EditorGUI.BeginProperty(position, label, property);
            try
            {

                BindingFlags all = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
                //Script where object is serialized
                object targetObject = property.serializedObject.targetObject;
                Type targetType = targetObject.GetType();



                bool result = property.serializedObject.FindProperty(string.Concat(property.name, ".result")).boolValue;
                opened = EditorGUI.BeginFoldoutHeaderGroup(position, opened, result.ToString());
                if (opened)
                {
                    //Fields and properties
                    FieldInfo complexeConditionField = targetType.GetField(property.name, all);
                    ComplexeCondition complexeCondition = complexeConditionField.GetValue(targetObject) as ComplexeCondition;

                    var dictField = typeof(ComplexeCondition).GetField("parameters", all);
                    var dict = dictField.GetValue(complexeCondition) as Dictionary<string, ComplexeCondition.Parameter>;

                    EditorGUI.BeginDisabledGroup(true);
                    
                    foreach (var item in dict)
                    {
                        Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
                        rect = EditorGUI.IndentedRect(rect);
                        rect.xMax = EditorGUIUtility.currentViewWidth * .95f;
                        EditorGUI.DrawRect(rect, item.Value.Value ? Color.green * .5f : Color.red * .5f);
                        EditorGUI.Toggle(rect, item.Key, item.Value.RealValue); ;
                    }

                    EditorGUI.EndDisabledGroup();
                }
            }
            finally
            {
                EditorGUI.EndProperty();
            }
        }
    }
}
#endif