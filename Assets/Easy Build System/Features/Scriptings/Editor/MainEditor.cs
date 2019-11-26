using EasyBuildSystem.Runtimes;
using EasyBuildSystem.Runtimes.Extensions;
using EasyBuildSystem.Runtimes.Internal.Addons;
using EasyBuildSystem.Runtimes.Internal.Area;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Internal.Socket;
using System;
using System.Collections.Generic;
using UnityEditor;
#if UNITY_2018_3
using UnityEditor.Experimental.SceneManagement;
#endif
using UnityEditorInternal;
using UnityEngine;

namespace EasyBuildSystem.Editor
{
    public class MainEditor : EditorWindow
    {
        #region Public Fields

        public static List<AddOnAttribute> Addons = new List<AddOnAttribute>();

        public static Color GetEditorColor = new Color(0f, 2f, 2f);

        public static Action LastAction;

        #endregion Public Fields

        #region Private Fields

        private static MainEditor Window;

        private int NavigationIndex;

        private static List<BuildTargetGroup> Targets;

        private Vector2 AddonsScrollPosition;

        private Vector2 IntegrationScrollPosition;

        private static List<string> Integrations = new List<string>();

        #endregion Private Fields

        #region Private Methods

        private void OnEnable()
        {
            Addons.Clear();

            Addons = AddOnHelper.GetAddons();

            Targets = new List<BuildTargetGroup>();

            Targets.Add(BuildTargetGroup.iOS);

            Targets.Add(BuildTargetGroup.WebGL);

            Targets.Add(BuildTargetGroup.Standalone);

            Targets.Add(BuildTargetGroup.Android);
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(1, -14, Screen.width - 1, Screen.height + 14), "", "window");

            DrawHeader();

            #region Menu

            GUILayout.BeginHorizontal("box");

            if (NavigationIndex == 0)
                GUI.color = GetEditorColor;
            else
                GUI.color = Color.white;

            if (GUILayout.Button("About & Add-Ons", GUILayout.Height(25)))
                NavigationIndex = 0;

            if (NavigationIndex == 1)
                GUI.color = GetEditorColor;
            else
                GUI.color = Color.white;

            if (GUILayout.Button("Integration(s)", GUILayout.Height(25)))
                NavigationIndex = 1;

            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            #endregion Menu

            #region Navigations

            if (NavigationIndex == 0)
            {
                #region About

                GUILayout.BeginVertical("box");

                GUI.color = GetEditorColor;

                GUILayout.Label("About : ", EditorStyles.largeLabel);

                GUI.color = Color.white;

                GUILayout.BeginHorizontal();

                GUILayout.Label("You've specific questions about the asset ? Do not hesitate to contact us.\nDo not forget to write a review on Asset Store about what you think of the system.\nThe documentation of the asset is available and updated to each update."
                    , EditorStyles.label);

                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                GUI.color = GetEditorColor;

                if (GUILayout.Button("About Us..."))
                    Application.OpenURL("https://www.assetstore.unity3d.com/en/?stay#!/search/page=1/sortby=popularity/query=publisher:15834");

                if (GUILayout.Button("Tutorial(s)..."))
                    Application.OpenURL("https://github.com/AdsCryptoz22/EasyBuildSystem/wiki/Tutorials");

                if (GUILayout.Button("Documentation..."))
                    Application.OpenURL(Constants.DOCS_LINK);

                GUI.color = Color.white;

                GUILayout.Space(5);

                GUILayout.EndVertical();

                #endregion About

                #region Add-Ons

                GUILayout.BeginVertical("box");

                GUI.color = GetEditorColor;

                GUILayout.Label("Add-On(s) :", EditorStyles.largeLabel);

                GUI.color = Color.white;

                EditorGUILayout.HelpBox("The add-ons can will be only activated directly on their target component in add-ons section.\n" +
                    "You can find more information about them below or in the documentation.", MessageType.Info);

                AddonsScrollPosition = GUILayout.BeginScrollView(AddonsScrollPosition);

                if (Addons.Count == 0)
                    GUILayout.Label("No Add-On(s) are available for this component.", EditorStyles.miniLabel);
                else
                {
                    foreach (AddOnAttribute Addon in Addons)
                    {
                        GUILayout.BeginVertical("box");

                        GUI.color = new Color(1.5f, 1.5f, 1.5f);

                        GUILayout.BeginHorizontal();

                        GUILayout.BeginVertical();

                        GUILayout.Space(3);

                        GUI.color = new Color(1f, 1f, 1f);

                        GUILayout.Label(Addon.Name, EditorStyles.label);

                        GUILayout.EndVertical();

                        GUILayout.FlexibleSpace();

                        GUILayout.BeginVertical();

                        GUI.color = GetEditorColor;

                        if (GUILayout.Button("About This", GUILayout.Width(100)))
                        {
                            EditorUtility.DisplayDialog("Easy Build System - Add-On", "Name : " + Addon.Name
                                + "\nTarget Component : " + Addon.Target.ToString()
                                + "\nAuthor : " + Addon.Author
                                + "\nDescription :\n" + Addon.Description, "Ok");
                        }

                        GUI.color = Color.white;

                        GUILayout.Space(5);

                        GUILayout.EndVertical();

                        GUILayout.EndHorizontal();

                        GUI.color = Color.white;

                        GUILayout.EndVertical();
                    }
                }

                GUILayout.EndScrollView();

                GUILayout.EndVertical();

                #endregion Add-Ons
            }

