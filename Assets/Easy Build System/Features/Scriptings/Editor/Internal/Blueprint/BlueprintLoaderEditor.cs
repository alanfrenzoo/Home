using EasyBuildSystem.Runtimes;
using EasyBuildSystem.Runtimes.Internal.Blueprint;
using EasyBuildSystem.Runtimes.Internal.Blueprint.Data;
using UnityEditor;
using UnityEngine;

namespace EasyBuildSystem.Editor.Internal.Blueprint
{
    [CustomEditor(typeof(BlueprintLoader), true)]
    public class BlueprintLoaderEditor : UnityEditor.Editor
    {
        #region Private Fields

        private static bool Help;

        private static bool DefaultInspector;

        #endregion Private Fields

        #region Public Methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(10);

            #region Inspector

            GUILayout.BeginVertical("Easy Build System - Blueprint Loader", "window", GUILayout.Height(10));

            GUILayout.BeginVertical("box");

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.BeginHorizontal();

            GUILayout.Label("Blueprint Loader Settings", EditorStyles.largeLabel);

            #region Blueprint Data Settings

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(DefaultInspector ? "Advanced Inspector" : "Default Inspector", GUILayout.Width(130)))
                DefaultInspector = !DefaultInspector;

            if (GUILayout.Button(Help ? "Hide Help" : "Show Help", GUILayout.Width(100)))
                Help = !Help;

            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            if (Help)
            {
                EditorGUILayout.HelpBox("This component allows to load a blueprint data on scene startup.", MessageType.Info);

                GUI.color = MainEditor.GetEditorColor;

                if (GUILayout.Button("Open Documentation Link"))
                    Application.OpenURL(Constants.DOCS_LINK);

                GUI.color = Color.white;
            }

            if (DefaultInspector)
            {
                DrawDefaultInspector();

                GUILayout.EndVertical();

                GUILayout.EndVertical();

                serializedObject.ApplyModifiedProperties();

                GUILayout.Space(10);

                return;
            }

            GUI.enabled = false;

            EditorGUILayout.ObjectField("Script", target, typeof(BlueprintData), true);

            GUI.enabled = true;

            GUI.color = Color.white;

            GUILayout.BeginVertical();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Blueprint"), new GUIContent("Blueprint Data :", "This allows to define the blueprint data has load on scene startup."));

            GUILayout.EndVertical();

            #endregion Blueprint Data Settings

            GUILayout.EndVertical();

            GUILayout.EndVertical();

            #endregion Inspector

            serializedObject.ApplyModifiedProperties();

            GUILayout.Space(10);
        }

        #endregion Public Methods
    }
}