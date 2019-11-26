using EasyBuildSystem.Runtimes;
using EasyBuildSystem.Runtimes.Internal.Blueprint.Data;
using EasyBuildSystem.Runtimes.Internal.Group;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Internal.Storage.Data;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyBuildSystem.Editor.Internal.Blueprint
{
    [CustomEditor(typeof(BlueprintData), true)]
    public class BlueprintDataEditor : UnityEditor.Editor
    {
        #region Private Fields

        private BlueprintData Target;

        private static bool Help;

        private static bool DefaultInspector;

        #endregion Private Fields

        #region Public Methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(10);

            #region Inspector

            GUILayout.BeginVertical("Easy Build System - Blueprint Data", "window", GUILayout.Height(10));

            GUILayout.BeginVertical("box");

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.BeginHorizontal();

            GUILayout.Label("Blueprint Data Settings", EditorStyles.largeLabel);

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
                EditorGUILayout.HelpBox("This component allows to save and load of Group Behaviour who contains of Parts Behaviour during the edit/runtime.\n" +
                    "The data below can be shared with the people who use the same Parts Collection.\n" +
                    "You can consult the documentation to find more information about this feature.", MessageType.Info);

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

            if (Target.Model == null)
            {
                GUILayout.BeginHorizontal("box");

                GUI.color = new Color(1.5f, 1.5f, 0f);

                GUILayout.Label("The list does not contains of part(s).");

                GUI.color = Color.white;

                GUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("It is important to use the same Parts Collection that was used during the save of the blueprint for loading.", MessageType.Info);

                GUILayout.Label("Blueprint Data :");

                EditorGUI.BeginChangeCheck();

                Target.Data = EditorGUILayout.TextArea(Target.Data);

                if (EditorGUI.EndChangeCheck())
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
                }

                GUI.color = MainEditor.GetEditorColor;

                GUI.enabled = Target.Model != null;

                if (GUILayout.Button("Load Blueprint in Editor"))
                {
                    BuildManager Manager = FindObjectOfType<BuildManager>();

                    if (Manager == null)
                    {
                        Debug.Log("<b><color=red>[Easy Build System]</color></b> : The build manager does not exists.");

                        return;
                    }

                    List<PartModel.SerializedPart> Parts = Target.Model.DecodeToStr(Target.Data);

                    GameObject Parent = new GameObject("(Editor) Blueprint", typeof(GroupBehaviour));

                    for (int i = 0; i < Parts.Count; i++)
                    {
                        PartBehaviour Part = Manager.GetPart(Parts[i].Id);

                        PartBehaviour PlacedPart = Manager.PlacePrefab(Part, PartModel.ParseToVector3(Parts[i].Position),
                            PartModel.ParseToVector3(Parts[i].Rotation), PartModel.ParseToVector3(Parts[i].Scale), Parent.transform);

                        PlacedPart.ChangeAppearance(Parts[i].AppearanceIndex);
                    }
                }

                GUI.enabled = true;

                GUI.enabled = Application.isPlaying;

                if (GUILayout.Button("Load Blueprint in Runtime"))
                {
                    BuildManager Manager = FindObjectOfType<BuildManager>();

                    if (Manager == null)
                    {
                        Debug.Log("<b><color=red>[Easy Build System]</color></b> : The build manager does not exists.");

                        return;
                    }

                    List<PartModel.SerializedPart> Parts = Target.Model.DecodeToStr(Target.Data);

                    GameObject Parent = new GameObject("(Runtime) Blueprint", typeof(GroupBehaviour));

                    for (int i = 0; i < Parts.Count; i++)
                    {
                        PartBehaviour Part = Manager.GetPart(Parts[i].Id);

                        PartBehaviour PlacedPart = Manager.PlacePrefab(Part, PartModel.ParseToVector3(Parts[i].Position),
                            PartModel.ParseToVector3(Parts[i].Rotation), PartModel.ParseToVector3(Parts[i].Scale), Parent.transform);

                        PlacedPart.ChangeAppearance(Parts[i].AppearanceIndex);
                    }
                }

                GUI.enabled = true;

                GUI.color = Color.white;
            }

            GUILayout.EndVertical();

            #endregion Blueprint Data Settings

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
            Target = (BlueprintData)target;
        }

        #endregion Private Methods
    }
}