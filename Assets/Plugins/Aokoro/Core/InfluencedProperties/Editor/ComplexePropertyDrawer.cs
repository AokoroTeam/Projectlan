#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;

using System.IO;
using UnityEngine.UIElements;

namespace Aokoro.Editor
{
    [CustomPropertyDrawer(typeof(InfluencedProperty<>))]
    public class ComplexePropertyDrawer : PropertyDrawer
    {
            /*
        private bool opened = true;

        static Color gray1 = new Color(144f / 250f, 148f / 250f, 151f / 250f);
        static Color gray2 = new Color(215f / 250f, 219f / 250f, 221f / 250f);

        static Color green1 = new Color(47f / 250f, 203f / 250f, 113f / 250f);
        static Color green2 = new Color(87f / 250f, 214f / 250f, 140f / 250f);

        static Color black1 = Color.HSVToRGB(0, 0, 0.19f);
        static Color black2 = Color.HSVToRGB(0, 0, 0.32f);

        private FieldInfo interactionField;
        private FieldInfo outputKeyProperty;

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return base.CanCacheInspectorGUI(property);
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            
            return base.CreatePropertyGUI(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            EditorGUI.BeginProperty(position, label, property);

            BindingFlags all = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            //Script where object is serialized
            object targetObject = property.serializedObject.targetObject;
            Type targetType = targetObject.GetType();

            //Fields and properties
            FieldInfo complexepropertyField = targetType.GetField(property.name, all);
            var complexeProperty = complexepropertyField.GetValue(targetObject);

            if (interactionField == null)
                interactionField = complexeProperty.GetType().GetField("interactionsDict", BindingFlags.Instance | BindingFlags.NonPublic);
            if (outputKeyProperty == null)
                outputKeyProperty = complexeProperty.GetType().GetField("outputKey", BindingFlags.Instance | BindingFlags.NonPublic);

            var interactions = interactionField.GetValue(complexeProperty);
            var outputKey = outputKeyProperty.GetValue(complexeProperty);

            if (interactions is IDictionary dic)
            {
                EditorGUI.indentLevel = 1;

                if (dic.Count > 0)
                {
                    opened = EditorGUI.BeginFoldoutHeaderGroup(position, opened, GUIContent.none);
                    if (opened)
                    {
                        DrawCells("From", "Priority", "Value", black1, black2, EditorGUIUtility.singleLineHeight * 2);
                        PropertyInfo EntryValueInfos = null;
                        PropertyInfo EntryPriorityInfos = null;
                        bool first = true;

                        foreach (DictionaryEntry kv in dic)
                        {
                            if (first)
                            {
                                Type valueType = kv.Value.GetType();
                                EntryValueInfos = valueType.GetProperty("Value");
                                EntryPriorityInfos = valueType.GetProperty("Priority");
                                first = false;
                            }

                            var from = kv.Key;
                            var priority = (int)EntryPriorityInfos.GetValue(kv.Value);
                            var value = EntryValueInfos.GetValue(kv.Value);

                            bool isPriority = outputKey == kv.Key;
                            DrawCells(from, priority.ToString(), value.ToString(), isPriority ? green1 : gray1, isPriority ? green2 : gray2, EditorGUIUtility.singleLineHeight);
                        }
                    }
                }
                else
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.LabelField("Property is empty. Returning Default");
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.Space();

                }

                EditorGUI.EndFoldoutHeaderGroup();
            }

            EditorGUI.indentLevel = 0;
            
            EditorGUI.EndProperty();
        }

        private static void DrawCells(object from, string priority, string value,
            Color color1, Color color2,
           float height)
        {
            GUIStyle ComplexePropertyStyle = UnityEditor.EditorStyles.label;
            EditorGUI.indentLevel = 2;
            EditorGUI.BeginDisabledGroup(true);

            Rect rect = EditorGUILayout.GetControlRect(false, height);
            rect = EditorGUI.IndentedRect(rect);
            rect.xMax = EditorGUIUtility.currentViewWidth * .95f;

            EditorGUI.indentLevel = 0;
            float caseWidth = rect.width / 3;

            Rect labelRect = new Rect(rect.x, rect.y, caseWidth, rect.height);
            Rect priorityRect = new Rect(rect.x + caseWidth, rect.y, caseWidth, rect.height);
            Rect valueRect = new Rect(rect.x + caseWidth * 2, rect.y, caseWidth, rect.height);

            EditorGUI.DrawRect(labelRect, color1);
            if (from is UnityEngine.Object obj)
                EditorGUI.ObjectField(labelRect, obj, obj.GetType(), true);
            else
                EditorGUI.LabelField(labelRect, from.ToString(), ComplexePropertyStyle);

            EditorGUI.DrawRect(priorityRect, color2);
            EditorGUI.LabelField(priorityRect, priority, ComplexePropertyStyle);

            EditorGUI.DrawRect(valueRect, color1);
            EditorGUI.LabelField(valueRect, value, ComplexePropertyStyle);

            EditorGUI.indentLevel = 2;
            EditorGUI.EndDisabledGroup();
        }
    */
    }
}
#endif