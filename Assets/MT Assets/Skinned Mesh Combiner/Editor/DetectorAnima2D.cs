﻿using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

namespace SkinnedMeshCombiner_ForEditor
{
    /*
     * This class is responsible for detecting if Anima2D is present in the project, so the Skinned Mesh Combiner component can handle each situation.
     */

    public class DetectorAnima2D : MonoBehaviour
    {
        [UnityEditor.Callbacks.DidReloadScripts]
        public static void DetectIfAnima2dAssetExists()
        {
            //Get active build target
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;

            if (NamespaceExists("Anima2D") == true)
            {
                //If anima2d is available, add the define
                AddDefineIfNecessary("MTAssets_Anima2D_Available", BuildPipeline.GetBuildTargetGroup(buildTarget));
            }
            if (NamespaceExists("Anima2D") == false)
            {
                //If anima2d is not available, remove the define
                RemoveDefineIfNecessary("MTAssets_Anima2D_Available", BuildPipeline.GetBuildTargetGroup(buildTarget));
            }
        }

        public static bool NamespaceExists(string desiredNamespace)
        {
            //Return true if namespace exists
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.Namespace == desiredNamespace)
                        return true;
                }
            }
            return false;
        }

        public static void AddDefineIfNecessary(string _define, BuildTargetGroup _buildTargetGroup)
        {
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(_buildTargetGroup);

            if (defines == null) { defines = _define; }
            else if (defines.Length == 0) { defines = _define; }
            else { if (defines.IndexOf(_define, 0) < 0) { defines += ";" + _define; } }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(_buildTargetGroup, defines);
        }

        public static void RemoveDefineIfNecessary(string _define, BuildTargetGroup _buildTargetGroup)
        {
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(_buildTargetGroup);

            if (defines.StartsWith(_define + ";"))
            {
                // First of multiple defines.
                defines = defines.Remove(0, _define.Length + 1);
            }
            else if (defines.StartsWith(_define))
            {
                // The only define.
                defines = defines.Remove(0, _define.Length);
            }
            else if (defines.EndsWith(";" + _define))
            {
                // Last of multiple defines.
                defines = defines.Remove(defines.Length - _define.Length - 1, _define.Length + 1);
            }
            else
            {
                // Somewhere in the middle or not defined.
                var index = defines.IndexOf(_define, 0, System.StringComparison.Ordinal);
                if (index >= 0) { defines = defines.Remove(index, _define.Length + 1); }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(_buildTargetGroup, defines);
        }
    }
}