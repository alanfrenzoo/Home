using EasyBuildSystem.Runtimes;
using EasyBuildSystem.Runtimes.Extensions;
using EasyBuildSystem.Runtimes.Internal.Addons;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Internal.Socket;
using EasyBuildSystem.Runtimes.Internal.Socket.Data;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EasyBuildSystem.Editor.Internal.Socket
{
    [CustomEditor(typeof(SocketBehaviour))]
    public class SocketBehaviourEditor : UnityEditor.Editor
    {
        #region Public Enums

        public enum EditorHandleType
        {
            None,
            Position,
            Rotation,
            Scale
        }

        #endregion Public Enums

        #region Public Fields

        public static Rect OffsetEditingWindow = new Rect(50, 50, 300, 258);

        #endregion Public Fields

        #region Private Fields

        private SocketBehaviour Target;

        private PartBehaviour PrefabField;

        private bool ShowPartOffsetEditor;

        private int CurrentOffsetIndex;

        private EditorHandleType CurrentHandle = EditorHandleType.Position;

        private PartOffset CurrentOffset;

        private GameObject PreviewPart;

        #region Foldout(s)

        private static bool AllIsOpen = false;

        private static bool BaseFoldout;

        private static bool OffsetsFoldout;

        private static bool UtilitiesFoldout;

        private static bool AddonsFoldout;

        private static bool Help;

        private static bool DefaultInspector;

        #endregion Foldout(s)

        #endregion Private Fields

        #region Public Methods

        public void OnSceneGUI()
        {
            if (SceneView.lastActiveSceneView.camera == null)
                return;

            if (ShowPartOffsetEditor)
            {
                if (CurrentOffset != null)
                {
                    if (PreviewPart == null)
                    {
                        CreatePreview(CurrentOffset);
                    }
                    else
                    {
                        EditorGUI.BeginChangeCheck();

                        Vector3 RelativePos = Vector3.zero;

                        Quaternion RelativeRot = Quaternion.identity;

                        Vector3 RelativeScale = Vector3.one;

                        if (CurrentHandle == EditorHandleType.Position)
                            RelativePos = Handles.PositionHandle(Target.transform.position + CurrentOffset.Position, Quaternion.identity);
                        else if (CurrentHandle == EditorHandleType.Rotation)
                            RelativeRot = Handles.RotationHandle(Quaternion.Euler(CurrentOffset.Rotation), Target.transform.position + CurrentOffset.Position);
                        else if (CurrentHandle == EditorHandleType.Scale)
                            RelativeScale = Handles.ScaleHandle(CurrentOffset.Scale, Target.transform.position + CurrentOffset.Position, Quaternion.identity, HandleUtility.GetHandleSize(Target.transform.position + CurrentOffset.Position));

                        if (EditorGUI.EndChangeCheck())
                        {
                            if (CurrentHandle == EditorHandleType.Position)
                                CurrentOffset.Position = RelativePos - Target.transform.position;
                            else if (CurrentHandle == EditorHandleType.Rotation)
                                CurrentOffset.Rotation = RelativeRot.eulerAngles;
                            else if (CurrentHandle == EditorHandleType.Scale)
                                CurrentOffset.Scale = RelativeScale;
                        }

                        if (CurrentOffset.UseCustomScale)
                            PreviewPart.transform.localScale = CurrentOffset.Scale;
                    }
                }

                Handles.BeginGUI();

                OffsetEditingWindow = GUI.Window(1, OffsetEditingWindow, OnOffsetEditingWindow, "Easy Build System - Offset Part Editing");

                Handles.EndGUI();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(10);

            #region Inspector

            GUILayout.BeginVertical("Easy Build System - Socket Behaviour", "window", GUILayout.MaxHeight(10));

            GUILayout.BeginVertical("box");

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.BeginHorizontal();

            GUILayout.Label("Socket Behaviour Settings", EditorStyles.largeLabel);

            #region Socket Behaviour Settings

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(DefaultInspector ? "Advanced Inspector" : "Default Inspector", GUILayout.Width(130)))
                DefaultInspector = !DefaultInspector;

            if (GUILayout.Button(AllIsOpen ? "Fold In" : "Fold Out", GUILayout.Width(80)))
            {
                BaseFoldout = !BaseFoldout;
                OffsetsFoldout = !OffsetsFoldout;
                AddonsFoldout = !AddonsFoldout;
                AllIsOpen = !AllIsOpen;
            }

            if (GUILayout.Button(Help ? "Hide Help" : "Show Help", GUILayout.Width(100)))
                Help = !Help;

            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            if (Help)
            {
                EditorGUILayout.HelpBox("This component allows to snap with the Builder Behaviour the Parts Behaviour according the offset settings during the runtime.\n" +
                    "Sockets can cause problems if they are misconfigured, If it disable off unnaturally, please change their offset or radius.", MessageType.Info);

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

            EditorGUILayout.ObjectField("Script", target, typeof(SocketBehaviour), true);

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
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Type"), new GUIContent("Socket Type :", "This allows to define the socket type."));

                if ((SocketType)serializedObject.FindProperty("Type").enumValueIndex == SocketType.Socket)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Radius"), new GUIContent("Socket Radius :", "This allows to define the socket radius point.\nYou can decrease the socket radius to improve the precision during the detection."));
                else
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("AttachmentBounds"), new GUIContent("Socket Attachment Bounds :", "This allows to define the attachment bounds."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("DisableOnGroundContact"), new GUIContent("Disable On Terrain/Surface Contact :", "This allows to define if the socket will be disable if she's entering in collision with the terrain or the surface."));
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            #endregion Socket Behaviour Settings

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Socket Offset(s) Settings", EditorStyles.largeLabel);

            #region Socket Offsets Settings

            GUI.color = Color.white;

            GUILayout.BeginHorizontal();

            GUILayout.Space(13);

            OffsetsFoldout = EditorGUILayout.Foldout(OffsetsFoldout, "Offsets Section Settings");

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Space(13);

            GUILayout.BeginVertical();

            if (OffsetsFoldout)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Space(13);

                GUILayout.BeginVertical();

                if (Target.PartOffsets.Count == 0)
                {
                    GUILayout.BeginHorizontal("box");

                    GUI.color = new Color(1.5f, 1.5f, 0f);

                    GUILayout.Label("The list does not contains of offset part(s), No part(s) will may be snap on it.");

                    GUI.color = Color.white;

                    GUILayout.EndHorizontal();
                }
                else
                {
                    int Index = 0;

                    foreach (PartOffset Offset in Target.PartOffsets)
                    {
                        if (Offset == null || Offset.Part == null)
                        {
                            Target.PartOffsets.Remove(Offset);

                            EditorUtility.SetDirty(target);

                            return;
                        }

                        GUILayout.BeginHorizontal("box");

                        GUILayout.BeginVertical();

                        GUILayout.BeginHorizontal();

                        GUILayout.Label("(ID:" + Offset.Part.Id + ") " + Offset.Part.Name, EditorStyles.largeLabel);

                        if (PreviewPart != null && int.Parse(PreviewPart.name) == Offset.Part.Id)
                        {
                            GUI.color = new Color(1.5f, 1.5f, 0f);

                            if (GUILayout.Button("Cancel Preview", GUILayout.Width(100)))
                                ClearPreview();
                        }
                        else
                        {
                            GUI.color = MainEditor.GetEditorColor;

                            if (GUILayout.Button("Preview Part", GUILayout.Width(100)))
                            {
                                ClearPreview();

                                CurrentOffset = Offset;

                                CreatePreview(Offset);
                            }
                        }

                        GUI.color = Color.white;

                        GUI.color = new Color(1.5f, 0, 0);

                        if (GUILayout.Button("Remove", GUILayout.Width(80)))
                        {
                            Target.PartOffsets.Remove(Offset);

                            ClearPreview();

                            return;
                        }

                        GUI.color = Color.white;

                        GUILayout.EndHorizontal();

                        GUILayout.Space(5);

                        EditorGUI.BeginChangeCheck();

                        Offset.Position = EditorGUILayout.Vector3Field(new GUIContent("Offset Position : ", "This allows to set the position of part that will snapped on this socket."), Offset.Position);
                        Offset.Rotation = EditorGUILayout.Vector3Field(new GUIContent("Offset Rotation : ", "This allows to set the rotation of part that will snapped on this socket."), Offset.Rotation);
                        Offset.UseCustomScale = EditorGUILayout.Toggle(new GUIContent("Offset Use Custom Scale : ", "This allows to define whether the part to be snap to this socket will have a specific scale, otherwise it will be the scale only of the part."), Offset.UseCustomScale);

                        if (Offset.UseCustomScale)
                            Offset.Scale = EditorGUILayout.Vector3Field(new GUIContent("Offset Scale : ", "This allows to define the specific scale of the part that will be snapped on this socket."), Offset.Scale);

                        if (EditorGUI.EndChangeCheck())
                        {
                            if (PreviewPart != null)
                            {
                                if (PreviewPart.name == Offset.Part.Id.ToString())
                                {
                                    PreviewPart.transform.position = Target.transform.TransformPoint(Offset.Position);
                                    PreviewPart.transform.rotation = Quaternion.Euler(Offset.Rotation);
                                    PreviewPart.transform.localScale = Offset.UseCustomScale ? Offset.Scale : (Target.transform.parent != null ? Target.transform.parent.localScale : Target.transform.localScale);
                                }

                                EditorUtility.SetDirty(target);
                            }
                        }

                        GUI.color = MainEditor.GetEditorColor;

                        if (!ShowPartOffsetEditor || CurrentOffset != Offset)
                        {
                            if (GUILayout.Button("Edit Offset Part"))
                            {
                                SceneHelper.Focus(target, DrawCameraMode.Textured);

                                if (PreviewPart != null)
                                    DestroyImmediate(PreviewPart);

                                CurrentOffsetIndex = Index;

                                CurrentOffset = Offset;

                                ShowPartOffsetEditor = true;
                            }
                        }
                        else if (CurrentOffset == Offset)
                        {
                            GUI.color = Color.white;

                            EditorGUILayout.HelpBox("The position, rotation as well as the scale of the current part displayed, will be exactly the same in runtime.", MessageType.Info);

                            GUI.color = new Color(0f, 1.5f, 1.5f);

                            if (GUILayout.Button("Close"))
                            {
                                SceneHelper.UnFocus();

                                ShowPartOffsetEditor = false;

                                Target.PartOffsets[CurrentOffsetIndex] = CurrentOffset;

                                EditorUtility.SetDirty(target);

                                Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : The offsets changes were saved.");
                            }
                        }

                        GUI.color = Color.white;

                        GUILayout.Space(2);

                        GUILayout.EndVertical();

                        GUILayout.EndHorizontal();

                        Index++;
                    }
                }

                try
                {
                    GUILayout.BeginVertical("box");

                    GUILayout.BeginHorizontal();

                    PrefabField = (PartBehaviour)EditorGUILayout.ObjectField(new GUIContent("Base Part :", "Part at add to the list for the snap according to the offset on this socket in runtime."), PrefabField, typeof(PartBehaviour), false);

                    GUILayout.EndHorizontal();

                    GUILayout.BeginVertical();

                    GUI.enabled = PrefabField != null;

                    GUI.color = MainEditor.GetEditorColor;

                    if (GUILayout.Button("Add New Offset Part"))
                    {
                        if (PrefabField == null)
                        {
                            Debug.LogError("<b><color=cyan>[Easy Build System]</color></b> : Empty field.");
                            return;
                        }

                        if (Target.PartOffsets.Find(entry => entry.Part.Name == PrefabField.Name) == null)
                        {
                            ClearPreview();

                            PartOffset Offset = new PartOffset(PrefabField);

                            Target.PartOffsets.Add(Offset);

                            CurrentOffset = Offset;

                            CreatePreview(Offset);

                            PrefabField = null;

                            Repaint();
                        }
                        else
                            Debug.LogError("<b><color=cyan>[Easy Build System]</color></b> : This part already exists in the collection.");
                    }

                    GUI.color = Color.white;

                    GUI.enabled = true;

                    GUI.color = new Color(1.5f, 1.5f, 0f);

                    if (GUILayout.Button("Clear All Offset Part(s) List"))
                    {
                        if (EditorUtility.DisplayDialog("Easy Build System - Information", "Do you want remove all the offset part(s) from the collection ?", "Ok", "Cancel"))
                        {
                            Target.PartOffsets.Clear();

                            Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : The collection has been clear !.");
                        }
                    }

                    GUI.color = Color.white;

                    GUILayout.EndVertical();

                    GUILayout.Space(3);

                    GUILayout.EndVertical();
                }
                catch { }

                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            #endregion Socket Offsets Settings

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Socket Utilities Settings", EditorStyles.largeLabel);

            #region Socket Utilities Settings

            GUI.color = Color.white;

            GUILayout.BeginHorizontal();

            GUILayout.Space(13);

            UtilitiesFoldout = EditorGUILayout.Foldout(UtilitiesFoldout, "Utilities Section Settings");

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Space(13);

            GUILayout.BeginVertical();

            if (UtilitiesFoldout)
            {
                GUI.color = MainEditor.GetEditorColor;

                if (GUILayout.Button("Duplicate Socket (Inverse)"))
                {
                    GameObject Duplicate = Instantiate(Target.gameObject, Target.transform.parent);

                    Duplicate.name = Target.transform.name;

                    Vector3 Inverse = new Vector3(-Target.transform.localPosition.x, Target.transform.localPosition.y, -Target.transform.localPosition.z);

                    Duplicate.transform.localPosition = Inverse;

                    Selection.activeGameObject = Duplicate;
                }

                GUI.color = Color.white;
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            #endregion Socket Utilities Settings

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Socket Add-Ons Settings", EditorStyles.largeLabel);

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
                MainEditor.DrawAddons(Target, AddOnTarget.SocketBehaviour);
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            #endregion Socket Add-Ons Settings

            GUILayout.EndVertical();

            GUILayout.EndVertical();

            #endregion Inspector

            GUILayout.Space(10);

            serializedObject.ApplyModifiedProperties();

            if (CurrentOffset != null)
            {
                if (PreviewPart != null)
                {
                    PreviewPart.transform.position = Target.transform.TransformPoint(CurrentOffset.Position);
                    PreviewPart.transform.rotation = Target.transform.rotation * Quaternion.Euler(CurrentOffset.Rotation);
                }
            }

            if (GUI.changed)
                Target.PartOffsets = Target.PartOffsets.ToList().OrderBy(item => item.Part.Id).ToList();
        }

        #endregion Public Methods

        #region Private Methods

        private void OnEnable()
        {
            Target = (SocketBehaviour)target;

            MainEditor.LoadAddons(Target, AddOnTarget.SocketBehaviour);

            Target.PartOffsets = Target.PartOffsets.ToList().OrderBy(item => item.Part.Id).ToList();
        }

        private void OnDisable()
        {
            SceneHelper.UnFocus();

            ClearPreview();
        }

        private void OnOffsetEditingWindow(int windowId)
        {
            GUILayout.BeginVertical("box");

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Part Offset(s) Settings", EditorStyles.largeLabel);

            GUI.color = Color.white;

            EditorGUILayout.LabelField("Offset Part (" + CurrentOffset.Part.name + ")");

            CurrentOffset.Position = EditorGUILayout.Vector3Field("Part Position :", CurrentOffset.Position);

            GUILayout.BeginHorizontal();

            if (CurrentHandle != EditorHandleType.Position)
                GUI.color = new Color(0f, 1.5f, 0f, 1f);
            else
                GUI.color = new Color(0f, 1.5f, 0f, 0.5f);

            if (GUILayout.Button("Show Position Handle"))
                ChangeHandle(EditorHandleType.Position);

            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            CurrentOffset.Rotation = EditorGUILayout.Vector3Field("Part Rotation :", CurrentOffset.Rotation);

            GUILayout.BeginHorizontal();

            if (CurrentHandle != EditorHandleType.Rotation)
                GUI.color = new Color(0f, 1.5f, 0f, 1f);
            else
                GUI.color = new Color(0f, 1.5f, 0f, 0.5f);

            if (GUILayout.Button("Show Rotation Handle"))
                ChangeHandle(EditorHandleType.Rotation);

            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            CurrentOffset.Scale = EditorGUILayout.Vector3Field("Part Scale :", CurrentOffset.Scale);

            GUILayout.BeginHorizontal();

            if (CurrentHandle != EditorHandleType.Scale)
                GUI.color = new Color(0f, 1.5f, 0f, 1f);
            else
                GUI.color = new Color(0f, 1.5f, 0f, 0.5f);

            if (GUILayout.Button("Show Scale Handle"))
                ChangeHandle(EditorHandleType.Scale);

            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            GUI.color = MainEditor.GetEditorColor;

            if (GUILayout.Button("Close"))
            {
                ShowPartOffsetEditor = false;

                SceneHelper.UnFocus();

                Repaint();

                Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : The offsets changes were saved.");
            }

            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
        }

        private void CreatePreview(PartOffset offsetPart)
        {
            if (Application.isPlaying)
                return;

            if (PreviewPart == null)
            {
                PreviewPart = Instantiate(offsetPart.Part.gameObject, Target.transform);

                PreviewPart.transform.position = Target.transform.TransformPoint(offsetPart.Position);

                PreviewPart.transform.rotation = Target.transform.rotation * Quaternion.Euler(offsetPart.Rotation);

                if (offsetPart.UseCustomScale)
                    PreviewPart.transform.localScale = offsetPart.Scale;

                PreviewPart.name = offsetPart.Part.Id.ToString();

                //PreviewPart.hideFlags = HideFlags.HideAndDontSave;

                DestroyImmediate(PreviewPart.GetComponent<PartBehaviour>());

                foreach (SocketBehaviour Socket in PreviewPart.GetComponentsInChildren<SocketBehaviour>())
                    DestroyImmediate(Socket);

                Material PreviewMaterial = new Material(Shader.Find(Constants.DIFFUSE_SHADER_NAME));

                PreviewMaterial.color = Color.cyan;

                PreviewPart.ChangeAllMaterialsInChildren(PreviewPart.GetComponentsInChildren<MeshRenderer>(), PreviewMaterial);
            }
        }

        private void ClearPreview()
        {
            if (PreviewPart != null)
            {
                DestroyImmediate(PreviewPart);

                PreviewPart = null;
            }
        }

        private void ChangeHandle(EditorHandleType type)
        {
            if (type == CurrentHandle)
                type = EditorHandleType.None;

            CurrentHandle = type;
        }

        #endregion Private Methods
    }
}