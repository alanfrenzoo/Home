using EasyBuildSystem.Editor.Extensions;
using EasyBuildSystem.Runtimes;
using EasyBuildSystem.Runtimes.Internal.Blueprint.Data;
using EasyBuildSystem.Runtimes.Internal.Managers.Data;
using UnityEditor;
using UnityEngine;

namespace EasyBuildSystem.Editor
{
    public class MenuItems : ScriptableObject
    {
        #region Public Methods

        [MenuItem("Tools/Easy Build System/Main Editor %#e", priority = 1)]
        public static void EditorWindow()
        {
            MainEditor.Init();
        }

        [MenuItem("Tools/Easy Build System/Utilities.../Check Layer(s)...", priority = 100)]
        public static void EditorCheckLayer()
        {
            MainEditor.CheckLayers();
        }

        [MenuItem("Tools/Easy Build System/Utilities.../Create New Area...", priority = 200)]
        public static void EditorCreateArea()
        {
            MainEditor.CreateNewArea();
        }

        [MenuItem("Tools/Easy Build System/Utilities.../Create New Part...", priority = 200)]
        public static void EditorCreatePart()
        {
            MainEditor.CreateNewPart();
        }

        [MenuItem("Tools/Easy Build System/Utilities.../Create New Socket...", priority = 200)]
        public static void EditorCreateSocket()
        {
            MainEditor.CreateNewSocket();
        }

        [MenuItem("Tools/Easy Build System/Scriptable Object(s)/Create New Parts Collection...", priority = 50)]
        public static void EditorCreatePartsCollection()
        {
            ScriptableObjectExtension.CreateAsset<PartsCollection>("New Parts Collection...");
        }

        [MenuItem("Tools/Easy Build System/Scriptable Object(s)/Create New Blueprint Data...", priority = 50)]
        public static void EditorCreateBlueprintData()
        {
            ScriptableObjectExtension.CreateAsset<BlueprintData>("New Empty Blueprint...");
        }

        [MenuItem("Tools/Easy Build System/About Us...", priority = 999)]
        public static void EditorLinkAboutUs()
        {
            Application.OpenURL("https://www.assetstore.unity3d.com/en/?stay#!/search/page=1/sortby=popularity/query=publisher:15834");
        }

        [MenuItem("Tools/Easy Build System/Tutorials...", priority = 999)]
        public static void EditorLinkTutorials()
        {
            Application.OpenURL("https://github.com/AdsCryptoz22/EasyBuildSystem/wiki/Tutorials");
        }

        [MenuItem("Tools/Easy Build System/Documentation...", priority = 999)]
        public static void EditorLinkDocumenation()
        {
            Application.OpenURL(Constants.DOCS_LINK);
        }

        #endregion Public Methods
    }
}