using EasyBuildSystem.Runtimes;
using EasyBuildSystem.Runtimes.Internal.Addons;
using EasyBuildSystem.Runtimes.Internal.Builder;
using UnityEditor;
using UnityEngine;

namespace EasyBuildSystem.Editor.Internal.Builder
{
    [CustomEditor(typeof(BuilderBehaviour), true)]
    public class BuilderBehaviourEditor : UnityEditor.Editor
    {
        #region Private Fields

        private BuilderBehaviour Target;

        #region Foldout(s)

        private static bool AllIsOpen = false;

        private static bool BaseFoldout;

        private static bool ModesFoldout;

        private static bool PreviewFoldout;

        private static bool InputsFoldout;

        private static bool AudioFoldout;

        private static bool AddonsFoldout;

        private static bool Help;

        #endregion Foldout(s)

        private static bool DefaultInspector;

        #endregion Private Fields

        #region Public Methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(10);

            #region Inspector

            GUILayout.BeginVertical("Easy Build System - Builder Behaviour", "window", GUILayout.Height(10));

            GUILayout.BeginVertical("box");

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.BeginHorizontal();

            GUILayout.Label("Builder Behaviour Settings", EditorStyles.boldLabel);

            if (GUILayout.Button(DefaultInspector ? "Advanced Inspector" : "Default Inspector", GUILayout.Width(130)))
                DefaultInspector = !DefaultInspector;

            if (GUILayout.Button(AllIsOpen ? "Fold In" : "Fold Out", GUILayout.Width(80)))
            {
                BaseFoldout = !BaseFoldout;
                ModesFoldout = !ModesFoldout;
                PreviewFoldout = !PreviewFoldout;
                InputsFoldout = !InputsFoldout;
                AudioFoldout = !AudioFoldout;
                AddonsFoldout = !AddonsFoldout;
                AllIsOpen = !AllIsOpen;
            }

            if (GUILayout.Button(Help ? "Hide Help" : "Show Help", GUILayout.Width(100)))
                Help = !Help;

            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            if (Help)
            {
                EditorGUILayout.HelpBox("This component contains all the actions behaviours whose can be performed by the player during the runtime.\n" +
                    "It require the Build Manager prefab in the scene with an Parts Collection, otherwise it will be useless.\n" +
                    "Note : Also check that this component is only present once in your scene, otherwise it can cause problems.", MessageType.Info);

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

            EditorGUILayout.HelpBox("This class must be used to derive by API, you can use the components already provided by the system (Desktop or Android).\n" +
                "Note : The switch of behaviour, will not save your Builder Behaviour settings.", MessageType.Warning);

            GUI.color = MainEditor.GetEditorColor;

            if (GUILayout.Button("Switch Builder Behaviour For Desktop")) {
                Target.gameObject.AddComponent<DesktopBuilderBehaviour>();
                MonoBehaviour.DestroyImmediate(Target);
                return;
            }

            if (GUILayout.Button("Switch Builder Behaviour For Android")) {
                Target.gameObject.AddComponent<AndroidBuilderBehaviour>();
                MonoBehaviour.DestroyImmediate(Target);
                return;
            }

            GUI.color = Color.white;

            GUILayout.EndVertical();

            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

            GUILayout.Space(10);

            #endregion
        }

        #endregion Public Methods

        #region Private Methods

        private void OnEnable()
        {
            Target = (BuilderBehaviour)target;

            MainEditor.LoadAddons(Target, AddOnTarget.BuilderBehaviour);
        }

        #endregion Private Methods
    }
}