            if (NavigationIndex == 1)
            {
                #region Integrations

                GUILayout.BeginVertical("box");

                GUI.color = GetEditorColor;

                GUILayout.Label("Integration(s) :", EditorStyles.largeLabel);

                GUI.color = Color.white;

                EditorGUILayout.HelpBox("It is possible to use the Quick Start function to some integration in (Miscs & Tools).\n" +
                    "Note : If the version of an integration no longer compatible, please to report it to the support.", MessageType.Info);

                IntegrationScrollPosition = GUILayout.BeginScrollView(IntegrationScrollPosition);

                AddIntegration("(Unity) uNet", "", "", "", null, null);

                AddIntegration("(Exit Games) Photon Network (PUN1)", "/content/1786", "EXITGAMESV1", "", null, null);

                AddIntegration("(Exit Games) Photon Network (PUN2)", "/content/119922", "EXITGAMESV2", "", null, null);

                AddIntegration("(Devdog) Inventory Pro", "/content/66801", "INVENTORYPRODEVDOG", "Requires version 2.5 or higher of Inventory Pro.", null, null);

                AddIntegration("(Devdog) Rucksack", "/content/114921", "RUCKSACKDEVDOG", "", null, null);

                AddIntegration("(Vis2k) uSurvival", "/content/95015", "USURVIVALVIS2K", "", null, null);

                AddIntegration("(Vis2k) uMMORPG", "/content/51212", "UMMORPGVIS2K", "", null, null);

                AddIntegration("(Vis2k) uRPG", "/content/95016", "URPGVIS2K", "", null, null);

                AddIntegration("(Denis Pahunov) Voxeland", "/content/9180", "VOXELAND", "Requires version 3.1 or higher of Voxeland.", null, null);

                AddIntegration("(Opsive) Third Person Controller", "/content/27438", "OPSIVE", "", null, null);

                GUILayout.EndScrollView();

                GUILayout.FlexibleSpace();

                GUILayout.EndVertical();

                #endregion Integrations
            }

            #endregion Navigations

            GUILayout.BeginHorizontal("box");

            GUI.color = GetEditorColor;

            if (GUILayout.Button("Close", GUILayout.Height(30)))
            {
                Close();
            }

            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawHeader()
        {
            #region Header

            GUILayout.BeginHorizontal("box");

            GUILayout.FlexibleSpace();

            GUILayout.Label(Resources.Load<Texture2D>("Editor/header"));

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            #endregion Header
        }

