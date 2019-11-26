using EasyBuildSystem.Runtimes;
using EasyBuildSystem.Runtimes.Internal.Addons;
using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Managers;
using UnityEditor;
using UnityEngine;

namespace EasyBuildSystem.Editor.Internal.Manager
{
    [CustomEditor(typeof(BuildManager))]
    public class BuildManagerInspector : UnityEditor.Editor
    {
        #region Private Fields

        #region Foldout(s)

        private static bool AllIsOpen = false;

        private static bool Help;

        private static bool BaseFoldout;
        private static bool PreviewFoldout;
        private static bool AddonsFoldout;

        #endregion Foldout(s)

        private static bool DefaultInspector;

        private BuildManager Target;

        #endregion Private Fields

        #region Public Methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(10);

            #region Inspector

            GUILayout.BeginVertical("Easy Build System - Build Manager", "window", GUILayout.Height(10));

            GUILayout.BeginVertical("box");

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.BeginHorizontal();

            GUILayout.Label("Build Manager Settings", EditorStyles.largeLabel);

            #region Build Manager Settings

            if (GUILayout.Button(DefaultInspector ? "Advanced Inspector" : "Default Inspector", GUILayout.Width(130)))
                DefaultInspector = !DefaultInspector;

            if (GUILayout.Button(AllIsOpen ? "Fold In" : "Fold Out", GUILayout.Width(80)))
            {
                BaseFoldout = !BaseFoldout;
                PreviewFoldout = !PreviewFoldout;
                AddonsFoldout = !AddonsFoldout;
                AllIsOpen = !AllIsOpen;
            }

            if (GUILayout.Button(Help ? "Hide Help" : "Show Help", GUILayout.Width(100)))
                Help = !Help;

            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            if (Help)
            {
                EditorGUILayout.HelpBox("This component is fundamental to the proper functioning of the system.\n" +
                    "It allows to manage all the actions that can be performed on the scene during the runtime.\n" +
                    "This component must be only once in each scene where you want to use the system.", MessageType.Info);

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

            EditorGUILayout.ObjectField("Script", target, typeof(BuilderBehaviour), true);

            GUI.enabled = true;

            GUILayout.BeginHorizontal();

            GUILayout.Space(13);

            BaseFoldout = EditorGUILayout.Foldout(BaseFoldout, "Base Section Settings");

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Space(13);

            GUILayout.BeginVertical();

            if (BaseFoldout)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("BuildingSupport"), new GUIContent("Build Support :",
                    "This allows to define the type of support to be able to place all the parts of type (Foundation).\n\n" +
                    "(All) Allow only the placement on all the gameObject(s).\n\n" +
                    "(Terrain) Allow only the placement on Unity Terrain.\n\n" +
                    "(Voxeland) Allow only the placement on Voxeland Terrain.\n\n" +
                    "(Surface) Allow only the placement on the colliders with the component Surface Collider."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("PartsCollection"), new GUIContent("Collection To Load :", "This allows define all the parts selectable in runtime."));

                if (serializedObject.FindProperty("UsePhysics").boolValue)
                    EditorGUILayout.HelpBox("The physics will be taken into account at the startup of the scene during the runtime.\nCheck that your parts uses also the physics.", MessageType.Info);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("UsePhysics"), new GUIContent("Use Conditional Physics :", "This allows define if the system take into account the physics of parts."));
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            #endregion Build Manager Settings

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Build Manager Preview Settings", EditorStyles.largeLabel);

            #region Preview Settings

            GUI.color = Color.white;

            GUILayout.BeginHorizontal();

            GUILayout.Space(13);

            PreviewFoldout = EditorGUILayout.Foldout(PreviewFoldout, "Preview Section Settings");

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Space(13);

            GUILayout.BeginVertical();

            if (PreviewFoldout)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("UseDefaultPreviewMaterial"), new GUIContent("Use Custom Material :", "This allows to use a custom material for the preview."));

                if (serializedObject.FindProperty("UseDefaultPreviewMaterial").boolValue)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("CustomPreviewMaterial"), new GUIContent("Preview Custom Material :", "Define here the custom material."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("PreviewAllowedColor"), new GUIContent("Preview Allowed Color :", "This allows to show the allowed color when the preview can be placed."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("PreviewDeniedColor"), new GUIContent("Preview Denied Color :", "This allows to show the denied color when the preview can't be placed."));
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            #endregion Preview Settings

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Build Manager Add-Ons Settings", EditorStyles.largeLabel);

            #region Build Manager Add-Ons Settings

            GUI.color = Color.white;

            GUILayout.BeginHorizontal();

            GUILayout.Space(13);

            AddonsFoldout = EditorGUILayout.Foldout(AddonsFoldout, "Add-Ons Section Settings");

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Space(13);

            GUILayout.BeginVertical();

            if (AddonsFoldout)
            {
                MainEditor.DrawAddons(Target, AddOnTarget.BuildManager);
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            #endregion Build Manager Add-Ons Settings

            GUI.color = MainEditor.GetEditorColor;

            if (GUILayout.Button("Open Easy Build System - Main Editor")) MainEditor.Init();

            GUI.color = Color.white;

            GUILayout.EndVertical();

            GUILayout.EndVertical();

            #endregion Inspector

            serializedObject.ApplyModifiedProperties();

            GUILayout.Space(10);
        }

        #endregion Public Methods

        #region Private Fields

        private void OnEnable()
        {
            Target = (BuildManager)target;

            MainEditor.LoadAddons((BuildManager)target, AddOnTarget.BuildManager);
        }

        #endregion Private Fields
    }
}