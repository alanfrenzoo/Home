using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace EasyBuildSystem.Editor.Extensions
{
    public static class ScriptableObjectExtension
    {
        #region Public Class

        internal class EndNameEdit : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                AssetDatabase.CreateAsset(EditorUtility.InstanceIDToObject(instanceId), AssetDatabase.GenerateUniqueAssetPath(pathName));
            }
        }

        #endregion Public Class

        #region Public Methods

        public static T CreateAsset<T>(string name, bool select = true) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = "Assets/" + name + ".asset";

            AssetDatabase.CreateAsset(asset, path);

            AssetDatabase.SaveAssets();

            if (select)
            {
                EditorUtility.FocusProjectWindow();

                Selection.activeObject = asset;

                EditorGUIUtility.PingObject(asset);
            }

            return asset;
        }

        #endregion Public Methods
    }
}