        private void AddIntegration(string name, string link, string defName, string description, Action onEnable, Action onDisable)
        {
            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();

            if (defName == string.Empty)
                GUI.enabled = false;
            else
                GUI.enabled = true;

            GUILayout.Space(3);
            GUILayout.BeginVertical();
            GUILayout.Space(2);
            if (GUILayout.Toggle(IsIntegrationEnabled(defName), " " + name))
            {
                EnableIntegration(defName, onEnable);
            }
            else
            {
                DisableIntegration(defName, onDisable);
            }
            GUILayout.EndVertical();

            GUI.enabled = true;

            GUILayout.FlexibleSpace();

            if (description.Length != 0)
            {
                GUI.color = new Color(1.5f, 1.5f, 0f);

                GUILayout.BeginVertical();
                GUILayout.Space(2);
                if (GUILayout.Button(new GUIContent(@"/!\", description), GUILayout.Width(30)))
                {
                    EditorUtility.DisplayDialog("Easy Build System - Integration", description, "Ok");
                }
                GUILayout.EndVertical();
            }

            GUI.color = GetEditorColor;

            GUILayout.BeginVertical();
            GUILayout.Space(2);
            if (GUILayout.Button("Asset Store Page", GUILayout.Width(125)) && link != string.Empty)
            {
                AssetStore.Open(link);
            }
            GUILayout.EndVertical();

            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            GUILayout.Space(3);

            GUILayout.EndVertical();
        }

        private static bool IsIntegrationEnabled(string name)
        {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone).Contains(name);
        }

        #endregion Private Methods

        #region Public Methods

        public static void Init()
        {
            Window = CreateInstance<MainEditor>();

            Window.titleContent = new GUIContent("Easy Build System - Main Editor");

            Window.minSize = new Vector2(615, 505);

            Window.maxSize = new Vector2(615, 800);

            Window.ShowUtility();
        }

        public static void DisableIntegration(string name, Action onDisable)
        {
            if (IsIntegrationEnabled(name) == false)
            {
                return;
            }

            if (onDisable != null)
            {
                onDisable.Invoke();
            }

            foreach (BuildTargetGroup Target in Targets)
            {
                string Symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(Target);

                string[] SplitArray = Symbols.Split(';');

                List<string> Array = new List<string>(SplitArray);

                Array.Remove(name);

                if (Target != BuildTargetGroup.Unknown)
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(Target, string.Join(";", Array.ToArray()));
                }
            }

            Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : Integration <b>(" + name + ")</b> has been disabled !");
        }

        public static void EnableIntegration(string name, Action onEnable)
        {
            if (IsIntegrationEnabled(name))
            {
                return;
            }

            Targets = new List<BuildTargetGroup>();

            Targets.Add(BuildTargetGroup.iOS);

            Targets.Add(BuildTargetGroup.WebGL);

            Targets.Add(BuildTargetGroup.Standalone);

            Targets.Add(BuildTargetGroup.Android);

            foreach (BuildTargetGroup Target in Targets)
            {
                string Symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(Target);

                string[] SplitArray = Symbols.Split(';');

                List<string> Array = new List<string>(SplitArray);

                Array.Add(name);

                if (Target != BuildTargetGroup.Unknown)
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(Target, string.Join(";", Array.ToArray()));
                }
            }

            if (onEnable != null)
            {
                Integrations.Add(onEnable.Method.Name);

                onEnable.Invoke();
            }

            Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : Integration <b>(" + name + ")</b> has been enabled !");
        }

        public static void LoadAddons(MonoBehaviour target, AddOnTarget addOnTarget)
        {
            Addons = AddOnHelper.GetAddonsByTarget(addOnTarget);

            foreach (AddOnBehaviour Addon in target.GetComponentsInChildren<AddOnBehaviour>())
                if (Addon != null)
                    Addon.hideFlags = HideFlags.HideInInspector;
        }

