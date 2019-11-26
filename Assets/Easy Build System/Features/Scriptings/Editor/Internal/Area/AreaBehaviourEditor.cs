using EasyBuildSystem.Runtimes;
using EasyBuildSystem.Runtimes.Internal.Addons;
using EasyBuildSystem.Runtimes.Internal.Area;
using UnityEditor;
using UnityEngine;

namespace EasyBuildSystem.Editor.Internal.Area
{
    [CustomEditor(typeof(AreaBehaviour))]
    public class AreaBehaviourEditor : UnityEditor.Editor
    {
        #region Public Fields

        public static Rect OffsetEditingWindow = new Rect(50, 50, 300, 280);

        #endregion Public Fields

        #region Private Fields

        private AreaBehaviour Target;

        private static bool AllIsOpen = false;

        private static bool BaseFoldout;

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

            GUILayout.BeginVertical("Easy Build System - Area Behaviour", "window", GUILayout.Height(10));

            GUILayout.BeginVertical("box");

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.BeginHorizontal();

            GUILayout.Label("Area Behaviour Settings", EditorStyles.largeLabel);

            #region Area Behaviour Settings

            GUILayout.FlexibleSpace();

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
                EditorGUILayout.HelpBox("This component allow/deny the placement or the destruction according the radius during the runtime.\n" +
                    "It can also attached as childs to avoid the close placement.", MessageType.Info);

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

            GUILayout.BeginHorizontal();

            GUILayout.Space(13);

            BaseFoldout = EditorGUILayout.Foldout(BaseFoldout, "Base Section Settings");

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Space(13);

            GUILayout.BeginVertical();

            if (BaseFoldout)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Radius"), new GUIContent("Area Radius :", "This allows to define the radius of area."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("AllowPlacement"), new GUIContent("Allow Placement In Area :", "This allows the placement in the area of part(s) from the collection below."));

                if (serializedObject.FindProperty("AllowPlacement").boolValue)
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Space(13);

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("AllowPartPlacement"), new GUIContent("Allow Part(s) In Area :", "This allows to define the part(s) that can be placed."), true);

                    GUILayout.EndHorizontal();
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("AllowDestruction"), new GUIContent("Allow Destruction In Area :", "This allows the destruction of all the part(s) in the area."));
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            #endregion Area Behaviour Settings

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Area Add-On(s) Settings", EditorStyles.largeLabel);

            #region Area Add-Ons Settings

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
                MainEditor.DrawAddons(Target, AddOnTarget.AreaBehaviour);
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            #endregion Area Add-Ons Settings

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
            Target = (AreaBehaviour)target;

            MainEditor.LoadAddons(Target, AddOnTarget.AreaBehaviour);
        }

        #endregion Private Methods
    }
}