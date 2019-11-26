using EasyBuildSystem.Runtimes;
using EasyBuildSystem.Runtimes.Internal.Addons;
using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Storage;
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EasyBuildSystem.Editor.Internal.Storage
{
    [CustomEditor(typeof(BuildStorage))]
    public class BuildStorageInspector : UnityEditor.Editor
    {
        #region Private Fields

        private BuildStorage Target;

        private string LoadPath;

        #region Foldout(s)

        private static bool AllIsOpen = false;

        private static bool BaseFoldout;

        private static bool AddonsFoldout;

        private static bool Help;

        private static bool DefaultInspector;

        #endregion Foldout(s)

        #endregion Private Fields

        #region Public Methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(10);

            #region Inspector

            GUILayout.BeginVertical("Easy Build System - Build Storage", "window", GUILayout.Height(10));

            GUILayout.BeginVertical("box");

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.BeginHorizontal();

            GUILayout.Label("Build Storage Settings", EditorStyles.largeLabel);

            #region Build Storage Settings

            if (GUILayout.Button(DefaultInspector ? "Advanced Inspector" : "Default Inspector", GUILayout.Width(130)))
                DefaultInspector = !DefaultInspector;

            if (GUILayout.Button(AllIsOpen ? "Fold In" : "Fold Out", GUILayout.Width(80)))
            {
                BaseFoldout = !BaseFoldout;
                AddonsFoldout = !AddonsFoldout;
                AllIsOpen = !AllIsOpen;
            }

            if (GUILayout.Button(Help ? "Hide Help" : "Show Help", GUILayout.Width(100)))
                Help = !Help;

            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            if (Help)
            {
                EditorGUILayout.HelpBox("This component allows to save and load all the Parts Behaviour of the scene before/after the runtime.\n" +
                    "Note : The larger the number of parts saved in the file, please select Json in the serializer type field.", MessageType.Info);

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
                EditorGUILayout.PropertyField(serializedObject.FindProperty("StorageType"), new GUIContent("Storage Type :", "This allows to save/load for Desktop or Android."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("StorageSerializer"), new GUIContent("Storage Serializer Type :", "This allows to define the serializer type."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("AutoSave"), new GUIContent("Use Auto Save :", "This allows to enable auto save."));

                if (serializedObject.FindProperty("AutoSave").boolValue)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("AutoSaveInterval"), new GUIContent("Auto Save Interval (ms) :", "This allows to define the auto save interval."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("SavePrefabs"), new GUIContent("Save All Part Behaviour :", "This allows to save all the prefabs after have exited the scene."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("LoadPrefabs"), new GUIContent("Load All Part Behaviour :", "This allows to save all the prefabs at startup of the scene."));

                if (serializedObject.FindProperty("StorageType").enumValueIndex == 0)
                {
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.HelpBox("Define here the complete path with the name & extension.\n" +
                        @"Example For Windows : C:\Users\My Dekstop\Desktop\MyFile.dat", MessageType.Info);

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("StorageOutputFile"), new GUIContent("Storage Output Path :", "Output path to save and load the file."));

                    EditorGUI.EndChangeCheck();

                    if (GUI.changed)
                        EditorUtility.SetDirty(target);
                }

                GUI.color = MainEditor.GetEditorColor;

                if (GUILayout.Button("Load Storage File In Editor Scene ..."))
                {
                    if (EditorUtility.DisplayDialog("Easy Build System - Information", "(Only Large File) Note :\nYour scene will be saved, to avoid the loss of data in case of a crash of the editor.", "Load", "Cancel"))
                    {
                        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

                        LoadPath = EditorUtility.OpenFilePanel("Load Ebs Storage File :", "", "*.*");

                        if (LoadPath != string.Empty)
                        {
                            Target.LoadInEditor(LoadPath);
                        }
                    }
                }

                GUILayout.EndVertical();

                GUILayout.BeginVertical("box");
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            #endregion Build Storage Settings

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Build Storage Add-Ons Settings", EditorStyles.largeLabel);

            #region Socket Add-Ons Settings

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
                MainEditor.DrawAddons(Target, AddOnTarget.StorageBehaviour);
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            #endregion Socket Add-Ons Settings


            GUILayout.EndVertical();

            GUILayout.EndVertical();

            #endregion Inspector

            serializedObject.ApplyModifiedProperties();

            GUILayout.Space(10);
        }

        #endregion Public Methods

        #region Private Methods

        private void OnEnable()
        {
            Target = (BuildStorage)target;

            MainEditor.Addons = AddOnHelper.GetAddonsByTarget(AddOnTarget.StorageBehaviour);
        }

        #endregion Private Methods
    }
}