        public static void DrawAddons(MonoBehaviour target, AddOnTarget addOnTarget)
        {
            if (Addons.Count == 0)
                GUILayout.Label("No Add-On(s) are available for this component.", EditorStyles.miniLabel);
            else
            {
                foreach (AddOnAttribute Addon in Addons)
                {
                    if (Addon.Target == addOnTarget)
                    {
                        GUILayout.BeginVertical("box");

                        GUILayout.BeginHorizontal();

                        GUILayout.BeginHorizontal();

                        GUILayout.Space(3);

                        GUILayout.Label(Addon.Name, EditorStyles.label);

                        GUILayout.FlexibleSpace();

                        GUI.color = GetEditorColor;

                        if (GUILayout.Button("About This", GUILayout.Width(100)))
                            EditorUtility.DisplayDialog("Easy Build System - Information", "Name : " + Addon.Name
                                + "\nAuthor : " + Addon.Author, "Ok");

                        if (target.gameObject.GetComponent(Addon.Behaviour) == null)
                        {
                            GUI.color = new Color(0f, 1.5f, 0f);

                            if (GUILayout.Button("Enable Add-On", GUILayout.Width(130)))
                            {
                                if (target.gameObject.GetComponent(Addon.Behaviour) != null)
                                    return;

                                Component Com = target.gameObject.AddComponent(Addon.Behaviour);

                                Com.hideFlags = HideFlags.HideInInspector;
                            }
                        }
                        else
                        {
                            GUI.color = new Color(2f, 0f, 0f);

                            if (GUILayout.Button("Disable Add-On", GUILayout.Width(130)))
                            {
                                try
                                {
                                    DestroyImmediate(target.gameObject.GetComponent(Addon.Behaviour));
                                   
                                    break;
                                }
                                catch { }
                            }
                        }

                        GUILayout.EndHorizontal();

                        GUILayout.EndHorizontal();

                        GUI.color = Color.white;

                        if (target.gameObject.GetComponent(Addon.Behaviour) != null)
                        {
                            EditorGUILayout.HelpBox(Addon.Description, MessageType.Info);

                            GUILayout.BeginHorizontal();

                            GUILayout.Space(13);

                            GUILayout.BeginVertical();

                            UnityEditor.Editor AddonEditor = UnityEditor.Editor.CreateEditor(target.gameObject.GetComponent(Addon.Behaviour));

                            AddonEditor.DrawDefaultInspector();

                            GUILayout.EndVertical();

                            GUILayout.EndHorizontal();
                        }

                        GUILayout.EndVertical();
                    }
                }
            }
        }

        #region Tools

        public static void CheckLayers()
        {
            if (LoadLayers() == true)
                Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : All layer(s) linked to the system have been added.");
            else
                Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : All the layer(s) are correctly loaded.");
        }

        public static bool LoadLayers()
        {
            string[] LayerNames = new string[] { Constants.LAYER_SOCKET, Constants.LAYER_INTERACTION };

            bool Result = false;

            SerializedObject Manager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            SerializedProperty SerializedLayers = Manager.FindProperty("layers");

            foreach (string LayerName in LayerNames)
            {
                bool IsFound = false;

                for (int i = 0; i <= 31; i++)
                {
                    SerializedProperty SerializedProperty = SerializedLayers.GetArrayElementAtIndex(i);

                    if (SerializedProperty != null && LayerName.Equals(SerializedProperty.stringValue))
                    {
                        IsFound = true;

                        break;
                    }
                }

                if (!IsFound)
                {
                    Result = true;

                    SerializedProperty SerializedSlot = null;

                    for (int i = 8; i <= 31; i++)
                    {
                        SerializedProperty sp = SerializedLayers.GetArrayElementAtIndex(i);

                        if (sp != null && string.IsNullOrEmpty(sp.stringValue))
                        {
                            SerializedSlot = sp;

                            break;
                        }
                    }

                    if (SerializedSlot != null)
                    {
                        SerializedSlot.stringValue = LayerName;

                        Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : The layer (<b>" + LayerName + "</b>) has been added.");
                    }
                    else
                    {
                        Debug.LogError("<b><color=cyan>[Easy Build System]</color></b> : Could not find an open Layer Slot for : " + LayerName);
                    }
                }
            }

            Manager.ApplyModifiedProperties();

            return Result;
        }

        #endregion Tools

        #region Miscs

