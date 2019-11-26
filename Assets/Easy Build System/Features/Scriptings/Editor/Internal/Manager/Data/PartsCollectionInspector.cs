using EasyBuildSystem.Editor.Extensions;
using EasyBuildSystem.Runtimes;
using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Managers.Data;
using EasyBuildSystem.Runtimes.Internal.Part;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace EasyBuildSystem.Editor.Internal.Manager.Data
{
    [CustomEditor(typeof(PartsCollection))]
    public class PartsCollectionInspector : UnityEditor.Editor
    {
        #region Private Fields

        private PartsCollection Target;

        private PartBehaviour PrefabToAdd;

        private Vector2 ScrollPosition;

        private static bool Help;

        #endregion Private Fields

        #region Public Methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(10);

            #region Inspector

            GUILayout.BeginVertical("Easy Build System - Parts Collection", "window", GUILayout.Height(10));

            GUILayout.BeginVertical("box");

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.BeginHorizontal();

            GUILayout.Label("Parts Collection Settings", EditorStyles.boldLabel);

            #region Parts Collection Settings

            if (GUILayout.Button(Help ? "Hide Help" : "Show Help", GUILayout.Width(100)))
                Help = !Help;

            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            if (Help)
            {
                EditorGUILayout.HelpBox("This component allows to store all the Parts Behaviour who will be used during the runtime by the Build Manager component.\n" +
                    "Check that some elements of the list below does not contains the same ids.\n" +
                    "You can consult the documentation to find more information about this feature.", MessageType.Info);

                GUI.color = MainEditor.GetEditorColor;

                if (GUILayout.Button("Open Documentation Link"))
                    Application.OpenURL(Constants.DOCS_LINK);

                GUI.color = Color.white;
            }

            GUI.enabled = false;

            EditorGUILayout.ObjectField("Script", target, typeof(BuilderBehaviour), true);

            GUI.enabled = true;

            GUILayout.BeginVertical();

            if (Target.Parts.Count == 0)
            {
                GUILayout.BeginHorizontal("box");

                GUI.color = new Color(1.5f, 1.5f, 0f);

                GUILayout.Label("The list does not contains of part(s).");

                GUI.color = Color.white;

                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();

                GUI.color = MainEditor.GetEditorColor;

                if (GUILayout.Button("Sort Alphabetically"))
                    Target.Parts = Target.Parts.OrderBy(e => e.Name).ToList();

                if (GUILayout.Button("Sort Numerically"))
                    Target.Parts = Target.Parts.OrderBy(e => e.Id).ToList();

                GUI.color = Color.white;

                GUILayout.EndHorizontal();

                ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, false, false, GUILayout.Height(150));

                foreach (PartBehaviour Part in Target.Parts)
                {
                    if (Part == null)
                    {
                        Target.Parts.Remove(Part);

                        EditorUtility.SetDirty(target);

                        return;
                    }

                    GUILayout.BeginHorizontal("box");

                    GUILayout.BeginVertical();

                    GUILayout.Space(2);

                    GUILayout.Label("[ID:" + Part.Id + "] " + Regex.Replace(Part.Name.ToString(), "([a-z])([A-Z])", "$1 $2"));

                    GUI.color = Color.white;

                    GUILayout.Space(2);

                    GUILayout.EndVertical();

                    if (GUILayout.Button("Up", GUILayout.Width(50)))
                    {
                        try
                        {
                            ListExtension.Move<PartBehaviour>(Target.Parts, Target.Parts.IndexOf(Part), ListExtension.MoveDirection.Up);
                        }
                        catch
                        {
                        }
                    }

                    if (GUILayout.Button("Down", GUILayout.Width(50)))
                    {
                        try
                        {
                            ListExtension.Move<PartBehaviour>(Target.Parts, Target.Parts.IndexOf(Part), ListExtension.MoveDirection.Down);
                        }
                        catch
                        {
                        }
                    }

                    GUI.color = MainEditor.GetEditorColor;

                    if (GUILayout.Button("Configure Part", GUILayout.Width(100)))
                    {
                        Selection.activeGameObject = Part.gameObject;
                    }

                    GUI.color = new Color(1.5f, 0, 0);

                    if (GUILayout.Button("Remove", GUILayout.Width(80)))
                    {
                        Target.Parts.Remove(Part);

                        EditorUtility.SetDirty(target);

                        return;
                    }

                    GUI.color = Color.white;

                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(100);

                EditorGUILayout.EndScrollView();
            }

            try
            {
                GUILayout.BeginVertical("box");

                GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();

                PrefabToAdd = (PartBehaviour)EditorGUILayout.ObjectField("Base Part :", PrefabToAdd, typeof(PartBehaviour), false);

                GUILayout.EndHorizontal();

                GUI.enabled = PrefabToAdd != null;

                GUI.color = MainEditor.GetEditorColor;

                if (GUILayout.Button("Add Part At List"))
                {
                    if (PrefabToAdd == null)
                    {
                        Debug.LogError("<b><color=cyan>[Easy Build System]</color></b> : Empty field.");
                        return;
                    }

                    if (Target.Parts.Contains(PrefabToAdd) == false)
                    {
                        Target.Parts.Add(PrefabToAdd);

                        PrefabToAdd = null;

                        EditorUtility.SetDirty(target);

                        Repaint();
                    }
                    else
                        Debug.LogError("<b><color=cyan>[Easy Build System]</color></b> : This part already exists in the collection.");
                }

                GUI.color = Color.white;

                GUI.enabled = true;

                GUI.color = new Color(1.5f, 1.5f, 0);

                if (GUILayout.Button("Clear All Part(s) List"))
                {
                    if (EditorUtility.DisplayDialog("Easy Build System - Information", "Do you want remove all the part(s) from the collection ?", "Ok", "Cancel"))
                    {
                        Target.Parts.Clear();

                        Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : The collection has been clear !.");
                    }
                }

                GUI.color = Color.white;

                GUILayout.EndVertical();

                GUILayout.EndVertical();
            }
            catch { }

            GUILayout.Space(3);

            GUILayout.EndVertical();

            #endregion Parts Collection Settings

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
            Target = (PartsCollection)target;
        }

        private void OnDisable()
        {
            
        }

        #endregion Private Methods
    }
}