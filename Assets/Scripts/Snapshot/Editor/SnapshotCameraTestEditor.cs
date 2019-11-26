using UnityEngine;
using UnityEditor;
using System.Collections;

// Custom editor code based on Oliver Eberlei's "Editor Scripting for n00bs" Unite Europe 2016 talk=
// Video:   https://www.youtube.com/watch?v=9bHzTDIJX_Q
// Website: http://letscodegames.com/

// This custom editor serves to automatically call UpdatePreview() on the SnapshotCameraTest
// whenever one of the fields is changed in the inspector
[CustomEditor(typeof(SnapshotCameraTest))]
public class SnapshotCameraTestEditor : Editor
{
    SnapshotCameraTest m_Target;

    public override void OnInspectorGUI ()
    {
        m_Target = (SnapshotCameraTest)target;

        DrawDefaultInspector();
        DrawCustomInspector();
    }

    void DrawCustomInspector ()
    {
        EditorGUI.BeginChangeCheck();
        GameObject objectToSnapshot = (GameObject)EditorGUILayout.ObjectField("Object To Snapshot", m_Target.objectToSnapshot, typeof(GameObject), true);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(m_Target, "Object To Snapshot Change");
            m_Target.objectToSnapshot = objectToSnapshot;

            if (Application.isPlaying)
                m_Target.UpdatePreview();
        }

        EditorGUI.BeginChangeCheck();
        Color backgroundColor = EditorGUILayout.ColorField("Background Color", m_Target.backgroundColor);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(m_Target, "Background Color Change");
            m_Target.backgroundColor = backgroundColor;

            if (Application.isPlaying)
                m_Target.UpdatePreview();
        }

        EditorGUI.BeginChangeCheck();
        Vector3 position = EditorGUILayout.Vector3Field("Position", m_Target.position);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(m_Target, "Position Change");
            m_Target.position = position;

            if (Application.isPlaying)
                m_Target.UpdatePreview();
        }

        EditorGUI.BeginChangeCheck();
        Vector3 rotation = EditorGUILayout.Vector3Field("Rotation", m_Target.rotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(m_Target, "Rotation Change");
            m_Target.rotation = rotation;

            if (Application.isPlaying)
                m_Target.UpdatePreview();
        }

        EditorGUI.BeginChangeCheck();
        Vector3 scale = EditorGUILayout.Vector3Field("Scale", m_Target.scale);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(m_Target, "Scale Change");
            m_Target.scale = scale;

            if (Application.isPlaying)
                m_Target.UpdatePreview();
        }
    }
}