        public static void CreateNewPart()
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<BuildManager>() == null)
            {
                if (Selection.activeGameObject.GetComponentInParent<PartBehaviour>() == null)
                {
                    GameObject Parent = new GameObject("Part " + Selection.activeGameObject.name);

                    string LocalPath = EditorUtility.SaveFilePanel("Save Path to Part (" + Parent.name + ")", "", Parent.name + ".prefab", "prefab");

                    if (LocalPath == string.Empty)
                    {
                        DestroyImmediate(Parent);

                        return;
                    }

                    try
                    {
                        LocalPath = LocalPath.Substring(LocalPath.LastIndexOf("Assets"));
                    }
                    catch { return; }

                    if (LocalPath != string.Empty)
                    {
                        Selection.activeGameObject.transform.SetParent(Parent.transform, false);

                        Selection.activeGameObject.transform.position = Vector3.zero;

                        PartBehaviour Temp = Parent.AddComponent<PartBehaviour>();

                        Temp.Name = Selection.activeGameObject.name;

                        Temp.MeshBounds = Parent.GetChildsBounds();

                        Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : Bounds generated " + Temp.MeshBounds.size.ToString() + ".");

#if UNITY_2018_3 || UNITY_2019
                        UnityEngine.Object AssetPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(Temp.gameObject, LocalPath, InteractionMode.UserAction);

                        EditorGUIUtility.PingObject(AssetPrefab);
#else
                        UnityEngine.Object AssetPrefab = PrefabUtility.CreateEmptyPrefab(LocalPath);

                        GameObject Asset = PrefabUtility.ReplacePrefab(Parent, AssetPrefab, ReplacePrefabOptions.ConnectToPrefab);

                        AssetDatabase.Refresh();

                        EditorGUIUtility.PingObject(Asset);
#endif

                        SceneHelper.Focus(Parent, DrawCameraMode.Textured);

                        Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : The part has been created !");
                    }
                }
                else
                    Debug.LogError("<b><color=red>[Easy Build System]</color></b> : This selected object has already a Base Part component.");
            }
            else
            {
                if (!EditorUtility.DisplayDialog("Easy Build System - Information", "You've not selected object.\nDo you want create a empty Base Part ?", "Yes", "No"))
                    return;

                GameObject Parent = new GameObject("New Part");

                string LocalPath = EditorUtility.SaveFilePanel("Save Path to Part (" + Parent.name + ")", "", Parent.name + ".prefab", "prefab");

                if (LocalPath == string.Empty)
                {
                    DestroyImmediate(Parent);
                    return;
                }

                try
                {
                    LocalPath = LocalPath.Substring(LocalPath.LastIndexOf("Assets"));
                }
                catch { return; }

                if (LocalPath != string.Empty)
                {
                    Parent.AddComponent<PartBehaviour>();
#if UNITY_2018_3
                    GameObject Asset = PrefabUtility.SaveAsPrefabAssetAndConnect(Parent, LocalPath, InteractionMode.UserAction);

                    EditorGUIUtility.PingObject(Asset);
#elif UNITY_2018
                    UnityEngine.Object AssetPrefab = PrefabUtility.CreateEmptyPrefab(LocalPath);

                    GameObject Asset = PrefabUtility.ReplacePrefab(Parent, AssetPrefab, ReplacePrefabOptions.ConnectToPrefab);

                    AssetDatabase.Refresh();

                    EditorGUIUtility.PingObject(Asset);
#endif
                    SceneHelper.Focus(Parent, DrawCameraMode.Textured);

                    Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : The part parent has been created, you can now add your mesh(s) and configure it.");
                }
            }
        }

        public static void CreateNewSocket()
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<BuildManager>() == null)
            {
                GameObject Child = new GameObject("New Socket");

                Child.transform.SetParent(Selection.activeGameObject.transform, false);

                Child.AddComponent<SocketBehaviour>();

                SceneHelper.Focus(Child, DrawCameraMode.Textured);

                Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : The socket has been created in the parent (" + Selection.activeGameObject.name + ").");
            }
            else
            {
                GameObject Parent = new GameObject("New Socket");

                Parent.AddComponent<SocketBehaviour>();

                SceneHelper.Focus(Parent, DrawCameraMode.Textured);

                Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : The socket has been created, you can now add this to in children part.");
            }
        }

        public static void CreateNewArea()
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<BuildManager>() == null)
            {
                GameObject Child = new GameObject("New Area");

                Child.transform.SetParent(Selection.activeGameObject.transform, false);

                Child.AddComponent<AreaBehaviour>();

                SceneHelper.Focus(Child, DrawCameraMode.Textured);

                Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : The area has been created in the parent (" + Selection.activeGameObject.name + ").");
            }
            else
            {
                GameObject Parent = new GameObject("New Area");

                Parent.AddComponent<AreaBehaviour>();

                SceneHelper.Focus(Parent, DrawCameraMode.Textured);

                Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : The area has been created, you can now add this to in children part.");
            }
        }

        #endregion Miscs

        #endregion Public Methods
    }
}