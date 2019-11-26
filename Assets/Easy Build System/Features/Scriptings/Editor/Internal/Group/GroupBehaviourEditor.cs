using EasyBuildSystem.Editor.Extensions;
using EasyBuildSystem.Runtimes;
using EasyBuildSystem.Runtimes.Internal.Addons;
using EasyBuildSystem.Runtimes.Internal.Area;
using EasyBuildSystem.Runtimes.Internal.Blueprint.Data;
using EasyBuildSystem.Runtimes.Internal.Group;
using UnityEditor;
using UnityEngine;

namespace EasyBuildSystem.Editor.Internal.Group
{
    [CustomEditor(typeof(GroupBehaviour))]
    public class GroupBehaviourEditor : UnityEditor.Editor
    {
        #region Public Fields

        public static Rect OffsetEditingWindow = new Rect(50, 50, 300, 280);

        #endregion Public Fields

        #region Private Fields

        private GroupBehaviour Target;

        private static bool AllIsOpen = false;

        private static bool AddonsFoldout;

        private static bool Help;

        private static bool DefaultInspector;

        #endregion Private Fields

        #region Public Methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(10);

            #region Inspector

            GUILayout.BeginVertical("Easy Build System - Group Behaviour", "window", GUILayout.Height(10));

            GUILayout.BeginVertical("box");

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.BeginHorizontal();

            GUILayout.Label("Group Behaviour Settings", EditorStyles.largeLabel);

            #region Group Behaviour Settings

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(DefaultInspector ? "Advanced Inspector" : "Default Inspector", GUILayout.Width(130)))
                DefaultInspector = !DefaultInspector;

            if (GUILayout.Button(AllIsOpen ? "Fold In" : "Fold Out", GUILayout.Width(80)))
            {
                AddonsFoldout = !AddonsFoldout;
                AllIsOpen = !AllIsOpen;
            }

            if (GUILayout.Button(Help ? "Hide Help" : "Show Help", GUILayout.Width(100)))
                Help = !Help;

            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            if (Help)
            {
                EditorGUILayout.HelpBox("This component allows to save and load of Group Behaviour who contains of Parts Behaviour during the edit/runtime.\n" +
                    "The data below can be shared and loaded if the Build Manager use the same Id's in the Parts Collection.", MessageType.Info);

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

            EditorGUILayout.ObjectField("Script", target, typeof(AreaBehaviour), true);

            GUI.enabled = true;

            GUI.color = Color.white;

            if (Application.isPlaying)
            {
                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical();

                GUI.color = MainEditor.GetEditorColor;

                if (GUILayout.Button("Save As Blueprint..."))
                {
                    BlueprintData Data = ScriptableObjectExtension.CreateAsset<BlueprintData>("New Empty Blueprint");

                    Data.name = Target.name;
                    Data.Model = Target.GetModel();
                    Data.Data = Target.GetModel().EncodeToStr();

                    Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : The blueprint model has been saved.");
                }

                if (GUILayout.Button("Destroy Group"))
                {
                    Destroy(Target.gameObject);

                    Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : The group has been destroyed.");
                }

                GUI.enabled = true;

                GUI.color = Color.white;

                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }

            #endregion Group Behaviour Settings

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Group Add-On(s) Settings", EditorStyles.largeLabel);

            #region Group Add-Ons Settings

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
                MainEditor.DrawAddons(Target, AddOnTarget.GroupBehaviour);
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            #endregion Group Add-Ons Settings

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
            Target = (GroupBehaviour)target;

            MainEditor.Addons = AddOnHelper.GetAddonsByTarget(AddOnTarget.GroupBehaviour);
        }

        #endregion Private Methods
    }
}