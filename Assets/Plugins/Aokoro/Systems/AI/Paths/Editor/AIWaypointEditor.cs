#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Aokoro.AI.Paths.Editor
{
    [CustomEditor(typeof(AIWaypoint), true)]
    [CanEditMultipleObjects]
    public class AIWaypointEditor : UnityEditor.Editor
    {

        BoxBoundsHandle m_BoundsHandle = new BoxBoundsHandle()
        {
            axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Z,
            midpointHandleSizeFunction = new Handles.SizeFunction(p => .1f)
        };


        public override void OnInspectorGUI()
        {

            serializedObject.Update();
            AIWaypoint waypoint = (AIWaypoint)target;

            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("behavior"));
            EditorGUILayout.IntSlider(serializedObject.FindProperty("probability"), 0, 100);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("layer"));

            if (targets.Length == 1)
            {
                Bounds zone = waypoint._zone;
                Vector3 currentSize = zone.size;
                SerializedProperty boundProp = serializedObject.FindProperty("_zone");

                Vector2 size = EditorGUILayout.Vector2Field("Zone size", new Vector2(currentSize.x, currentSize.z));
                boundProp.boundsValue = new Bounds(Vector3.zero, new Vector3(size.x, 0, size.y));

                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Activity", EditorStyles.boldLabel);

                serializedObject.ApplyModifiedProperties();
            }
        }
        
        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        public static void DrawGizmos(AIWaypoint waypoint, GizmoType gizmoType)
        {
            const float thickness = .3f;

            Bounds zone = waypoint.GetWorldZone();
            Vector3 center = zone.center + Vector3.up * .05f;
            Handles.color = Color.cyan;
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

            Handles.DrawLine(waypoint.transform.position, center, thickness * 3);
            Handles.DrawWireDisc(center, Vector3.up, .3f, thickness);

            if (gizmoType.HasFlag(GizmoType.NonSelected))
            {
                Handles.color = Color.cyan;
                Handles.DrawWireCube(center, zone.size);
            }
        }

        public void OnSceneGUI()
        {
            AIWaypoint waypoint = (AIWaypoint)target;

            Bounds zone = waypoint.GetWorldZone();
            m_BoundsHandle.center = zone.center;
            m_BoundsHandle.size = zone.size;

            EditorGUI.BeginChangeCheck();
            m_BoundsHandle.DrawHandle();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(waypoint, "Modified waypoint volume");
                waypoint.transform.position = new Vector3(m_BoundsHandle.center.x, waypoint.transform.position.y, m_BoundsHandle.center.z);
                waypoint._zone = new Bounds(Vector3.zero, m_BoundsHandle.size);
                EditorUtility.SetDirty(target);
            }
        }
    }
}
#endif
