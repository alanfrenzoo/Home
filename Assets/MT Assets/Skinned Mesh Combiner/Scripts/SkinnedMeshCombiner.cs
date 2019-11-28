#if UNITY_EDITOR
    using UnityEditor; 
    using UnityEngine.SceneManagement;
    using System.IO;
#endif
#if MTAssets_Anima2D_Available
    using Anima2D;
#endif
using MTAssets.PluginsOfSMC.ExtensionForMeshClass;
using MTAssets.PluginsOfSMC.TextureResizerClass;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MTAssets
{
    /*
     This class is responsible for the functioning of the "Skinned Mesh Combiner" component, and all its functions.
    */
    /*
     * The Skinned Mesh Combiner was developed by Marcos Tomaz in 2019.
     * Need help? Contact me (mtassets@windsoft.xyz)
     */

    [ExecuteInEditMode]
    public class SkinnedMeshCombiner : MonoBehaviour
    {
        private int SIZE_OF_EDGES_TEXTURES = 150;
        private int PADDING_OF_ATLAS = 2;
        private string ERROR_MESSAGE_ON_TRY_READ_TEXTURES = "There was an error reading the textures. It was not possible to combine them and create the UV of the combined mesh. Please undo the merge, correct the error, and try again. See the following error.";

#if UNITY_EDITOR
        [Serializable]
        public class LogOfSMC
        {
            public string content;
            public LogType logType;

            public LogOfSMC(string content, LogType logType)
            {
                this.content = content;
                this.logType = logType;
            }
        }
        [HideInInspector]
        public int currentTab = 0;
        [HideInInspector]
        public string statsOfMerge = "";
        [HideInInspector]
        public int numberOfMerges = 0;
        [HideInInspector]
        public bool saveDataInAssets = false;
        [HideInInspector]
        public bool exportTexturesAsPng = false;
        [HideInInspector]
        public string nameOfTextures = "Texture";
        [HideInInspector]
        public bool savePrefabOfThis = false;
        [HideInInspector]
        public bool alreadySettedFirstName = false;
        [HideInInspector]
        public string prefabName = "PrefabOfSkinnedMeshes";
        [HideInInspector]
        public List<string> pathsOfDataSavedInAssets = new List<string>();
        [HideInInspector]
        public Texture atlasMergedInAllInOne;
        [HideInInspector]
        public List<LogOfSMC> logs = new List<LogOfSMC>();
        [HideInInspector]
        public bool clearOnUndoMerge = true;
#endif
        [HideInInspector]
        public enum MaterialType
        {
            CustomMaterial,
            InternalMaterial
        }
        [HideInInspector]
        public MaterialType materialType = MaterialType.InternalMaterial;
        [HideInInspector]
        public Material materialCustom;
        [HideInInspector]
        public enum ListInternalMaterial
        {
            ProDiffuseCutout,
            ProDiffuseCutoutCullingOff,
            LightVertexLit,
            LightVertexLitCullingOff,
            StandardMetallic,
            StandardMetallicCullingOff,
            StandardSpecular,
            StandardSpecularCullingOff
        }
        [HideInInspector]
        public ListInternalMaterial internalMaterialList = ListInternalMaterial.ProDiffuseCutout;
        [HideInInspector]
        public enum AtlasSize
        {
            Pixels32x32,
            Pixels64x64,
            Pixels128x128,
            Pixels256x256,
            Pixels512x512,
            Pixels1024x1024,
            Pixels2048x2048,
            Pixels4096x4096,
            Pixels8192x8192
        }
        [HideInInspector]
        public enum RenderMode
        {
            Opaque,
            Cutout,
            Fade,
            Transparent
        }
        [HideInInspector]
        public RenderMode renderMode;
        [HideInInspector]
        public float alphaCutOff = 0;
        [HideInInspector]
        public float metallic = 0;
        [HideInInspector]
        public Color specular = new Color(51f / 255f, 51f / 255f, 51f / 255f, 255f / 255f);
        [HideInInspector]
        public float smoothness = 0.5f;
        [HideInInspector]
        public bool specularHighlights;
        [HideInInspector]
        public bool reflections;
        [HideInInspector]
        public AtlasSize atlasResolution = AtlasSize.Pixels512x512;
        [HideInInspector]
        public enum AtlasFormat
        {
            RGBA16Bits,
            RGBA32Bits,
        }
        [HideInInspector]
        public AtlasFormat atlasFormat = AtlasFormat.RGBA32Bits;
        [HideInInspector]
        public bool atlasLinearFilter = true;
        [HideInInspector]
        public bool atlasMipMap = true;
        [HideInInspector]
        public bool mergeAllUVSizes = true;
        [HideInInspector]
        public bool compatibilityMode = true;
        [HideInInspector]
        public bool enableAdvancedAtlas = false;
        [HideInInspector]
        public bool normalMapSupport = false;
        [HideInInspector]
        public string normalMapPropertyNameFind = "_BumpMap";
        [HideInInspector]
        public string normalMapPropertyNameInsert = "_BumpMap";
        [HideInInspector]
        public bool applyNormalMapScale = false;
        [HideInInspector]
        public string normalMapScalePropertyName = "_BumpScale";
        [HideInInspector]
        public float normalMapScale = 1;
        [HideInInspector]
        public bool secondNormalMapSupport = false;
        [HideInInspector]
        public string secondNormalMapPropertyNameFind = "_DetailNormalMap";
        [HideInInspector]
        public string secondNormalMapPropertyNameInsert = "_DetailNormalMap";
        [HideInInspector]
        public bool applySecondNormalMapScale = false;
        [HideInInspector]
        public string secondNormalMapScalePropertyName = "_DetailNormalMapScale";
        [HideInInspector]
        public float secondNormalMapScale = 1;
        [HideInInspector]
        public bool heightMapSupport = false;
        [HideInInspector]
        public string heightMapPropertyNameFind = "_ParallaxMap";
        [HideInInspector]
        public string heightMapPropertyNameInsert = "_ParallaxMap";
        [HideInInspector]
        public bool applyHeightMapScale = false;
        [HideInInspector]
        public string heightMapScalePropertyName = "_Parallax";
        [HideInInspector]
        public float heightMapScale = 1;
        [HideInInspector]
        public bool occlusionMapSupport = false;
        [HideInInspector]
        public string occlusionMapPropertyNameFind = "_OcclusionMap";
        [HideInInspector]
        public string occlusionMapPropertyNameInsert = "_OcclusionMap";
        [HideInInspector]
        public bool applyOcclusionMapScale = false;
        [HideInInspector]
        public string occlusionMapScalePropertyName = "_OcclusionStrength";
        [HideInInspector]
        public float occlusionMapScale = 1;
        [HideInInspector]
        public bool detailAlbedoMapSupport = false;
        [HideInInspector]
        public string detailAlbedoMapPropertyNameFind = "_DetailAlbedoMap";
        [HideInInspector]
        public string detailAlbedoMapPropertyNameInsert = "_DetailAlbedoMap";
        [HideInInspector]
        public bool metallicMapSupport = false;
        [HideInInspector]
        public string metallicMapPropertyNameFind = "_MetallicGlossMap";
        [HideInInspector]
        public string metallicMapPropertyNameInsert = "_MetallicGlossMap";
        [HideInInspector]
        public bool specularMapSupport = false;
        [HideInInspector]
        public string specularMapPropertyNameFind = "_SpecGlossMap";
        [HideInInspector]
        public string specularMapPropertyNameInsert = "_SpecGlossMap";
        [HideInInspector]
        public bool enableMonoGC;
        [HideInInspector]
        public bool enableUnityGC;
        [HideInInspector]
        public enum CombineMethod
        {
            OneMeshPerMaterial,
            AllInOne,
            JustMaterialColors,
            OnlyAnima2DMeshes
        }
        [HideInInspector]
        public CombineMethod mergeMethod;
        [HideInInspector]
        public SkinnedMeshRenderer[] meshesToMerge;
        [HideInInspector]
        public bool isMeshesCombineds = false;
        [HideInInspector]
        public enum AnimQuality
        {
            UseQualitySettings,
            Bad,
            Good,
            VeryGood
        }
        [HideInInspector]
        public AnimQuality qualityOfAnim;
        [HideInInspector]
        public bool updateWhenOffScreen = false;
        [HideInInspector]
        public bool skinnedMotionVectors = true;
        [HideInInspector]
        public bool mergeOnlyEqualsRootBones = true;
        [HideInInspector]
        public bool combineInactives = false;
        [HideInInspector]
        public bool moreThan65kVertices = false;
        [HideInInspector]
        public bool enableDebugging = false;
        [HideInInspector]
        public bool showUvVerticesInAtlas = false;
        [HideInInspector]
        public List<SkinnedMeshRenderer> meshesToIgnore = new List<SkinnedMeshRenderer>();
#if MTAssets_Anima2D_Available
        [HideInInspector]
        public List<SpriteMeshInstance> meshesToIgnoreOfAnima2dMergeMethod = new List<SpriteMeshInstance>();
        [HideInInspector]
        public SpriteMeshInstance[] spriteMeshInstances;
#endif

#if UNITY_EDITOR
        //The UI of this component
        [UnityEditor.CustomEditor(typeof(SkinnedMeshCombiner)), CanEditMultipleObjects]
        public class ConfiguracaoUI : UnityEditor.Editor
        {
            bool listExpanded_meshesToIgnore = false;
            bool listExpanded_materialOptions = false;
            bool listExpanded_normalMapSupport = false;
            bool listExpanded_normalMapSecondarySupport = false;
            bool listExpanded_heightMapSupport = false;
            bool listExpanded_occlusionMapSupport = false;
            bool listExpanded_detailAlbedoMapSupport = false;
            bool listExpanded_metallicMapSupport = false;
            bool listExpanded_specularMapSupport = false;
            Vector2 scrollPosLogs;

            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                serializedObject.Update();
                SkinnedMeshCombiner script = (SkinnedMeshCombiner)target;

                //Verify if Skinned Mesh Combiner is duplicate
                SkinnedMeshCombiner[] components = script.GetComponents<SkinnedMeshCombiner>();
                if (components.Length > 1)
                {
                    EditorGUILayout.HelpBox("\nOops..\n\nIt looks like there are more than 1 Skinned Mesh Combiner in this GameObject. Please remove all and leave only 1 to ensure a smooth operation!\n", MessageType.Error);
                }
                if (components.Length == 1)
                {
                    //Warning if animator not found
                    if (script.GetComponent<Animator>() == null)
                    {
                        GUILayout.Space(10);
                        EditorGUILayout.HelpBox("The \"Animator\" component could not be found on this object. Please add the \"Animator\" component. The Skinned Mesh Combiner only works on the root of animated objects, next to the \"Animator\" component. Read the documentation for more details.", MessageType.Error);
                    }

                    //Support reminder
                    GUILayout.Space(10);
                    EditorGUILayout.HelpBox("Remember to read the Skinned Mesh Combiner documentation to understand how to use it.\nGet support at: mtassets@windsoft.xyz", MessageType.None);

                    GUILayout.Space(10);

                    //Show the toolbar
                    script.currentTab = GUILayout.Toolbar(script.currentTab, new string[] { "Merge", "Stats", "Logs of Merge (" + script.logs.Count + ")" });

                    GUILayout.Space(10);

                    //Draw the content of toolbar selected
                    switch (script.currentTab)
                    {
                        case 0:
                            //---------------------------------------- Start of "Merge" tab
                            EditorGUILayout.LabelField("Settings For Combiner", EditorStyles.boldLabel);

                            //Selection box for "Merge Method"
                            script.mergeMethod = (CombineMethod)EditorGUILayout.EnumPopup(new GUIContent("Merge Method",
                                "Method to which the Skinned Mesh Combiner will use to merge the meshes." +
                                "\n\n\"One Mesh Per Material\" - Combines all meshes that share the same materials in just one mesh. All meshes will continue to use the original materials. It is a fast method to use at runtime." +
                                "\n\n\"All In One\" - This merge method combines all meshes in just one mesh. Even if each mesh uses different materials. The textures and materials will also be merged into just one. It is a fast method, but more suitable for use in the editor. You can configure it to be faster and use it at run time without problems." +
                                "\n\n\"Just Material Colors\" - It only works with the main colors of the materials. This blending method does not work with textures, it's perfect for people who do not use textures, just color their characters. All meshes of the model are merged into one, and all colors of the materials are combined in an atlas color palette. The matched mesh will use the colors of this palette." +
                                "\n\n\"Only Anima2D Meshes\" - Merge mode with exclusive compatibility for the \"Anima2D\" tool from Unity Technologies. This merge method combines all meshes generated from sprites (using the Anima2D tool) in just one mesh." +
                                "\n\nYou can read the documentation to understand all the limitations of metods and how the merge process works in this way to get your bearings."),
                                script.mergeMethod);

                            //Settings if "One Mesh Per Material" is activated
                            if (script.mergeMethod == CombineMethod.OneMeshPerMaterial)
                            {
                                EditorGUI.indentLevel += 1;

                                script.mergeOnlyEqualsRootBones = (bool)EditorGUILayout.Toggle(new GUIContent("Only Equals Root Bones",
                                    "This is a security mechanism to prevent your meshes from being deformed after the merge.\n\nIf this option is enabled, the Skinned Mesh Combiner will ignore meshes that have a different Root Bone, thus preventing the combined mesh from being deformed. For example, Mixamo models often have different Root Bones.\n\nIf this option is disabled, meshes with different Root Bones will be combined. Disabling this option does not guarantee that the end result is the one you want."),
                                    script.mergeOnlyEqualsRootBones);

                                EditorGUI.indentLevel -= 1;
                            }

                            //Settings if "All In One" is activated
                            if (script.mergeMethod == CombineMethod.AllInOne)
                            {
                                EditorGUI.indentLevel += 1;

                                script.materialType = (MaterialType)EditorGUILayout.EnumPopup(new GUIContent("Material Type",
                                "The type of material that will be used in the combined mesh that is generated.\n\n\"Internal Material\"- You can select a pre-built material of Skinned Mesh Combiner in list.\n\n\"Custom Material\" - You can add your own custom material so that the merged mesh uses it. The Skinned Mesh Combiner will copy its properties and place it in the merged mesh."),
                                script.materialType);

                                if (script.materialType == MaterialType.CustomMaterial)
                                {
                                    if (script.materialCustom == null)
                                    {
                                        EditorGUILayout.HelpBox("Please add a custom material. This custom material will have its properties copied and will be associated with the merged mesh. The Skinned Mesh Combine can not function without a material.", MessageType.Error);
                                    }
                                    EditorGUILayout.ObjectField(serializedObject.FindProperty("materialCustom"), new GUIContent("Material To Use", "This custom material will have its properties copied and will be associated with the merged mesh."));
                                }
                                if (script.materialType == MaterialType.InternalMaterial)
                                {
                                    script.internalMaterialList = (ListInternalMaterial)EditorGUILayout.EnumPopup(new GUIContent("Material To Use",
                                    "The material that will be used in the merged mesh.\n\n\"Culling Off\" materials will have their faces always drawn, even when viewed behind the normal."),
                                    script.internalMaterialList);

                                    if (script.internalMaterialList == ListInternalMaterial.StandardMetallic || script.internalMaterialList == ListInternalMaterial.StandardMetallicCullingOff ||
                                        script.internalMaterialList == ListInternalMaterial.StandardSpecular || script.internalMaterialList == ListInternalMaterial.StandardSpecularCullingOff)
                                    {
                                        listExpanded_materialOptions = EditorGUILayout.Foldout(listExpanded_materialOptions, new GUIContent("Material Options", "Set here the properties of the final material that will be generated."));
                                        if (listExpanded_materialOptions == true)
                                        {
                                            EditorGUI.indentLevel += 1;

                                            script.renderMode = (RenderMode)EditorGUILayout.EnumPopup(new GUIContent("Render Mode",
                                            "The rendering mode of the final merge material."),
                                            script.renderMode);

                                            if (script.renderMode == RenderMode.Cutout)
                                            {
                                                script.alphaCutOff = EditorGUILayout.Slider("Alpha Cutoff", script.alphaCutOff, 0f, 1f);
                                            }

                                            if(script.internalMaterialList == ListInternalMaterial.StandardMetallic || script.internalMaterialList == ListInternalMaterial.StandardMetallicCullingOff)
                                            {
                                                script.metallic = EditorGUILayout.Slider("Metallic", script.metallic, 0f, 1f);
                                            }

                                            if (script.internalMaterialList == ListInternalMaterial.StandardSpecular || script.internalMaterialList == ListInternalMaterial.StandardSpecularCullingOff)
                                            {
                                                script.specular = EditorGUILayout.ColorField(new GUIContent("Specular",
                                                "The specular color to be applied to the final material."),
                                                script.specular);
                                            }

                                            script.smoothness = EditorGUILayout.Slider("Smoothness", script.smoothness, 0f, 1f);

                                            script.specularHighlights = (bool)EditorGUILayout.Toggle(new GUIContent("Specular Highlights",
                                            "Enable Specular Hightlights in the final material?"),
                                            script.specularHighlights);

                                            script.reflections = (bool)EditorGUILayout.Toggle(new GUIContent("Reflections",
                                            "Activate Reflections on the final material?"),
                                            script.reflections);

                                            EditorGUI.indentLevel -= 1;
                                        }
                                    }
                                }

                                script.atlasFormat = (AtlasFormat)EditorGUILayout.EnumPopup(new GUIContent("Atlas Format",
                                "The format that the generated atlas will have.\n\nRead the documentation of Skinned Mesh Combiner for more details on each type of format."),
                                script.atlasFormat);

                                script.atlasResolution = (AtlasSize)EditorGUILayout.EnumPopup(new GUIContent("Atlas Max Resolution",
                                "The maximum resolution that the generated atlas can have. The higher the texture, the more detail in the model, but the longer the processing time. Larger textures will also consume more video memory."),
                                script.atlasResolution);

                                script.atlasMipMap = (bool)EditorGUILayout.Toggle(new GUIContent("Atlas MipMaps",
                                "Activates the mipmaps for the atlas. If mipmaps are active, more video memory will be consumed, but you'll get better performance. Note that enabling this option increases the processing time when you merge the meshes. Disabling this option can also cause some artifacts or transparent borders to appear in their meshes, depending on the distance between the camera and these meshes.\n\nIf this option is disabled, the mipmaps of the atlas will still be generated if the textures to be merged are with mipmaps enabled in the import settings."),
                                script.atlasMipMap);

                                if (script.atlasMipMap == true)
                                {
                                    script.atlasLinearFilter = (bool)EditorGUILayout.Toggle(new GUIContent("Atlas Linear Filter",
                                    "Enables or disables the linear filter for the generated atlas."),
                                    script.atlasLinearFilter);
                                }

                                script.mergeAllUVSizes = (bool)EditorGUILayout.Toggle(new GUIContent("Merge All UV Sizes",
                                "If this option is enabled, the Skinned Mesh Combiner will try to merge meshes that have the UV map larger than the texture (UV maps larger than the texture, make the textures repeat) using an internal algorithm. When you merge UV maps that are larger than the texture, this mesh may lose some of the texture quality. Enabling this option will increase the optimization as more meshes will be merged.\n\nIf this option is off, the Skinned Mesh Combiner will combine all meshes that have UV mapping less than or equal to the texture size. This option loses a bit in performance, but will maintain the beauty and quality of its textures. Meshes that have a UV map larger than the texture will simply be ignored."),
                                script.mergeAllUVSizes);

                                script.enableAdvancedAtlas = (bool)EditorGUILayout.Toggle(new GUIContent("Enable Advanced Atlas",
                                "If this option is enabled, the Skinned Mesh Combiner will generate the atlas with a different algorithm, that allows textures to be duplicated in the atlas. The atlas will be generated in a way that will allow full compatibility with special effects, such as normal maps.\n\nHowever, be aware that you may have quality losses in texture. If so, try increasing the resolution of the atlas.\n\nYou also need to provide a material that supports especial effects. Be a personalized or pre-defined material. If you are using predefined material, the Standard material (which supports normal maps, and other effects) will be selected automatically.\n\nIn addition, because of the unique processing of each texture, the blending time can be increased."),
                                script.enableAdvancedAtlas);

                                script.mergeOnlyEqualsRootBones = (bool)EditorGUILayout.Toggle(new GUIContent("Only Equals Root Bones",
                                    "This is a security mechanism to prevent your meshes from being deformed after the merge.\n\nIf this option is enabled, the Skinned Mesh Combiner will ignore meshes that have a different Root Bone, thus preventing the combined mesh from being deformed. For example, Mixamo models often have different Root Bones.\n\nIf this option is disabled, meshes with different Root Bones will be combined. Disabling this option does not guarantee that the end result is the one you want."),
                                    script.mergeOnlyEqualsRootBones);

                                EditorGUI.indentLevel -= 1;
                            }

                            //Settings if "Just Material Colors" is activated
                            if (script.mergeMethod == CombineMethod.JustMaterialColors)
                            {
                                EditorGUI.indentLevel += 1;

                                script.materialType = (MaterialType)EditorGUILayout.EnumPopup(new GUIContent("Material Type",
                                "The type of material that will be used in the combined mesh that is generated.\n\n\"Internal Material\"- You can select a pre-built material of Skinned Mesh Combiner in list.\n\n\"Custom Material\" - You can add your own custom material so that the merged mesh uses it. The Skinned Mesh Combiner will copy its properties and place it in the merged mesh."),
                                script.materialType);

                                if (script.materialType == MaterialType.CustomMaterial)
                                {
                                    if (script.materialCustom == null)
                                    {
                                        EditorGUILayout.HelpBox("Please add a custom material. This custom material will have its properties copied and will be associated with the merged mesh. The Skinned Mesh Combine can not function without a material.", MessageType.Error);
                                    }
                                    EditorGUILayout.ObjectField(serializedObject.FindProperty("materialCustom"), new GUIContent("Material To Use", "This custom material will have its properties copied and will be associated with the merged mesh."));
                                }
                                if (script.materialType == MaterialType.InternalMaterial)
                                {
                                    script.internalMaterialList = (ListInternalMaterial)EditorGUILayout.EnumPopup(new GUIContent("Material To Use",
                                    "The material that will be used in the merged mesh."),
                                    script.internalMaterialList);

                                    if (script.internalMaterialList == ListInternalMaterial.StandardMetallic || script.internalMaterialList == ListInternalMaterial.StandardMetallicCullingOff ||
                                        script.internalMaterialList == ListInternalMaterial.StandardSpecular || script.internalMaterialList == ListInternalMaterial.StandardSpecularCullingOff)
                                    {
                                        listExpanded_materialOptions = EditorGUILayout.Foldout(listExpanded_materialOptions, new GUIContent("Material Options", "Set here the properties of the final material that will be generated."));
                                        if (listExpanded_materialOptions == true)
                                        {
                                            EditorGUI.indentLevel += 1;

                                            script.renderMode = (RenderMode)EditorGUILayout.EnumPopup(new GUIContent("Render Mode",
                                            "The rendering mode of the final merge material."),
                                            script.renderMode);

                                            if (script.renderMode == RenderMode.Cutout)
                                            {
                                                script.alphaCutOff = EditorGUILayout.Slider("Alpha Cutoff", script.alphaCutOff, 0f, 1f);
                                            }

                                            if (script.internalMaterialList == ListInternalMaterial.StandardMetallic || script.internalMaterialList == ListInternalMaterial.StandardMetallicCullingOff)
                                            {
                                                script.metallic = EditorGUILayout.Slider("Metallic", script.metallic, 0f, 1f);
                                            }

                                            if (script.internalMaterialList == ListInternalMaterial.StandardSpecular || script.internalMaterialList == ListInternalMaterial.StandardSpecularCullingOff)
                                            {
                                                script.specular = EditorGUILayout.ColorField(new GUIContent("Specular",
                                                "The specular color to be applied to the final material."),
                                                script.specular);
                                            }

                                            script.smoothness = EditorGUILayout.Slider("Smoothness", script.smoothness, 0f, 1f);

                                            script.specularHighlights = (bool)EditorGUILayout.Toggle(new GUIContent("Specular Highlights",
                                            "Enable Specular Hightlights in the final material?"),
                                            script.specularHighlights);

                                            script.reflections = (bool)EditorGUILayout.Toggle(new GUIContent("Reflections",
                                            "Activate Reflections on the final material?"),
                                            script.reflections);

                                            EditorGUI.indentLevel -= 1;
                                        }
                                    }
                                }

                                script.atlasFormat = (AtlasFormat)EditorGUILayout.EnumPopup(new GUIContent("Atlas Format",
                                "The format that the generated atlas will have.\n\nRead the documentation of Skinned Mesh Combiner for more details on each type of format."),
                                script.atlasFormat);

                                script.mergeOnlyEqualsRootBones = (bool)EditorGUILayout.Toggle(new GUIContent("Only Equals Root Bones",
                                    "This is a security mechanism to prevent your meshes from being deformed after the merge.\n\nIf this option is enabled, the Skinned Mesh Combiner will ignore meshes that have a different Root Bone, thus preventing the combined mesh from being deformed. For example, Mixamo models often have different Root Bones.\n\nIf this option is disabled, meshes with different Root Bones will be combined. Disabling this option does not guarantee that the end result is the one you want."),
                                    script.mergeOnlyEqualsRootBones);

                                EditorGUI.indentLevel -= 1;
                            }

                            //Settings if "Only Anima2D Meshes" is activated
                            if (script.mergeMethod == CombineMethod.OnlyAnima2DMeshes)
                            {
                                EditorGUI.indentLevel += 1;
#if MTAssets_Anima2D_Available
                                script.atlasFormat = (AtlasFormat)EditorGUILayout.EnumPopup(new GUIContent("Atlas Format",
                                "The format that the generated atlas will have.\n\nRead the documentation of Skinned Mesh Combiner for more details on each type of format."),
                                script.atlasFormat);

                                script.atlasResolution = (AtlasSize)EditorGUILayout.EnumPopup(new GUIContent("Atlas Max Resolution",
                                "The maximum resolution that the generated atlas can have. The higher the texture, the more detail in the model, but the longer the processing time. Larger textures will also consume more video memory."),
                                script.atlasResolution);
#else
    EditorGUILayout.HelpBox("This merge method works only with meshes generated from Sprites 2D, using the Anima2D tool from Unity Technologies. The Skinned Mesh Combiner has not detected the namespace of this tool anywhere in your project. Please install Anima2D before using this merge method. If you're not going to use Anima2D, consider using other standard merge methods (facing 3D meshes).", MessageType.Error);
#endif
                                EditorGUI.indentLevel -= 1;
                            }

                            //Setting for "Compatibility Mode"
                            script.compatibilityMode = (bool)EditorGUILayout.Toggle(new GUIContent("Compatiblity Mode",
                                "If the compatibility mode is enabled, the Skinned Mesh Combiner will calculate the positions (of the array) in a way that it will reach more types of models (such as .blend, .fbx and others). Enabling compatibility mode will ensure that your animations are not deformed on most types of models.\n\nIf any model is displaying deformed animations, after merge, try disabling this option.\n\n**Disabling this option may cause the distortion of your model's animation. Only disable this option if you believe your model will benefit from this."),
                                script.compatibilityMode);

                            //Setting for "Combine Inactive"
                            script.combineInactives = (bool)EditorGUILayout.Toggle(new GUIContent("Combine Inactives",
                                "If this option is on, the Skinned Mesh Combiner will attempt to combine the disabled meshes too."),
                                script.combineInactives);

                            //Setting for "More Than 65k Vertices"
                            script.moreThan65kVertices = (bool)EditorGUILayout.Toggle(new GUIContent("More Than 65k Vertices",
                                "If support for more than 64,000 vertices is enabled, the 64,000 vertex limitation will no longer be imposed on the merged meshes, but the meshes may consume more GPU memory and bandwidth.\n\nEnable this option if you want to combine meshes without worrying about the limitation of 64,000 vertices."),
                                script.moreThan65kVertices);

                            //Settings for "Meshes To Ignore"
                            if(script.mergeMethod != CombineMethod.OnlyAnima2DMeshes)
                            {
                                SerializedProperty meshesToIgnoreList = serializedObject.FindProperty("meshesToIgnore");
                                listExpanded_meshesToIgnore = EditorGUILayout.Foldout(listExpanded_meshesToIgnore, new GUIContent("Meshes To Ignore (" + script.meshesToIgnore.Count.ToString() + ")", "Add here meshes that you do not want combined. The meshes in this list are ignored when the Skinned Mesh Combiner is merging."));
                                if (listExpanded_meshesToIgnore == true)
                                {
                                    if (script.meshesToIgnore.Count == 0)
                                    {
                                        EditorGUILayout.HelpBox("Oops! No mesh was registered to be ignored! If you want to subscribe any, click the button below!", MessageType.Info);
                                    }
                                    if (script.meshesToIgnore.Count > 0)
                                    {
                                        for (int i = 0; i < script.meshesToIgnore.Count; i++)
                                        {
                                            GUILayout.BeginHorizontal();
                                            if (GUILayout.Button("-", GUILayout.Width(25), GUILayout.Height(15)))
                                            {
                                                script.meshesToIgnore.RemoveAt(i);
                                            }
                                            EditorGUILayout.ObjectField(meshesToIgnoreList.GetArrayElementAtIndex(i), new GUIContent("Skinned Mesh " + i.ToString(), "This mesh will be ignored in the merge. Click the button to the left if you want to remove this mesh from the list."));
                                            GUILayout.EndHorizontal();
                                        }
                                    }
                                    GUILayout.Space(10);
                                    GUILayout.BeginHorizontal();
                                    if (GUILayout.Button("Add New Slot"))
                                    {
                                        script.meshesToIgnore.Add(null);
                                    }
                                    if (script.meshesToIgnore.Count > 0)
                                    {
                                        if (GUILayout.Button("Remove Empty Slots", GUILayout.Width(Screen.width * 0.30f)))
                                        {
                                            for (int i = script.meshesToIgnore.Count - 1; i >= 0; i--)
                                            {
                                                if (script.meshesToIgnore[i] == null)
                                                {
                                                    script.meshesToIgnore.RemoveAt(i);
                                                }
                                            }
                                        }
                                    }
                                    GUILayout.EndHorizontal();
                                }
                                serializedObject.ApplyModifiedProperties();
                            }

                            //Settings for "Meshes To Ignore" to Anima2D merge method
                            if (script.mergeMethod == CombineMethod.OnlyAnima2DMeshes)
                            {
#if MTAssets_Anima2D_Available
                                SerializedProperty meshesToIgnoreList = serializedObject.FindProperty("meshesToIgnoreOfAnima2dMergeMethod");
                                listExpanded_meshesToIgnore = EditorGUILayout.Foldout(listExpanded_meshesToIgnore, new GUIContent("Meshes To Ignore (" + script.meshesToIgnoreOfAnima2dMergeMethod.Count.ToString() + ")", "Add here meshes that you do not want combined. The meshes in this list are ignored when the Skinned Mesh Combiner is merging."));
                                if (listExpanded_meshesToIgnore == true)
                                {
                                    if (script.meshesToIgnoreOfAnima2dMergeMethod.Count == 0)
                                    {
                                        EditorGUILayout.HelpBox("Oops! No sprite mesh instance was registered to be ignored! If you want to subscribe any, click the button below!", MessageType.Info);
                                    }
                                    if (script.meshesToIgnoreOfAnima2dMergeMethod.Count > 0)
                                    {
                                        for (int i = 0; i < script.meshesToIgnoreOfAnima2dMergeMethod.Count; i++)
                                        {
                                            GUILayout.BeginHorizontal();
                                            if (GUILayout.Button("-", GUILayout.Width(25), GUILayout.Height(15)))
                                            {
                                                script.meshesToIgnoreOfAnima2dMergeMethod.RemoveAt(i);
                                            }
                                            EditorGUILayout.ObjectField(meshesToIgnoreList.GetArrayElementAtIndex(i), new GUIContent("Sprite Mesh " + i.ToString(), "This sprite mesh instance will be ignored in the merge. Click the button to the left if you want to remove this mesh from the list."));
                                            GUILayout.EndHorizontal();
                                        }
                                    }
                                    GUILayout.Space(10);
                                    GUILayout.BeginHorizontal();
                                    if (GUILayout.Button("Add New Slot"))
                                    {
                                        script.meshesToIgnoreOfAnima2dMergeMethod.Add(null);
                                    }
                                    if (script.meshesToIgnoreOfAnima2dMergeMethod.Count > 0)
                                    {
                                        if (GUILayout.Button("Remove Empty Slots", GUILayout.Width(Screen.width * 0.30f)))
                                        {
                                            for (int i = script.meshesToIgnoreOfAnima2dMergeMethod.Count - 1; i >= 0; i--)
                                            {
                                                if (script.meshesToIgnoreOfAnima2dMergeMethod[i] == null)
                                                {
                                                    script.meshesToIgnoreOfAnima2dMergeMethod.RemoveAt(i);
                                                }
                                            }
                                        }
                                    }
                                    GUILayout.EndHorizontal();
                                }
                                serializedObject.ApplyModifiedProperties();
#endif
                            }

                            GUILayout.Space(10);

                            //Settings for Additional Effects (Only shows if merge method is all in one)
                            if(script.mergeMethod == CombineMethod.AllInOne)
                            {
                                EditorGUILayout.LabelField("Settings For Additional Effects", EditorStyles.boldLabel);

                                if (script.enableAdvancedAtlas == false)
                                {
                                    EditorGUILayout.HelpBox("To enable support for additional effects, such as Normal Maps, Height Maps, and others, enable \"Enable Advanced Atlas\". You can configure support for these effects here.", MessageType.Info);
                                    script.normalMapSupport = false;
                                    script.secondNormalMapSupport = false;
                                    script.heightMapSupport = false;
                                    script.occlusionMapSupport = false;
                                    script.detailAlbedoMapSupport = false;
                                    script.metallicMapSupport = false;
                                    script.specularMapSupport = false;
                                }
                                if (script.enableAdvancedAtlas == true)
                                {
                                    if (script.materialType == MaterialType.InternalMaterial)
                                    {
                                        if (script.internalMaterialList != ListInternalMaterial.StandardMetallic && script.internalMaterialList != ListInternalMaterial.StandardMetallicCullingOff &&
                                            script.internalMaterialList != ListInternalMaterial.StandardSpecular && script.internalMaterialList != ListInternalMaterial.StandardSpecularCullingOff)
                                        {
                                            script.internalMaterialList = ListInternalMaterial.StandardMetallicCullingOff;
                                        }
                                    }

                                    if (script.metallicMapSupport == true && script.specularMapSupport == true)
                                    {
                                        EditorGUILayout.HelpBox("Consider leaving only the \"Normal Map\" or \"Specular Map\" enabled. If you leave both enabled, you may have artifacts in the final shader.", MessageType.Warning);
                                    }

                                    script.metallicMapSupport = (bool)EditorGUILayout.Toggle(new GUIContent("Metallic Map Support",
                                    "If this option is enabled, the Skinned Mesh Combiner will try to look up Mettalic Map textures in each material of this model and combine them as well.\n\nYou will also need to provide the name of the property on which the shaders save the metallic texture map in \"Metallic Map Settings\". Usually the most used name is \"_MetallicGlossMap\".\n\nKeep in mind that this function can increase the time taken to process the merge."),
                                    script.metallicMapSupport);

                                    script.specularMapSupport = (bool)EditorGUILayout.Toggle(new GUIContent("Specular Map Support",
                                    "If this option is enabled, the Skinned Mesh Combiner will try to look up Specular Map textures in each material of this model and combine them as well.\n\nYou will also need to provide the name of the property on which the shaders save the specular texture map in \"Specular Map Settings\". Usually the most used name is \"_SpecGlossMap\".\n\nKeep in mind that this function can increase the time taken to process the merge."),
                                    script.specularMapSupport);

                                    script.normalMapSupport = (bool)EditorGUILayout.Toggle(new GUIContent("Normal Map Support",
                                    "If this option is enabled, the Skinned Mesh Combiner will try to look up Normal Map textures in each material of this model and combine them as well.\n\nYou will also need to provide the name of the property on which the shaders save the normal texture map in \"Normal Map Settings\". Usually the most used name is \"_BumpMap\".\n\nKeep in mind that this function can increase the time taken to process the merge."),
                                    script.normalMapSupport);

                                    script.secondNormalMapSupport = (bool)EditorGUILayout.Toggle(new GUIContent("Normal Map 2x Support",
                                    "If this option is enabled, the Skinned Mesh Combiner will try to look up 2x Normal Map textures in each material of this model and combine them as well.\n\nYou will also need to provide the name of the property on which the shaders save the second normal texture map in \"Normal Map 2x Settings\". Usually the most used name is \"_DetailNormalMap\".\n\nKeep in mind that this function can increase the time taken to process the merge."),
                                    script.secondNormalMapSupport);

                                    if (script.atlasMipMap == true)
                                    {
                                        script.heightMapSupport = (bool)EditorGUILayout.Toggle(new GUIContent("Height Map Support",
                                        "If this option is enabled, the Skinned Mesh Combiner will try to look up Height Map textures in each material of this model and combine them as well.\n\nYou will also need to provide the name of the property on which the shaders save the height map texture in \"Height Map Settings\". Usually the most used name is \"_ParallaxMap\".\n\nKeep in mind that this function can increase the time taken to process the merge."),
                                        script.heightMapSupport);
                                    }

                                    script.occlusionMapSupport = (bool)EditorGUILayout.Toggle(new GUIContent("Occlusion Map Support",
                                    "If this option is enabled, the Skinned Mesh Combiner will try to look up Occlusion Map textures in each material of this model and combine them as well.\n\nYou will also need to provide the name of the property on which the shaders save the occlusion texture map in \"Occlusion Map Settings\". Usually the most used name is \"_OcclusionMap\".\n\nKeep in mind that this function can increase the time taken to process the merge."),
                                    script.occlusionMapSupport);

                                    script.detailAlbedoMapSupport = (bool)EditorGUILayout.Toggle(new GUIContent("Detail Albedo Map Support",
                                    "If this option is enabled, the Skinned Mesh Combiner will try to look up Detail Albedo Map textures in each material of this model and combine them as well.\n\nYou will also need to provide the name of the property on which the shaders save the detail albedo texture map in \"Detail Albedo Map Settings\". Usually the most used name is \"_DetailAlbedoMap\".\n\nKeep in mind that this function can increase the time taken to process the merge."),
                                    script.detailAlbedoMapSupport);

                                    if (script.metallicMapSupport == true)
                                    {
                                        listExpanded_metallicMapSupport = EditorGUILayout.Foldout(listExpanded_metallicMapSupport, new GUIContent("Metallic Map Settings",
                                            "Configure the Metallic Map support properties here."));
                                        if (listExpanded_metallicMapSupport == true)
                                        {
                                            EditorGUI.indentLevel += 1;

                                            script.metallicMapPropertyNameFind = EditorGUILayout.TextField(new GUIContent("Property Name Find",
                                            "The name of the shader property, which is responsible for storing the metallic map texture, in the material of its meshes. The Skinned Mesh Combiner will use the property here reported to fetch the metallic texture map on each material in each mesh. Usually the name used by most shaders is \"_MetallicGlossMap\", but if any of your shaders have a different name, you can enter it here.\n\nIf the Skinned Mesh Combiner can not find the metallic map in the mesh material, it will be without metallic map after merging."),
                                            script.metallicMapPropertyNameFind);

                                            script.metallicMapPropertyNameInsert = EditorGUILayout.TextField(new GUIContent("Property Name Apply",
                                            "The name of the shader property, which will be responsible for storing the texture of metallic map atlas, in the COMBINED mesh material. The Skinned Mesh Combiner will use the property here informed to apply the metallic atlas map texture in the final material after the merge. Normally the name used by most shaders (including the standard pre-built shader) is \"_MetallicGlossMap\", but if you have defined a custom shader and it has a different property name, you can enter it here.\n\nIf the Skinned Mesh Combiner can not find the metallic map in the final material of the mesh merge, it will be without metallic map after merging."),
                                            script.metallicMapPropertyNameInsert);

                                            EditorGUI.indentLevel -= 1;
                                        }
                                    }

                                    if (script.specularMapSupport == true)
                                    {
                                        listExpanded_specularMapSupport = EditorGUILayout.Foldout(listExpanded_specularMapSupport, new GUIContent("Specular Map Settings",
                                            "Configure the Specular Map support properties here."));
                                        if (listExpanded_specularMapSupport == true)
                                        {
                                            EditorGUI.indentLevel += 1;

                                            script.specularMapPropertyNameFind = EditorGUILayout.TextField(new GUIContent("Property Name Find",
                                            "The name of the shader property, which is responsible for storing the specular map texture, in the material of its meshes. The Skinned Mesh Combiner will use the property here reported to fetch the specular texture map on each material in each mesh. Usually the name used by most shaders is \"_SpecGlossMap\", but if any of your shaders have a different name, you can enter it here.\n\nIf the Skinned Mesh Combiner can not find the specular map in the mesh material, it will be without specular map after merging."),
                                            script.specularMapPropertyNameFind);

                                            script.specularMapPropertyNameInsert = EditorGUILayout.TextField(new GUIContent("Property Name Apply",
                                            "The name of the shader property, which will be responsible for storing the texture of specular map atlas, in the COMBINED mesh material. The Skinned Mesh Combiner will use the property here informed to apply the specular atlas map texture in the final material after the merge. Normally the name used by most shaders (including the standard pre-built shader) is \"_SpecGlossMap\", but if you have defined a custom shader and it has a different property name, you can enter it here.\n\nIf the Skinned Mesh Combiner can not find the specular map in the final material of the mesh merge, it will be without specular map after merging."),
                                            script.specularMapPropertyNameInsert);

                                            EditorGUI.indentLevel -= 1;
                                        }
                                    }

                                    if (script.normalMapSupport == true)
                                    {
                                        listExpanded_normalMapSupport = EditorGUILayout.Foldout(listExpanded_normalMapSupport, new GUIContent("Normal Map Settings",
                                            "Configure the Normal Maps support properties here."));
                                        if (listExpanded_normalMapSupport == true)
                                        {
                                            EditorGUI.indentLevel += 1;

                                            script.normalMapPropertyNameFind = EditorGUILayout.TextField(new GUIContent("Property Name Find",
                                            "The name of the shader property, which is responsible for storing the normal map texture, in the material of its meshes. The Skinned Mesh Combiner will use the property here reported to fetch the normal texture map on each material in each mesh. Usually the name used by most shaders is \"_BumpMap\", but if any of your shaders have a different name, you can enter it here.\n\nIf the Skinned Mesh Combiner can not find the normal map in the mesh material, it will be without normal map after merging."),
                                            script.normalMapPropertyNameFind);

                                            script.normalMapPropertyNameInsert = EditorGUILayout.TextField(new GUIContent("Property Name Apply",
                                            "The name of the shader property, which will be responsible for storing the texture of normal map atlas, in the COMBINED mesh material. The Skinned Mesh Combiner will use the property here informed to apply the normal atlas map texture in the final material after the merge. Normally the name used by most shaders (including the standard pre-built shader) is \"_BumpMap\", but if you have defined a custom shader and it has a different property name, you can enter it here.\n\nIf the Skinned Mesh Combiner can not find the normal map in the final material of the mesh merge, it will be without normal map after merging."),
                                            script.normalMapPropertyNameInsert);

                                            script.applyNormalMapScale = (bool)EditorGUILayout.Toggle(new GUIContent("Apply Scale",
                                            "If this option is enabled, the Skinned Mesh Combiner attempts to scale the normal map texture of the final material. You will need to enter the name of the property that stores the normal map scale in the final material. If you are using a pre-created Skinned Mesh Combiner shader, leave the property name as it is."),
                                            script.applyNormalMapScale);

                                            if (script.applyNormalMapScale == true)
                                            {
                                                EditorGUI.indentLevel += 1;

                                                script.normalMapScalePropertyName = EditorGUILayout.TextField(new GUIContent("Property Name",
                                                "The name of the property that stores the scale of the normal map in the merge material."),
                                                script.normalMapScalePropertyName);

                                                script.normalMapScale = EditorGUILayout.FloatField(new GUIContent("Scale",
                                                "Scale the normal map in the final merge material."),
                                                script.normalMapScale);

                                                EditorGUI.indentLevel -= 1;
                                            }

                                            EditorGUI.indentLevel -= 1;
                                        }
                                    }

                                    if (script.secondNormalMapSupport == true)
                                    {
                                        listExpanded_normalMapSecondarySupport = EditorGUILayout.Foldout(listExpanded_normalMapSecondarySupport, new GUIContent("Normal Map 2x Settings",
                                            "Configure the Secondary Normal Map support properties here."));
                                        if (listExpanded_normalMapSecondarySupport == true)
                                        {
                                            EditorGUI.indentLevel += 1;

                                            script.secondNormalMapPropertyNameFind = EditorGUILayout.TextField(new GUIContent("Property Name Find",
                                            "The name of the shader property, which is responsible for storing the second normal map texture, in the material of its meshes. The Skinned Mesh Combiner will use the property here reported to fetch the second normal texture map on each material in each mesh. Usually the name used by most shaders is \"_DetailNormalMap\", but if any of your shaders have a different name, you can enter it here.\n\nIf the Skinned Mesh Combiner can not find the second normal map in the mesh material, it will be without second normal map after merging."),
                                            script.secondNormalMapPropertyNameFind);

                                            script.secondNormalMapPropertyNameInsert = EditorGUILayout.TextField(new GUIContent("Property Name Apply",
                                            "The name of the shader property, which will be responsible for storing the texture of second normal map atlas, in the COMBINED mesh material. The Skinned Mesh Combiner will use the property here informed to apply the second normal atlas map texture in the final material after the merge. Normally the name used by most shaders (including the standard pre-built shader) is \"_DetailNormalMap\", but if you have defined a custom shader and it has a different property name, you can enter it here.\n\nIf the Skinned Mesh Combiner can not find the second normal map in the final material of the mesh merge, it will be without second normal map after merging."),
                                            script.secondNormalMapPropertyNameInsert);

                                            script.applySecondNormalMapScale = (bool)EditorGUILayout.Toggle(new GUIContent("Apply Scale",
                                            "If this option is enabled, the Skinned Mesh Combiner attempts to scale the second normal map texture of the final material. You will need to enter the name of the property that stores the second normal map scale in the final material. If you are using a pre-created Skinned Mesh Combiner shader, leave the property name as it is."),
                                            script.applySecondNormalMapScale);

                                            if (script.applySecondNormalMapScale == true)
                                            {
                                                EditorGUI.indentLevel += 1;

                                                script.secondNormalMapScalePropertyName = EditorGUILayout.TextField(new GUIContent("Property Name",
                                                "The name of the property that stores the scale of the second normal map in the merge material."),
                                                script.secondNormalMapScalePropertyName);

                                                script.secondNormalMapScale = EditorGUILayout.FloatField(new GUIContent("Scale",
                                                "Scale the second normal map in the final merge material."),
                                                script.secondNormalMapScale);

                                                EditorGUI.indentLevel -= 1;
                                            }

                                            EditorGUI.indentLevel -= 1;
                                        }
                                    }

                                    if (script.atlasMipMap == true)
                                    {
                                        if (script.heightMapSupport == true)
                                        {
                                            listExpanded_heightMapSupport = EditorGUILayout.Foldout(listExpanded_heightMapSupport, new GUIContent("Height Map Settings",
                                            "Configure the Height Map support properties here."));
                                            if (listExpanded_heightMapSupport == true)
                                            {
                                                EditorGUI.indentLevel += 1;

                                                script.heightMapPropertyNameFind = EditorGUILayout.TextField(new GUIContent("Property Name Find",
                                                "The name of the shader property, which is responsible for storing the height map texture, in the material of its meshes. The Skinned Mesh Combiner will use the property here reported to fetch the height texture map on each material in each mesh. Usually the name used by most shaders is \"_ParallaxMap\", but if any of your shaders have a different name, you can enter it here.\n\nIf the Skinned Mesh Combiner can not find the height map in the mesh material, it will be without height map after merging."),
                                                script.heightMapPropertyNameFind);

                                                script.heightMapPropertyNameInsert = EditorGUILayout.TextField(new GUIContent("Property Name Apply",
                                                "The name of the shader property, which will be responsible for storing the texture of height map atlas, in the COMBINED mesh material. The Skinned Mesh Combiner will use the property here informed to apply the height atlas map texture in the final material after the merge. Normally the name used by most shaders (including the standard pre-built shader) is \"_ParallaxMap\", but if you have defined a custom shader and it has a different property name, you can enter it here.\n\nIf the Skinned Mesh Combiner can not find the height map in the final material of the mesh merge, it will be without height map after merging."),
                                                script.heightMapPropertyNameInsert);

                                                script.applyHeightMapScale = (bool)EditorGUILayout.Toggle(new GUIContent("Apply Scale",
                                                "If this option is enabled, the Skinned Mesh Combiner attempts to scale the height map texture of the final material. You will need to enter the name of the property that stores the height map scale in the final material. If you are using a pre-created Skinned Mesh Combiner shader, leave the property name as it is."),
                                                script.applyHeightMapScale);

                                                if (script.applyHeightMapScale == true)
                                                {
                                                    EditorGUI.indentLevel += 1;

                                                    script.heightMapScalePropertyName = EditorGUILayout.TextField(new GUIContent("Property Name",
                                                    "The name of the property that stores the scale of the height map in the merge material."),
                                                    script.heightMapScalePropertyName);

                                                    script.heightMapScale = EditorGUILayout.Slider("Scale", script.heightMapScale, 0.005f, 0.08f);

                                                    EditorGUI.indentLevel -= 1;
                                                }

                                                EditorGUI.indentLevel -= 1;
                                            }
                                        }
                                    }

                                    if (script.occlusionMapSupport == true)
                                    {
                                        listExpanded_occlusionMapSupport = EditorGUILayout.Foldout(listExpanded_occlusionMapSupport, new GUIContent("Occlusion Map Settings",
                                            "Configure the Occlusion Map support properties here."));
                                        if (listExpanded_occlusionMapSupport == true)
                                        {
                                            EditorGUI.indentLevel += 1;

                                            script.occlusionMapPropertyNameFind = EditorGUILayout.TextField(new GUIContent("Property Name Find",
                                            "The name of the shader property, which is responsible for storing the occlusion map texture, in the material of its meshes. The Skinned Mesh Combiner will use the property here reported to fetch the occlusion texture map on each material in each mesh. Usually the name used by most shaders is \"_OcclusionMap\", but if any of your shaders have a different name, you can enter it here.\n\nIf the Skinned Mesh Combiner can not find the occlusion map in the mesh material, it will be without occlusion map after merging."),
                                            script.occlusionMapPropertyNameFind);

                                            script.occlusionMapPropertyNameInsert = EditorGUILayout.TextField(new GUIContent("Property Name Apply",
                                            "The name of the shader property, which will be responsible for storing the texture of occlusion map atlas, in the COMBINED mesh material. The Skinned Mesh Combiner will use the property here informed to apply the occlusion atlas map texture in the final material after the merge. Normally the name used by most shaders (including the standard pre-built shader) is \"_OcclusionMap\", but if you have defined a custom shader and it has a different property name, you can enter it here.\n\nIf the Skinned Mesh Combiner can not find the occlusion map in the final material of the mesh merge, it will be without occlusion map after merging."),
                                            script.occlusionMapPropertyNameInsert);

                                            script.applyOcclusionMapScale = (bool)EditorGUILayout.Toggle(new GUIContent("Apply Scale",
                                            "If this option is enabled, the Skinned Mesh Combiner attempts to scale the occlusion map texture of the final material. You will need to enter the name of the property that stores the occlusion map scale in the final material. If you are using a pre-created Skinned Mesh Combiner shader, leave the property name as it is."),
                                            script.applyOcclusionMapScale);

                                            if (script.applyOcclusionMapScale == true)
                                            {
                                                EditorGUI.indentLevel += 1;

                                                script.occlusionMapScalePropertyName = EditorGUILayout.TextField(new GUIContent("Property Name",
                                                "The name of the property that stores the scale of the occlusion map in the merge material."),
                                                script.occlusionMapScalePropertyName);

                                                script.occlusionMapScale = EditorGUILayout.Slider("Scale", script.occlusionMapScale, 0f, 1f);

                                                EditorGUI.indentLevel -= 1;
                                            }

                                            EditorGUI.indentLevel -= 1;
                                        }
                                    }

                                    if (script.detailAlbedoMapSupport == true)
                                    {
                                        listExpanded_detailAlbedoMapSupport = EditorGUILayout.Foldout(listExpanded_detailAlbedoMapSupport, new GUIContent("Detail Albedo Map Settings",
                                            "Configure the Detail Albedo Map support properties here."));
                                        if (listExpanded_detailAlbedoMapSupport == true)
                                        {
                                            EditorGUI.indentLevel += 1;

                                            script.detailAlbedoMapPropertyNameFind = EditorGUILayout.TextField(new GUIContent("Property Name Find",
                                            "The name of the shader property, which is responsible for storing the detail albedo map texture, in the material of its meshes. The Skinned Mesh Combiner will use the property here reported to fetch the detail albedo texture map on each material in each mesh. Usually the name used by most shaders is \"_DetailAlbedoMap\", but if any of your shaders have a different name, you can enter it here.\n\nIf the Skinned Mesh Combiner can not find the detail albedo map in the mesh material, it will be without detail albedo map after merging."),
                                            script.detailAlbedoMapPropertyNameFind);

                                            script.detailAlbedoMapPropertyNameInsert = EditorGUILayout.TextField(new GUIContent("Property Name Apply",
                                            "The name of the shader property, which will be responsible for storing the texture of detail albedo map atlas, in the COMBINED mesh material. The Skinned Mesh Combiner will use the property here informed to apply the detail albedo atlas map texture in the final material after the merge. Normally the name used by most shaders (including the standard pre-built shader) is \"_DetailAlbedoMap\", but if you have defined a custom shader and it has a different property name, you can enter it here.\n\nIf the Skinned Mesh Combiner can not find the detail albedo map in the final material of the mesh merge, it will be without detail albedo map after merging."),
                                            script.detailAlbedoMapPropertyNameInsert);

                                            EditorGUI.indentLevel -= 1;
                                        }
                                    }
                                }
                                GUILayout.Space(10);
                            }

                            //Settings for "Combine In Editor"
                            EditorGUILayout.LabelField("Settings For Combiner In Editor", EditorStyles.boldLabel);

                            if(Application.isPlaying == false)
                            {
                                script.saveDataInAssets = (bool)EditorGUILayout.Toggle(new GUIContent("Save Data In Assets",
                                "** Only works in the Inspector. **\n\nAfter combining the meshes, save the final mesh data in your project files."),
                                script.saveDataInAssets);

                                if (script.saveDataInAssets == true && script.mergeMethod == CombineMethod.AllInOne || script.mergeMethod == CombineMethod.JustMaterialColors || script.mergeMethod == CombineMethod.OnlyAnima2DMeshes)
                                {
                                    script.exportTexturesAsPng = EditorGUILayout.Toggle(new GUIContent("Export Textures As PNG",
                                                "Save textures of merge, as PNG in project files?"),
                                                script.exportTexturesAsPng);

                                    EditorGUI.indentLevel += 1;

                                    if(script.exportTexturesAsPng == true)
                                    {
                                        script.nameOfTextures = EditorGUILayout.TextField(new GUIContent("Name Of Files",
                                                "The name of exported textures.\n\nThe textures will be saved in the \"_Exported\" folder in your project files. If there are textures with the same name, the old ones will be overwritten."),
                                                script.nameOfTextures);
                                    }

                                    EditorGUI.indentLevel -= 1;
                                }

                                script.savePrefabOfThis = (bool)EditorGUILayout.Toggle(new GUIContent("Save Prefab Of This",
                                    "** Only works in the Inspector. **\n\nAfter combining the meshes, save a prefab of this GameObject in your project files.\n\nYou must set a name for the prefab to be saved. If a prefab with this same name already exists, it will not be overwritten. So you can update it from this GameObject.\n\nWhen you choose to save a prefab, the data should automatically be saved as well."),
                                    script.savePrefabOfThis);

                                if (script.savePrefabOfThis == true)
                                {
                                    script.saveDataInAssets = true;

                                    if (script.alreadySettedFirstName == false)
                                    {
                                        script.prefabName = script.gameObject.name + " (Prefab)";
                                        script.alreadySettedFirstName = true;
                                    }

                                    EditorGUI.indentLevel += 1;

                                    script.prefabName = EditorGUILayout.TextField(new GUIContent("Prefab Name",
                                                "The name for the prefab to be saved."),
                                                script.prefabName);

                                    EditorGUI.indentLevel -= 1;
                                }
                            }
                            if(Application.isPlaying == true)
                            {
                                EditorGUILayout.HelpBox("These settings are not available while your game is running. Only in edit mode.", MessageType.Info);
                            }

                            GUILayout.Space(10);

                            //Settings for "Optimization On Undo Merge"
                            EditorGUILayout.LabelField("Optimization On Undo Merge", EditorStyles.boldLabel);

                            script.enableMonoGC = (bool)EditorGUILayout.Toggle(new GUIContent("Run Mono/IL2CPP GC",
                                "If this option is enabled, the Mono/IL2CPP garbage collector will run whenever the merge is undone. This will exclude unused resources from memory, preventing your game from consuming many device features.\n\nNote that this function may cause brief freezes in the game. Use it carefully."),
                                script.enableMonoGC);

                            script.enableUnityGC = (bool)EditorGUILayout.Toggle(new GUIContent("Run Unity GC",
                                "If this option is enabled, the Unity garbage collector will run whenever the merge is undone. This will exclude unused resources from memory, preventing your game from consuming many device features.\n\nNote that this function may cause brief game freezes, or a small loss of FPS, depending on the amount of resources to be deleted."),
                                script.enableUnityGC);

                            GUILayout.Space(10);

                            //Settings for "Resulting Merged Mesh Settings"
                            EditorGUILayout.LabelField("Resulting Merged Mesh Settings", EditorStyles.boldLabel);

                            script.qualityOfAnim = (AnimQuality)EditorGUILayout.EnumPopup(new GUIContent("Quality of Anim",
                                "This option configures the quality of the merge fine mesh animations. The higher the quality, the more vertices the bones will use to animate the mesh."),
                                script.qualityOfAnim);

                            script.updateWhenOffScreen = (bool)EditorGUILayout.Toggle(new GUIContent("Update When Off Screen",
                                "Unity will still continue to calculate the meshed meshes even when they are off the camera."),
                                script.updateWhenOffScreen);

                            script.skinnedMotionVectors = (bool)EditorGUILayout.Toggle(new GUIContent("Skinned Motion Vectors",
                                "Unity will use a double buffer to calculate the mesh on the GPU. This uses 2x more memory, but has more accurate motion vectors."),
                                script.skinnedMotionVectors);

                            GUILayout.Space(10);

                            //Settings for "Debugging"
                            EditorGUILayout.LabelField("Debugging", EditorStyles.boldLabel);

                            script.enableDebugging = (bool)EditorGUILayout.Toggle(new GUIContent("Enable Debug Logs",
                                "If debug mode is active, this component will display error messages whenever it encounter any problems during the merge. While you are developing this is fine, but if you are going to release a public version of your game, it is interesting to turn off this option to prevent log messages from taking up game memory."),
                                script.enableDebugging);

                            if (script.mergeMethod == CombineMethod.AllInOne || script.mergeMethod == CombineMethod.JustMaterialColors || script.mergeMethod == CombineMethod.OnlyAnima2DMeshes)
                            {
                                script.showUvVerticesInAtlas = (bool)EditorGUILayout.Toggle(new GUIContent("Show Atlas UV Vertices",
                                "If this option is enabled, after combining the textures in an atlas, the UV map vertices of the combined mesh will be displayed in the atlas by yellow pixels.\n\nKeep in mind that enabling this option will increase processing time when merging meshes."),
                                script.showUvVerticesInAtlas);
                            }

                            GUILayout.Space(10);

                            //Reminder of Debugging enabled
                            if (script.enableDebugging == true)
                            {
                                EditorGUILayout.HelpBox("Tip: The debugging mode is enabled for this component! Consider disabling debugging if you no longer need it. Logs generated by debugging can cause problems with memory consumption! You will still continue to receive the logs from the \"Logs of Merge\" tab here in the Inspector.", MessageType.Warning);
                            }

                            //Show reminder for active "Read/Write" in textures if never combined textures in this GameObject
                            if (script.mergeMethod == CombineMethod.AllInOne && script.numberOfMerges == 0)
                            {
                                EditorGUILayout.HelpBox("Remember to enable the \"Read/Write Enabled\" option in the import settings of the textures that will be merged. This option must be enabled so that textures can be manipulated and the Skinned Mesh Combiner can pack them into an atlas.", MessageType.Info);
                            }
                            if (script.mergeMethod == CombineMethod.OnlyAnima2DMeshes && script.numberOfMerges == 0)
                            {
                                EditorGUILayout.HelpBox("Remember to enable the \"Read/Write Enabled\" option in the import settings of the textures that will be merged. This option must be enabled so that textures can be manipulated and the Skinned Mesh Combiner can pack them into an atlas.", MessageType.Info);
                            }

                            GUILayout.Space(10);

                            //Verify if importante files of merge are missing
                            if (script.saveDataInAssets == true && script.isMeshesCombineds == true)
                            {
                                if (script.mergeMethod == CombineMethod.OneMeshPerMaterial)
                                {
                                    Transform objMerged = script.GetComponent<Transform>().Find("Combined Mesh");
                                    SkinnedMeshRenderer[] skinnedMeshRenderers = null;
                                    if (objMerged != null)
                                    {
                                        skinnedMeshRenderers = objMerged.GetComponentsInChildren<SkinnedMeshRenderer>();
                                    }
                                    bool hasMissingFiles = false;
                                    foreach (SkinnedMeshRenderer mesh in skinnedMeshRenderers)
                                    {
                                        if (mesh.sharedMesh == null)
                                        {
                                            hasMissingFiles = true;
                                        }
                                    }
                                    if (hasMissingFiles == true)
                                    {
                                        EditorGUILayout.HelpBox("\n\nOops, it looks like there are missing files in this model. Try undoing the merge and merge again to resolve this issue!\n\n", MessageType.Error);
                                        GUILayout.Space(10);
                                    }
                                }
                                if (script.mergeMethod == CombineMethod.AllInOne)
                                {
                                    Transform objMerged = script.GetComponent<Transform>().Find("Combined Mesh");
                                    SkinnedMeshRenderer[] skinnedMeshRenderers = null;
                                    if (objMerged != null)
                                    {
                                        skinnedMeshRenderers = objMerged.GetComponentsInChildren<SkinnedMeshRenderer>();
                                    }
                                    bool hasMissingFiles = false;
                                    foreach (SkinnedMeshRenderer mesh in skinnedMeshRenderers)
                                    {
                                        if (mesh.sharedMesh == null)
                                        {
                                            hasMissingFiles = true;
                                        }
                                        if (mesh.sharedMaterials[0] == null)
                                        {
                                            hasMissingFiles = true;
                                        }
                                        if (mesh.sharedMaterials[0] != null && mesh.sharedMaterials[0].mainTexture == null)
                                        {
                                            hasMissingFiles = true;
                                        }
                                    }
                                    if (hasMissingFiles == true)
                                    {
                                        EditorGUILayout.HelpBox("\n\nOops, it looks like there are missing files in this model. Try undoing the merge and merge again to resolve this issue!\n\n", MessageType.Error);
                                        GUILayout.Space(10);
                                    }
                                }
                            }

                            //Operate buttons
                            if (script.isMeshesCombineds == false)
                            {
                                if (GUILayout.Button("Combine Meshes!", GUILayout.Height(40)))
                                {
                                    if (script.GetComponent<Animator>() != null)
                                    {
                                        script.numberOfMerges += 1;

                                        switch (script.mergeMethod)
                                        {
                                            case CombineMethod.OneMeshPerMaterial:
                                                script.StartCoroutine(script.MergeMeshes_PerMaterial(true));
                                                //Change to console logs
                                                script.currentTab = 2;
                                                break;
                                            case CombineMethod.AllInOne:
                                                if (script.materialType != MaterialType.CustomMaterial)
                                                {
                                                    script.StartCoroutine(script.MergeMeshes_AllInOne(true));
                                                    //Change to console logs
                                                    script.currentTab = 2;
                                                }
                                                if (script.materialType == MaterialType.CustomMaterial)
                                                {
                                                    if (script.materialCustom != null)
                                                    {
                                                        script.StartCoroutine(script.MergeMeshes_AllInOne(true));
                                                        //Change to console logs
                                                        script.currentTab = 2;
                                                    }
                                                    if (script.materialCustom == null)
                                                    {
                                                        script.LaunchLog(LogType.Error, "Please add a custom material to combine " + script.transform.name + ". This custom material will be the material that the final atlas will use. The Skinned Mesh Combine can not function without a material.");
                                                    }
                                                }
                                                break;
                                            case CombineMethod.JustMaterialColors:
                                                if (script.materialType != MaterialType.CustomMaterial)
                                                {
                                                    script.StartCoroutine(script.MergeMeshes_JustMaterialColors(true));
                                                    //Change to console logs
                                                    script.currentTab = 2;
                                                }
                                                if (script.materialType == MaterialType.CustomMaterial)
                                                {
                                                    if (script.materialCustom != null)
                                                    {
                                                        script.StartCoroutine(script.MergeMeshes_JustMaterialColors(true));
                                                        //Change to console logs
                                                        script.currentTab = 2;
                                                    }
                                                    if (script.materialCustom == null)
                                                    {
                                                        script.LaunchLog(LogType.Error, "Please add a custom material to combine " + script.transform.name + ". This custom material will be the material that the final atlas will use. The Skinned Mesh Combine can not function without a material.");
                                                    }
                                                }
                                                break;
                                            case CombineMethod.OnlyAnima2DMeshes:
                                                if (script.materialType != MaterialType.CustomMaterial)
                                                {
                                                    script.StartCoroutine(script.MergeMeshes_OnlyAnima2dMeshes(true));
                                                    //Change to console logs
                                                    script.currentTab = 2;
                                                }
                                                if (script.materialType == MaterialType.CustomMaterial)
                                                {
                                                    if (script.materialCustom != null)
                                                    {
                                                        script.StartCoroutine(script.MergeMeshes_OnlyAnima2dMeshes(true));
                                                        //Change to console logs
                                                        script.currentTab = 2;
                                                    }
                                                    if (script.materialCustom == null)
                                                    {
                                                        script.LaunchLog(LogType.Error, "Please add a custom material to combine " + script.transform.name + ". This custom material will be the material that the final meshe will use. The Skinned Mesh Combine can not function without a material.");
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                            if (script.isMeshesCombineds == true)
                            {
                                if (GUILayout.Button("Undo Merge", GUILayout.Height(40)))
                                {
                                    script.StartCoroutine(script.UndoMergeMeshes());
                                }
                            }

                            GUILayout.Space(10);
                            //---------------------------------------- End of "Merge" tab
                            break;

                        case 1:
                            //---------------------------------------- Start of "Stats" tab
                            if (script.isMeshesCombineds == false)
                            {
                                EditorGUILayout.HelpBox("The merge of this model has not yet been made. Please merge to see statistics.", MessageType.Error);
                            }

                            if (script.isMeshesCombineds == true)
                            {
                                EditorGUILayout.HelpBox(script.statsOfMerge, MessageType.Info);

                                GUILayout.Space(10);

                                if (script.mergeMethod == CombineMethod.AllInOne || script.mergeMethod == CombineMethod.JustMaterialColors || script.mergeMethod == CombineMethod.OnlyAnima2DMeshes)
                                {
                                    EditorGUILayout.HelpBox("Below is the atlas generated by the merge. The atlases of normal maps and other effects, use the same UV map of this atlas.", MessageType.Info);
                                    GUILayout.Box(script.atlasMergedInAllInOne, GUILayout.Width(Screen.width - 36), GUILayout.Height(400));
                                }
                            }

                            GUILayout.Space(10);
                            //---------------------------------------- End of "Stats" tab
                            break;
                        case 2:
                            //---------------------------------------- Start of "Logs of Merge" tab

                            EditorGUILayout.BeginVertical("box", GUILayout.Width(Screen.width - 40), GUILayout.Height(400));
                            scrollPosLogs = EditorGUILayout.BeginScrollView(scrollPosLogs);

                            for(int i = script.logs.Count - 1; i >= 0; i--)
                            {
                                if(script.logs[i].logType == LogType.Assert || script.logs[i].logType == LogType.Error || script.logs[i].logType == LogType.Exception)
                                {
                                    EditorGUILayout.HelpBox(script.logs[i].content, MessageType.Error);
                                }
                                if (script.logs[i].logType == LogType.Log)
                                {
                                    EditorGUILayout.HelpBox(script.logs[i].content, MessageType.Info);
                                }
                                if (script.logs[i].logType == LogType.Warning)
                                {
                                    EditorGUILayout.HelpBox(script.logs[i].content, MessageType.Warning);
                                }
                            }
                            EditorGUILayout.EndScrollView();
                            EditorGUILayout.EndVertical();

                            GUILayout.Space(10);

                            EditorGUILayout.BeginHorizontal();
                            script.clearOnUndoMerge = (bool)EditorGUILayout.Toggle(new GUIContent("Clear On Undo Merge",
                                "Clear logs on undo merge?"),
                                script.clearOnUndoMerge);

                            if(script.logs.Count > 0)
                            {
                            if (GUILayout.Button("Clear Logs", GUILayout.Height(20), GUILayout.Width(110)))
                                {
                                    script.logs.Clear();
                                }
                            }

                            EditorGUILayout.EndHorizontal();

                            GUILayout.Space(10);

                            EditorGUILayout.HelpBox("This is the logs tab. On this tab, you can see all the logs that this component throws during the merge. This tab only captures released logs while you combine using the Unity Editor. If you combine during the game, you will only be able to see released logs when \"Enable Debug Logs\" is enabled.", MessageType.Info);

                            GUILayout.Space(10);

                            //---------------------------------------- End of "Logs of Merge" tab
                            break;
                    }
                }

                if (GUI.changed)
                {
                    //Apply changes on script, case is not playing in editor
                    if (!Application.isPlaying)
                    {
                        EditorUtility.SetDirty(script);
                        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(script.gameObject.scene);
                    }
                }
            }

        }
#endif

        public void CombineMeshes()
        {
            //Starts the coroutine to merge meshes
            if (GetComponent<Animator>() != null)
            {
                switch (mergeMethod)
                {
                    case CombineMethod.OneMeshPerMaterial:
                        StartCoroutine(MergeMeshes_PerMaterial(false));
                        break;
                    case CombineMethod.AllInOne:
                        if (materialType != MaterialType.CustomMaterial)
                        {
                            StartCoroutine(MergeMeshes_AllInOne(false));
                        }
                        if (materialType == MaterialType.CustomMaterial)
                        {
                            if (materialCustom != null)
                            {
                                StartCoroutine(MergeMeshes_AllInOne(false));
                            }
                            if (materialCustom == null)
                            {
                                LaunchLog(LogType.Error, "Please add a custom material to combine " + transform.name + ". This custom material will be the material that the final atlas will use. The Skinned Mesh Combine can not function without a material.");
                            }
                        }
                        break;
                    case CombineMethod.JustMaterialColors:
                        if (materialType != MaterialType.CustomMaterial)
                        {
                            StartCoroutine(MergeMeshes_JustMaterialColors(false));
                        }
                        if (materialType == MaterialType.CustomMaterial)
                        {
                            if (materialCustom != null)
                            {
                                StartCoroutine(MergeMeshes_JustMaterialColors(false));
                            }
                            if (materialCustom == null)
                            {
                                LaunchLog(LogType.Error, "Please add a custom material to combine " + transform.name + ". This custom material will be the material that the final atlas will use. The Skinned Mesh Combine can not function without a material.");
                            }
                        }
                        break;
                    case CombineMethod.OnlyAnima2DMeshes:
                        if (materialType != MaterialType.CustomMaterial)
                        {
                            StartCoroutine(MergeMeshes_OnlyAnima2dMeshes(false));
                        }
                        if (materialType == MaterialType.CustomMaterial)
                        {
                            if (materialCustom != null)
                            {
                                StartCoroutine(MergeMeshes_OnlyAnima2dMeshes(false));
                            }
                            if (materialCustom == null)
                            {
                                LaunchLog(LogType.Error, "Please add a custom material to combine " + transform.name + ". This custom material will be the material that the final meshe will use. The Skinned Mesh Combine can not function without a material.");
                            }
                        }
                        break;
                }
            }
        }

        public void UndoCombineMeshes()
        {
            //Starts the coroutine to merge meshes
            if (GetComponent<Animator>() != null)
            {
                StartCoroutine(UndoMergeMeshes());
            }
        }

        private void LaunchLog(LogType logType, string content)
        {
            //Launch log in the log of merge and console of unity, if is desired
            if(enableDebugging == true)
            {
                if(logType == LogType.Assert || logType == LogType.Error || logType == LogType.Exception)
                {
                    Debug.LogError(content + "\n\n");
                }
                if (logType == LogType.Log)
                {
                    Debug.Log(content + "\n\n");
                }
                if (logType == LogType.Warning)
                {
                    Debug.LogWarning(content + "\n\n");
                }
            }

#if UNITY_EDITOR
            DateTime dateTime = new DateTime();
            dateTime = DateTime.Now;
            string month = (dateTime.Month >= 10) ? dateTime.Month.ToString() : "0" + dateTime.Month.ToString();
            string day = (dateTime.Day >= 10) ? dateTime.Day.ToString() : "0" + dateTime.Day.ToString();
            string hour = (dateTime.Hour >= 10) ? dateTime.Hour.ToString() : "0" + dateTime.Hour.ToString();
            string minute = (dateTime.Minute >= 10) ? dateTime.Minute.ToString() : "0" + dateTime.Minute.ToString();

            logs.Add(new LogOfSMC(content + "\n\n" + month + "/" + day + "/" + dateTime.Year + " " + hour + ":" + minute, logType));
#endif
        }

        private IEnumerator MergeMeshes_PerMaterial(bool isEditor)
        {
            //Checks if the meshes are already combined.
            if (isMeshesCombineds == true)
            {
                LaunchLog(LogType.Error, "The \"" + this.transform.name + "\" meshes are already combined!");
            }
            if (isMeshesCombineds == false)
            {
#if UNITY_EDITOR
                //Time stats
                System.Diagnostics.Stopwatch timeMonitor = new System.Diagnostics.Stopwatch();
                timeMonitor.Start();
#endif

                //Find submeshes and meshes for merge
                FindMeshes();
                if (ExistsMeshesToMerge() == true)
                {
                    //Meshes and submeshes lists, per material
                    Dictionary<Material, List<SkinnedMeshRenderer>> meshesPerMaterial = new Dictionary<Material, List<SkinnedMeshRenderer>>();

                    //Gets the meshes and submeshes that are to be merged
                    for (int i = 0; i < meshesToMerge.Length; i++)
                    {
                        //Valid the mesh
                        if (ValidMesh(meshesToMerge[i]) == false)
                        {
                            meshesToMerge[i] = null;
                            continue;
                        }

                        //Store the current sub-mesh or mesh according to your material
                        for (int ii = 0; ii < meshesToMerge[i].sharedMesh.subMeshCount; ii++)
                        {
                            if (meshesPerMaterial.ContainsKey(meshesToMerge[i].sharedMaterials[ii]) == true)
                            {
                                meshesPerMaterial[meshesToMerge[i].sharedMaterials[ii]].Add(meshesToMerge[i]);
                            }
                            if (meshesPerMaterial.ContainsKey(meshesToMerge[i].sharedMaterials[ii]) == false)
                            {
                                meshesPerMaterial.Add(meshesToMerge[i].sharedMaterials[ii], new List<SkinnedMeshRenderer>() { meshesToMerge[i] });
                            }
                        }
                    }

                    bool canContinue = true;
                    //Checks whether the resulting mesh will have more than 65500 vertices, if support for more than 65k vertices is disabled
                    if(moreThan65kVertices == false)
                    {
                        foreach (var record in meshesPerMaterial)
                        {
                            if (MeshesMoreThan65kVertices(record.Value.ToArray()) == true)
                            {
                                canContinue = false;
                                break;
                            }
                        }
                    }
                    //Check if there are different root bones, if desired.
                    if(mergeOnlyEqualsRootBones == true)
                    {
                        Transform lastRootBone = null;
                        //Get reference Root Bone
                        foreach (var record in meshesPerMaterial)
                        {
                            lastRootBone = record.Value[0].rootBone;
                        }
                        //Verify if root bones is equal to reference
                        foreach (var record in meshesPerMaterial)
                        {
                            foreach(var skinnedMesh in record.Value)
                            {
                                if(skinnedMesh.rootBone != lastRootBone)
                                {
                                    LaunchLog(LogType.Error, "The GameObject \"" + this.gameObject.name + "\" merge was canceled, because there are one or more meshes with different root bones." +
                                        " This can cause mesh deformations during merge. The mesh with different bone that has been detected is \"" + skinnedMesh.transform.name + "\". You might consider adding it to the list of meshes to ignore." +
                                        " If you prefer you can force the Skinned Mesh Combiner to combine it by unchecking the \"Only Equal Root Bones\" option. A great way to fix this problem is to use a modeling program and re-implement the skeleton" +
                                        " of your model so that all meshes have the same root bone.");
                                    canContinue = false;
                                    break;
                                }
                            }
                        }
                    }

                    if (canContinue == true)
                    {
                        //Create a GameObject to store merged meshes
                        GameObject combinedMeshesOBJ = new GameObject("Combined Mesh");
                        combinedMeshesOBJ.transform.SetParent(this.transform);
#if UNITY_EDITOR
                        //Stats of merged meshes
                        int mergedMeshes = 0;
                        int drawCallReduction = 0;
                        int skinnedMeshsBefore = 0;
                        int processedVertices = 0;
#endif
                        //Configure index of material
                        int materialCount = 0;
                        //Starts the configuration and data split process
                        foreach (var record in meshesPerMaterial)
                        {
                            //Get the meshes or submeshes that will be combined, from the current material
                            List<SkinnedMeshRenderer> meshToCombine = record.Value;

#if UNITY_EDITOR
                            //Get stats of merged meshes
                            int meshesLenght = meshToCombine.ToArray().Length;
                            skinnedMeshsBefore += meshesLenght;
                            if (meshesLenght > 1)
                            {
                                mergedMeshes += meshesLenght;
                                drawCallReduction += meshesLenght - 1;
                            }
#endif

                            List<Transform> bonesToMerge = new List<Transform>();
                            List<Matrix4x4> bindPosesToMerge = new List<Matrix4x4>();
                            List<CombineInstance> combinesToMerge = new List<CombineInstance>();

                            //Obtains the data of each mesh or submesh in this register
                            for (int i = 0; i < meshToCombine.Count; i++)
                            {
                                SkinnedMeshRenderer currentMesh = meshToCombine[i];
#if UNITY_EDITOR
                                //Get stats of merged meshes
                                processedVertices += currentMesh.sharedMesh.vertexCount;
#endif
                                for (int ii = 0; ii < currentMesh.sharedMesh.subMeshCount; ii++)
                                {
                                    if (currentMesh.sharedMaterials[ii].name != record.Key.name)
                                    {
                                        continue;
                                    }

                                    //Add bone to list of bones to merge and set bones bindposes
                                    Transform[] currentMeshBones = currentMesh.bones;
                                    for (int iii = 0; iii < currentMeshBones.Length; iii++)
                                    {
                                        bonesToMerge.Add(currentMeshBones[iii]);
                                        if (compatibilityMode == true)
                                        {
                                            bindPosesToMerge.Add(currentMesh.sharedMesh.bindposes[iii] * currentMesh.transform.worldToLocalMatrix);
                                        }
                                        if (compatibilityMode == false)
                                        {
                                            bindPosesToMerge.Add(currentMeshBones[iii].worldToLocalMatrix * currentMesh.transform.worldToLocalMatrix);
                                        }
                                    }
                                    //Configure the Combine Instances for each submesh or mesh
                                    CombineInstance combineInstance = new CombineInstance();
                                    combineInstance.mesh = currentMesh.sharedMesh;
                                    combineInstance.subMeshIndex = ii;
                                    combineInstance.transform = currentMesh.transform.localToWorldMatrix;
                                    combinesToMerge.Add(combineInstance);
                                }
                            }

                            //Enable "Always Animate" in Animator
                            AnimatorCullingMode oldModeAnimator = GetComponent<Animator>().cullingMode;
                            GetComponent<Animator>().cullingMode = AnimatorCullingMode.AlwaysAnimate;
                            //Create GameObject with merged mesh
                            GameObject meshCombinedOBJ = new GameObject("Mesh (Material " + materialCount.ToString() + ")");
                            meshCombinedOBJ.transform.SetParent(combinedMeshesOBJ.transform);
                            //Configure the Renderer to the merged mesh
                            SkinnedMeshRenderer meshCombinedSMR = meshCombinedOBJ.AddComponent<SkinnedMeshRenderer>();
                            if (qualityOfAnim == AnimQuality.UseQualitySettings) { meshCombinedSMR.quality = SkinQuality.Auto; }
                            if (qualityOfAnim == AnimQuality.Bad) { meshCombinedSMR.quality = SkinQuality.Bone1; }
                            if (qualityOfAnim == AnimQuality.Good) { meshCombinedSMR.quality = SkinQuality.Bone2; }
                            if (qualityOfAnim == AnimQuality.VeryGood) { meshCombinedSMR.quality = SkinQuality.Bone4; }
                            meshCombinedSMR.rootBone = meshToCombine[0].rootBone;
                            meshCombinedSMR.sharedMaterials = new Material[] { record.Key };
                            meshCombinedSMR.updateWhenOffscreen = updateWhenOffScreen;
                            meshCombinedSMR.skinnedMotionVectors = skinnedMotionVectors;
                            //Create the merged mesh, and configure the limitation of vertices
                            meshCombinedSMR.sharedMesh = new Mesh();
                            if (moreThan65kVertices == false)
                                meshCombinedSMR.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
                            if (moreThan65kVertices == true)
                                meshCombinedSMR.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                            meshCombinedSMR.sharedMesh.CombineMeshes(combinesToMerge.ToArray(), true, true);
                            meshCombinedSMR.sharedMesh.name = "Combined Mesh";
                            //Pass the data to the new merged mesh
                            meshCombinedSMR.bones = bonesToMerge.ToArray();
                            meshCombinedSMR.sharedMesh.bindposes = bindPosesToMerge.ToArray();
                            meshCombinedSMR.sharedMesh.RecalculateBounds();
                            //Deactive the old meshes not merged
                            foreach (SkinnedMeshRenderer oldMesh in meshesToMerge)
                            {
                                if (oldMesh != null)
                                {
                                    oldMesh.gameObject.SetActive(false);
                                }
                            }
                            //Enable the old mode of culling in Animator
                            GetComponent<Animator>().cullingMode = oldModeAnimator;
#if UNITY_EDITOR
                            //Creates the asset
                            if (isEditor == true && saveDataInAssets == true && Application.isPlaying == false)
                            {
                                Editor_CreateDirectory();
                                string sceneName = SceneManager.GetActiveScene().name;
                                DateTime dateTime = new DateTime();
                                dateTime = DateTime.Now;
                                AssetDatabase.CreateAsset(meshCombinedSMR.sharedMesh, "Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Mesh/" + this.gameObject.name + " (Material " + materialCount.ToString() + ") (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                                pathsOfDataSavedInAssets.Add("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Mesh/" + this.gameObject.name + " (Material " + materialCount.ToString() + ") (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                            }
#endif
                            //Increase the material count by one
                            materialCount += 1; 
                        }
#if UNITY_EDITOR
                        //Time stats
                        timeMonitor.Stop();
                        //Save the prefab, if is desired
                        if (savePrefabOfThis == true)
                        {
                            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                            {
                                AssetDatabase.CreateFolder("Assets", "Prefabs");
                            }
                            if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Skinned Mesh Combiner"))
                            {
                                AssetDatabase.CreateFolder("Assets/Prefabs", "Skinned Mesh Combiner");
                            }
                            if (AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Skinned Mesh Combiner/" + prefabName + ".prefab", typeof(GameObject)) != null)
                            {
                                LaunchLog(LogType.Log, "Prefab \"" + prefabName + "\" already exists in your project files. Therefore, a new file was not created. You can update the existing prefab by clicking \"Apply\" here in the Inspector window.");
                            }
                            if (AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Skinned Mesh Combiner/" + prefabName + ".prefab", typeof(GameObject)) == null)
                            {
                                UnityEngine.Object prefab = PrefabUtility.CreatePrefab("Assets/Prefabs/Skinned Mesh Combiner/" + prefabName + ".prefab", this.gameObject);
                                PrefabUtility.ReplacePrefab(this.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
                                LaunchLog(LogType.Log, "The prefab \"" + prefabName + "\" was created in your project files! The path to the prefabs that the Skinned Mesh Combiner creates is the \"Prefabs/Skinned Mesh Combiner\"");
                            }
                        }
                        //Notify
                        if (isEditor == true && saveDataInAssets == true && Application.isPlaying == false)
                        {
                            Editor_ShowDialog();
                        }
                        if (isEditor == true && Application.isPlaying == false)
                        {
                            //Create string of stats
                            float optimizationRate = (1 - ((float)materialCount / (float)skinnedMeshsBefore)) * (float)100;
                            statsOfMerge = "The Skinned Mesh children of this GameObject were combined. See the optimization statistics below!"
                                + "\n\n"
                                + "Merge Method: One mesh per material"
                                + "\n"
                                + "Processed Vertices: " + processedVertices.ToString()
                                + "\n"
                                + "Processing time: " + ((double)(timeMonitor.ElapsedMilliseconds) / (double)(1000)).ToString() + "s"
                                + "\n"
                                + "Combined Meshes: " + mergedMeshes.ToString()
                                + "\n"
                                + "Mesh Count Before: " + skinnedMeshsBefore.ToString()
                                + "\n"
                                + "Mesh Count After: " + materialCount.ToString()
                                + "\n"
                                + "Draw Call Reduction: ≥" + drawCallReduction.ToString()
                                + "\n"
                                + "Optimization rate: " + optimizationRate.ToString("F2") + "%"
                                + "\n\n"
                                + "- Statistics are only generated when combined using the Inspector.";
                        }
                        if (isEditor == true && Application.isPlaying == true)
                        {
                            statsOfMerge = "You can not generate statistics while the game is running or outside the Editor.";
                        }
#endif
                        //Set variable as merged
                        isMeshesCombineds = true;
#if UNITY_EDITOR
                        //Notify
                        LaunchLog(LogType.Log, "The merge completed successfully. View the merge statistics in the \"Stats\" tab.");
#endif
                    }
                }
            }

            yield return null;
        }

        private IEnumerator MergeMeshes_AllInOne(bool isEditor)
        {
            //Checks if the meshes are already combined.
            if (isMeshesCombineds == true)
            {
                LaunchLog(LogType.Error, "The \"" + this.transform.name + "\" meshes are already combined!");
            }
            if (isMeshesCombineds == false)
            {
#if UNITY_EDITOR
                //Time stats
                System.Diagnostics.Stopwatch timeMonitor = new System.Diagnostics.Stopwatch();
                timeMonitor.Start();
#endif
                //Find the meshes to merge
                FindMeshes();

                //Mesh and submeshes list to be combined
                List<SkinnedMeshRenderer> meshesToCombine = new List<SkinnedMeshRenderer>();

                if (ExistsMeshesToMerge() == true)
                {
                    //Gets the meshes that are to be merged
                    for (int i = 0; i < meshesToMerge.Length; i++)
                    {
                        //Valid the mesh
                        if (ValidMesh(meshesToMerge[i]) == false)
                        {
                            meshesToMerge[i] = null;
                            continue;
                        }
                        //Checks if UV is larger than texture
                        if (mergeAllUVSizes == false)
                        {
                            bool largerThanTexture = true;
                            foreach (Vector2 vertexUV in meshesToMerge[i].sharedMesh.uv)
                            {
                                if (vertexUV.x > 1 || vertexUV.y > 1)
                                {
                                    largerThanTexture = true;
                                }
                                if (vertexUV.x < 0 || vertexUV.y < 0)
                                {
                                    largerThanTexture = true;
                                }
                            }
                            if (largerThanTexture == true)
                            {
                                LaunchLog(LogType.Log, "The \"" + meshesToMerge[i].transform.name + "\" mesh has a larger UV map than the texture. As desired, it was ignored by the Skinned Mesh Combiner.");
                                meshesToMerge[i] = null;
                                continue;
                            }
                        }

                        //Keep the current mesh according to your material
                        meshesToCombine.Add(meshesToMerge[i]);
                    }
                }

                bool canContinue = true;
                //Checks whether the resulting mesh will have more than 65500 vertices, if support for more than 65k vertices is disabled
                if(moreThan65kVertices == false)
                {
                    canContinue = !MeshesMoreThan65kVertices(meshesToCombine.ToArray());
                }
                //Check if there are different root bones, if desired.
                if (mergeOnlyEqualsRootBones == true)
                {
                    Transform lastRootBone = null;
                    //Get reference Root Bone
                    if(meshesToCombine.Count > 0)
                    {
                        lastRootBone = meshesToCombine.ToArray()[0].rootBone;
                    }
                    //Verify if root bones is equal to reference
                    foreach (var skinnedMesh in meshesToCombine)
                    {
                        if (skinnedMesh.rootBone != lastRootBone)
                        {
                            LaunchLog(LogType.Error, "The GameObject \"" + this.gameObject.name + "\" merge was canceled, because there are one or more meshes with different root bones. This can cause mesh deformations during merge." +
                                " The mesh with different bone that has been detected is \"" + skinnedMesh.transform.name + "\". You might consider adding it to the list of meshes to ignore. If you prefer you can force the Skinned Mesh" +
                                " Combiner to combine it by unchecking the \"Only Equal Root Bones\" option. A great way to fix this problem is to use a modeling program and re-implement the skeleton of your model so that all meshes have the same root bone.");
                            canContinue = false;
                        }
                    }
                }

                if (ExistsMeshesToMerge() == true && canContinue == true)
                {
#if UNITY_EDITOR
                    //Collect stats
                    int processedVertices = 0;
                    int mergeMeshesCount = meshesToCombine.Count;
                    int meshesCountBefore = meshesToMerge.Length;
#endif
                    //Data of meshes will be combined
                    List<Transform> bonesToMerge = new List<Transform>();
                    List<Matrix4x4> bindPosesToMerge = new List<Matrix4x4>();
                    List<CombineInstance> combinesToMerge = new List<CombineInstance>();
                    List<Texture2D> texturesToMerge = new List<Texture2D>();
                    List<int> meshesVerticesIndex = new List<int>();
                    List<Texture2D> normalMapsToMerge = new List<Texture2D>();
                    List<Texture2D> secondNormalMapsToMerge = new List<Texture2D>();
                    List<Texture2D> heightMapsToMerge = new List<Texture2D>();
                    List<Texture2D> occlusionMapsToMerge = new List<Texture2D>();
                    List<Texture2D> detailAlbedoMapsToMerge = new List<Texture2D>();
                    List<Texture2D> specularMapsToMerge = new List<Texture2D>();
                    List<Texture2D> metallicMapsToMerge = new List<Texture2D>();

                    //Obtains the data of each mesh in the list of meshes to merge
                    for (int i = 0; i < meshesToCombine.Count; i++)
                    {
                        SkinnedMeshRenderer currentMesh = meshesToCombine[i];
#if UNITY_EDITOR
                        //Collect stats
                        processedVertices += currentMesh.sharedMesh.vertexCount;
#endif
                        //Obtains the data of each submesh of current mesh
                        for (int ii = 0; ii < currentMesh.sharedMesh.subMeshCount; ii++)
                        {
                            //Add bone to list of bones to merge and set bones bindposes
                            Transform[] currentMeshBones = currentMesh.bones;
                            for (int iii = 0; iii < currentMeshBones.Length; iii++)
                            {
                                bonesToMerge.Add(currentMeshBones[iii]);
                                if (compatibilityMode == true)
                                {
                                    bindPosesToMerge.Add(currentMesh.sharedMesh.bindposes[iii] * currentMesh.transform.worldToLocalMatrix);
                                }
                                if (compatibilityMode == false)
                                {
                                    bindPosesToMerge.Add(currentMeshBones[iii].worldToLocalMatrix * currentMesh.transform.worldToLocalMatrix);
                                }
                            }
                            //Configure the Combine Instances
                            CombineInstance combineInstance = new CombineInstance();
                            combineInstance.mesh = currentMesh.sharedMesh;
                            combineInstance.subMeshIndex = ii;
                            combineInstance.transform = currentMesh.transform.localToWorldMatrix;
                            combinesToMerge.Add(combineInstance);
                            //Add the vertices count of this submesh to index of meshes
                            meshesVerticesIndex.Add(combineInstance.mesh.GetSubmesh(ii).vertexCount);
                            //Add the texture of mesh to textures to merge (if advanced effect is NOT used, just use the default texture, without copy). Make a commom atlas of textures
                            if (enableAdvancedAtlas == false)
                            {
                                texturesToMerge.Add(currentMesh.sharedMaterials[ii].mainTexture as Texture2D);
                            }
                            //Add the texture of mesh to textures to merge (if advanced effect IS used, copy the texture, to mantain same number of textures and normals, heights etc). Make differente atlas, that accept duplicated textures
                            if (enableAdvancedAtlas == true)
                            {
                                //Copy texture of this mesh to merge in atlas
                                texturesToMerge.Add(GetCopyOfTexture(currentMesh.sharedMaterials[ii].mainTexture as Texture2D));
                                //Add the specular map of mesh (if exists and enabled support) to specular maps to merge. The atlas of specular maps must have the same dimensions and structures as the texture atlas. Because the specular map atlas will use the same UV.
                                if (specularMapSupport == true)
                                {
                                    //Copy the specular map, of this mesh to merge in atlas
                                    specularMapsToMerge.Add(GetCopyOfSpecularMapOrFakeIfNotExists(currentMesh.sharedMaterials[ii], texturesToMerge[texturesToMerge.Count - 1].width, texturesToMerge[texturesToMerge.Count - 1].height));
                                }
                                //Add the metallic map of mesh (if exists and enabled support) to metallic maps to merge. The atlas of metallic maps must have the same dimensions and structures as the texture atlas. Because the metallic map atlas will use the same UV.
                                if (metallicMapSupport == true)
                                {
                                    //Copy the specular map, of this mesh to merge in atlas
                                    metallicMapsToMerge.Add(GetCopyOfMetallicMapOrFakeIfNotExists(currentMesh.sharedMaterials[ii], texturesToMerge[texturesToMerge.Count - 1].width, texturesToMerge[texturesToMerge.Count - 1].height));
                                }
                                //Add the normal map of mesh (if exists and enabled support) to normal maps to merge. The atlas of normal maps must have the same dimensions and structures as the texture atlas. Because the normal map atlas will use the same UV.
                                if (normalMapSupport == true)
                                {
                                    //Copy the normal map, of this mesh to merge in atlas
                                    normalMapsToMerge.Add(GetCopyOfNormalMapOrFakeIfNotExists(currentMesh.sharedMaterials[ii], texturesToMerge[texturesToMerge.Count - 1].width, texturesToMerge[texturesToMerge.Count - 1].height));
                                }
                                //Add the second normal map of mesh (if exists and enabled support) to second normal maps to merge. The atlas of second normal maps must have the same dimensions and structures as the texture atlas. Because the normal map atlas will use the same UV.
                                if (secondNormalMapSupport == true)
                                {
                                    //Copy the normal map, of this mesh to merge in atlas
                                    secondNormalMapsToMerge.Add(GetCopyOfSecondNormalMapOrFakeIfNotExists(currentMesh.sharedMaterials[ii], texturesToMerge[texturesToMerge.Count - 1].width, texturesToMerge[texturesToMerge.Count - 1].height));
                                }
                                //Add the height map of mesh (if exists and enabled support) to height maps to merge. The atlas of height maps must have the same dimensions and structures as the texture atlas. Because the height map atlas will use the same UV.
                                if (heightMapSupport == true && atlasMipMap == true)
                                {
                                    //Copy the height map, of this mesh to merge in atlas
                                    heightMapsToMerge.Add(GetCopyOfHeightMapOrFakeIfNotExists(currentMesh.sharedMaterials[ii], texturesToMerge[texturesToMerge.Count - 1].width, texturesToMerge[texturesToMerge.Count - 1].height));
                                }
                                //Add the occlusion map of mesh (if exists and enabled support) to occlusion maps to merge. The atlas of occlusion maps must have the same dimensions and structures as the texture atlas. Because the occlusion map atlas will use the same UV.
                                if (occlusionMapSupport == true)
                                {
                                    //Copy the occlusion map, of this mesh to merge in atlas
                                    occlusionMapsToMerge.Add(GetCopyOfOcclusionMapOrFakeIfNotExists(currentMesh.sharedMaterials[ii], texturesToMerge[texturesToMerge.Count - 1].width, texturesToMerge[texturesToMerge.Count - 1].height));
                                }
                                //Add the detail albedo map of mesh (if exists and enabled support) to detail albedo maps to merge. The atlas of detail albedo maps must have the same dimensions and structures as the texture atlas. Because the detail albedo map atlas will use the same UV.
                                if (detailAlbedoMapSupport == true)
                                {
                                    //Copy the detail albedo map, of this mesh to merge in atlas
                                    detailAlbedoMapsToMerge.Add(GetCopyOfDetailAlbedoMapOrFakeIfNotExists(currentMesh.sharedMaterials[ii], texturesToMerge[texturesToMerge.Count - 1].width, texturesToMerge[texturesToMerge.Count - 1].height));
                                }
                            }
                            //End of processament of this submesh
                        }
                    }

                    //Enable "Always Animate" in Animator
                    AnimatorCullingMode oldModeAnimator = GetComponent<Animator>().cullingMode;
                    GetComponent<Animator>().cullingMode = AnimatorCullingMode.AlwaysAnimate;
                    //Create a GameObject to store merged meshes
                    GameObject combinedMeshesOBJ = new GameObject("Combined Mesh");
                    combinedMeshesOBJ.transform.SetParent(this.transform);
                    //Create GameObject with merged mesh
                    GameObject meshCombinedOBJ = new GameObject("Mesh (All In One)");
                    meshCombinedOBJ.transform.SetParent(combinedMeshesOBJ.transform);
                    //Configure the Renderer to the merged mesh
                    SkinnedMeshRenderer meshCombinedSMR = meshCombinedOBJ.AddComponent<SkinnedMeshRenderer>();
                    if (qualityOfAnim == AnimQuality.UseQualitySettings) { meshCombinedSMR.quality = SkinQuality.Auto; }
                    if (qualityOfAnim == AnimQuality.Bad) { meshCombinedSMR.quality = SkinQuality.Bone1; }
                    if (qualityOfAnim == AnimQuality.Good) { meshCombinedSMR.quality = SkinQuality.Bone2; }
                    if (qualityOfAnim == AnimQuality.VeryGood) { meshCombinedSMR.quality = SkinQuality.Bone4; }
                    meshCombinedSMR.rootBone = meshesToCombine[0].rootBone;
                    meshCombinedSMR.updateWhenOffscreen = updateWhenOffScreen;
                    meshCombinedSMR.skinnedMotionVectors = skinnedMotionVectors;
                    //Create the merged mesh, and configure the limitation of vertices
                    meshCombinedSMR.sharedMesh = new Mesh();
                    if (moreThan65kVertices == false)
                        meshCombinedSMR.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
                    if (moreThan65kVertices == true)
                        meshCombinedSMR.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                    meshCombinedSMR.sharedMesh.CombineMeshes(combinesToMerge.ToArray(), true, true);
                    meshCombinedSMR.sharedMesh.name = "Combined Mesh";
                    //Apply tem generated material to the merged mesh
                    meshCombinedSMR.sharedMaterials = GetSelectedMaterialInListOfMaterials();
                    //Pass the data to the new merged mesh
                    meshCombinedSMR.bones = bonesToMerge.ToArray();
                    meshCombinedSMR.sharedMesh.bindposes = bindPosesToMerge.ToArray();
                    meshCombinedSMR.sharedMesh.RecalculateBounds();
                    //Deactive the old meshes not merged
                    foreach (SkinnedMeshRenderer oldMesh in meshesToMerge)
                    {
                        if (oldMesh != null)
                        {
                            oldMesh.gameObject.SetActive(false);
                        }
                    }
                    //Enable the old mode of culling in Animator
                    GetComponent<Animator>().cullingMode = oldModeAnimator;

                    //Prepare for read textures
                    bool texturesReadable = true;
                        //Create Atlas texture
                    //Get textures to merge, converted with edges to support mipmapsm, if mipmaps activated
                    List<Texture2D> texturesToMergeCompatibleWithMipMaps = new List<Texture2D>();
                    if (atlasMipMap == true)
                    {
                        try { texturesToMergeCompatibleWithMipMaps = GetConvertedTexturesToMergeForSupportMipMaps(texturesToMerge); }
                        catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                    }
                    //Create Atlas texture
                    Texture2D atlasTexture = new Texture2D(16, 16);
                    Rect[] atlasRects = new Rect[0];
                    if (atlasMipMap == true)
                    {
                        try { atlasRects = atlasTexture.PackTextures(texturesToMergeCompatibleWithMipMaps.ToArray(), PADDING_OF_ATLAS, GetSelectedResolutionOfAtlas(), false); }
                        catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                    }
                    if (atlasMipMap == false)
                    {
                        try { atlasRects = atlasTexture.PackTextures(texturesToMerge.ToArray(), PADDING_OF_ATLAS, GetSelectedResolutionOfAtlas(), false); }
                        catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                    }
                        //Create Atlas metallic map (Uses the same UV of atlas texture)
                    //Get textures metallic map to merge, converted with edges to support mipmapsm, if mipmaps activated
                    List<Texture2D> metallicMapsToMergeCompatibleWithMipMaps = new List<Texture2D>();
                    if (atlasMipMap == true)
                    {
                        try { metallicMapsToMergeCompatibleWithMipMaps = GetConvertedTexturesToMergeForSupportMipMaps(metallicMapsToMerge); }
                        catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                    }
                    //Create Atlas metallic map (Uses the same UV of atlas texture)
                    Texture2D atlasMetallicMap = new Texture2D(16, 16);
                    if (atlasMipMap == true)
                    {
                        try { atlasMetallicMap.PackTextures(metallicMapsToMergeCompatibleWithMipMaps.ToArray(), PADDING_OF_ATLAS, GetSelectedResolutionOfAtlas(), false); }
                        catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                    }
                    if (atlasMipMap == false)
                    {
                        try { atlasMetallicMap.PackTextures(metallicMapsToMerge.ToArray(), PADDING_OF_ATLAS, GetSelectedResolutionOfAtlas(), false); }
                        catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                    }
                        //Create Atlas specular map (Uses the same UV of atlas texture)
                    //Get textures specular map to merge, converted with edges to support mipmapsm, if mipmaps activated
                    List<Texture2D> specularMapsToMergeCompatibleWithMipMaps = new List<Texture2D>();
                    if (atlasMipMap == true)
                    {
                        try { specularMapsToMergeCompatibleWithMipMaps = GetConvertedTexturesToMergeForSupportMipMaps(specularMapsToMerge); }
                        catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                    }
                    //Create Atlas specular map (Uses the same UV of atlas texture)
                    Texture2D atlasSpecularMap = new Texture2D(16, 16);
                    if (atlasMipMap == true)
                    {
                        try { atlasSpecularMap.PackTextures(specularMapsToMergeCompatibleWithMipMaps.ToArray(), PADDING_OF_ATLAS, GetSelectedResolutionOfAtlas(), false); }
                        catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                    }
                    if (atlasMipMap == false)
                    {
                        try { atlasSpecularMap.PackTextures(specularMapsToMerge.ToArray(), PADDING_OF_ATLAS, GetSelectedResolutionOfAtlas(), false); }
                        catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                    }
                        //Create Atlas normal map (Uses the same UV of atlas texture)
                    //Get textures normal map to merge, converted with edges to support mipmapsm, if mipmaps activated
                    List<Texture2D> normalMapsToMergeCompatibleWithMipMaps = new List<Texture2D>();
                    if (atlasMipMap == true)
                    {
                        try { normalMapsToMergeCompatibleWithMipMaps = GetConvertedTexturesToMergeForSupportMipMaps(normalMapsToMerge); }
                        catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                    }
                    //Create Atlas normal map (Uses the same UV of atlas texture)
                    Texture2D atlasNormalMap = new Texture2D(16, 16);
                    if (atlasMipMap == true)
                    {
                        try { atlasNormalMap.PackTextures(normalMapsToMergeCompatibleWithMipMaps.ToArray(), PADDING_OF_ATLAS, GetSelectedResolutionOfAtlas(), false); }
                        catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                    }
                    if (atlasMipMap == false)
                    {
                        try { atlasNormalMap.PackTextures(normalMapsToMerge.ToArray(), PADDING_OF_ATLAS, GetSelectedResolutionOfAtlas(), false); }
                        catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                    }
                        //Create Atlas second normal map (Uses the same UV of atlas texture)
                    //Get textures second normal map to merge, converted with edges to support mipmapsm, if mipmaps activated
                    List<Texture2D> secondNormalMapsToMergeCompatibleWithMipMaps = new List<Texture2D>();
                    if (atlasMipMap == true)
                    {
                        try { secondNormalMapsToMergeCompatibleWithMipMaps = GetConvertedTexturesToMergeForSupportMipMaps(secondNormalMapsToMerge); }
                        catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                    }
                    //Create Atlas second normal map (Uses the same UV of atlas texture)
                    Texture2D atlasSecondNormalMap = new Texture2D(16, 16);
                    if (atlasMipMap == true)
                    {
                        try { atlasSecondNormalMap.PackTextures(secondNormalMapsToMergeCompatibleWithMipMaps.ToArray(), PADDING_OF_ATLAS, GetSelectedResolutionOfAtlas(), false); }
                        catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                    }
                    if (atlasMipMap == false)
                    {
                        try { atlasSecondNormalMap.PackTextures(secondNormalMapsToMerge.ToArray(), PADDING_OF_ATLAS, GetSelectedResolutionOfAtlas(), false); }
                        catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                    }
                        //Create Atlas height map (Uses the same UV of atlas texture)
                    //Get textures height maps to merge. The height map atlas, must have edges and support for mipmaps, to avoid black edges in the texture.
                    List<Texture2D> heightMapsToMergeCompatibleWithMipMaps = new List<Texture2D>();
                    try { heightMapsToMergeCompatibleWithMipMaps = GetConvertedTexturesToMergeForSupportMipMaps(heightMapsToMerge); }
                    catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                    //Create Atlas height map (Uses the same UV of atlas texture)
                    Texture2D atlasHeightMap = new Texture2D(16, 16);
                    try { atlasHeightMap.PackTextures(heightMapsToMergeCompatibleWithMipMaps.ToArray(), PADDING_OF_ATLAS, GetSelectedResolutionOfAtlas(), false); }
                    catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                        //Create Atlas occlusion map (Uses the same UV of atlas texture)
                    //Get textures occlusion map to merge, converted with edges to support mipmapsm, if mipmaps activated
                    List<Texture2D> occlusionMapsToMergeCompatibleWithMipMaps = new List<Texture2D>();
                    if (atlasMipMap == true)
                    {
                        try { occlusionMapsToMergeCompatibleWithMipMaps = GetConvertedTexturesToMergeForSupportMipMaps(occlusionMapsToMerge); }
                        catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                    }
                    //Create Atlas occlusion map (Uses the same UV of atlas texture)
                    Texture2D atlasOcclusionMap = new Texture2D(16, 16);
                    if (atlasMipMap == true)
                    {
                        try { atlasOcclusionMap.PackTextures(occlusionMapsToMergeCompatibleWithMipMaps.ToArray(), PADDING_OF_ATLAS, GetSelectedResolutionOfAtlas(), false); }
                        catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                    }
                    if (atlasMipMap == false)
                    {
                        try { atlasOcclusionMap.PackTextures(occlusionMapsToMerge.ToArray(), PADDING_OF_ATLAS, GetSelectedResolutionOfAtlas(), false); }
                        catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                    }
                        //Create Atlas detail albedo map (Uses the same UV of atlas texture)
                    //Get textures detail albedo map to merge, converted with edges to support mipmapsm, if mipmaps activated
                    List<Texture2D> detailAlbedoMapsToMergeCompatibleWithMipMaps = new List<Texture2D>();
                    if (atlasMipMap == true)
                    {
                        try { detailAlbedoMapsToMergeCompatibleWithMipMaps = GetConvertedTexturesToMergeForSupportMipMaps(detailAlbedoMapsToMerge); }
                        catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                    }
                    //Create Atlas detail albedo map (Uses the same UV of atlas texture)
                    Texture2D atlasDetailAlbedoMap = new Texture2D(16, 16);
                    if (atlasMipMap == true)
                    {
                        try { atlasDetailAlbedoMap.PackTextures(detailAlbedoMapsToMergeCompatibleWithMipMaps.ToArray(), PADDING_OF_ATLAS, GetSelectedResolutionOfAtlas(), false); }
                        catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                    }
                    if (atlasMipMap == false)
                    {
                        try { atlasDetailAlbedoMap.PackTextures(detailAlbedoMapsToMerge.ToArray(), PADDING_OF_ATLAS, GetSelectedResolutionOfAtlas(), false); }
                        catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                    }

                    //Only make process if textures is readable
                    if (texturesReadable == true)
                    {
                        //Set the atlas in material
                        meshCombinedSMR.sharedMaterials[0].mainTexture = atlasTexture;
#if UNITY_EDITOR
                        atlasMergedInAllInOne = meshCombinedSMR.sharedMaterials[0].mainTexture;
#endif
                        //Set the atlas of metallic map in material, if exists a property to storage them and metallic map support was enabled
                        if (metallicMapSupport == true)
                        {
                            if (meshCombinedSMR.sharedMaterials[0].HasProperty(metallicMapPropertyNameInsert) == true)
                            {
                                meshCombinedSMR.sharedMaterials[0].SetTexture(metallicMapPropertyNameInsert, atlasMetallicMap);
                                //Forces update of material of merged mesh
                                meshCombinedSMR.sharedMaterials[0].EnableKeyword("_METALLICGLOSSMAP");
                                meshCombinedSMR.sharedMaterials[0].EnableKeyword(metallicMapPropertyNameInsert);
                            }
                            if (meshCombinedSMR.sharedMaterials[0].HasProperty(metallicMapPropertyNameInsert) == false)
                            {
                                LaunchLog(LogType.Warning, "The metallic merge map could not be applied to the final mesh. The \"" + metallicMapPropertyNameInsert + "\" property was not found in the final material shader. Make sure you have provided a correct property name.");
                            }
                        }
                        //Set the atlas of specular map in material, if exists a property to storage them and specular map support was enabled
                        if (specularMapSupport == true)
                        {
                            if (meshCombinedSMR.sharedMaterials[0].HasProperty(specularMapPropertyNameInsert) == true)
                            {
                                meshCombinedSMR.sharedMaterials[0].SetTexture(specularMapPropertyNameInsert, atlasSpecularMap);
                                //Forces update of material of merged mesh
                                meshCombinedSMR.sharedMaterials[0].EnableKeyword("_SPECGLOSSMAP");
                                meshCombinedSMR.sharedMaterials[0].EnableKeyword(specularMapPropertyNameInsert);
                            }
                            if (meshCombinedSMR.sharedMaterials[0].HasProperty(specularMapPropertyNameInsert) == false)
                            {
                                LaunchLog(LogType.Warning, "The specular merge map could not be applied to the final mesh. The \"" + specularMapPropertyNameInsert + "\" property was not found in the final material shader. Make sure you have provided a correct property name.");
                            }
                        }
                        //Set the atlas of normal map in material, if exists a property to storage them and normal map support was enabled, and set the normal map scale
                        if (normalMapSupport == true)
                        {
                            if (meshCombinedSMR.sharedMaterials[0].HasProperty(normalMapPropertyNameInsert) == true)
                            {
                                meshCombinedSMR.sharedMaterials[0].SetTexture(normalMapPropertyNameInsert, atlasNormalMap);
                                //Forces update of material of merged mesh
                                meshCombinedSMR.sharedMaterials[0].EnableKeyword("_NORMALMAP");
                                meshCombinedSMR.sharedMaterials[0].EnableKeyword(normalMapPropertyNameInsert);
                                //Set the normal map scale
                                if(applyNormalMapScale == true && meshCombinedSMR.sharedMaterials[0].HasProperty(normalMapScalePropertyName) == true)
                                {
                                    meshCombinedSMR.sharedMaterials[0].SetFloat(normalMapScalePropertyName, normalMapScale);
                                }
                                if (applyNormalMapScale == true && meshCombinedSMR.sharedMaterials[0].HasProperty(normalMapScalePropertyName) == false)
                                {
                                    LaunchLog(LogType.Warning, "The normal map scale could not be applied to the final merge material. Property \"" + normalMapScalePropertyName + "\"  was not found in the final material.");
                                }
                            }
                            if (meshCombinedSMR.sharedMaterials[0].HasProperty(normalMapPropertyNameInsert) == false)
                            {
                                LaunchLog(LogType.Warning, "The normal merge map could not be applied to the final mesh. The \"" + normalMapPropertyNameInsert + "\" property was not found in the final material shader. Make sure you have provided a correct property name.");
                            }
                        }
                        //Set the atlas of second normal map in material, if exists a property to storage them and second normal map support was enabled, and set the second normal map scale
                        if (secondNormalMapSupport == true)
                        {
                            if (meshCombinedSMR.sharedMaterials[0].HasProperty(secondNormalMapPropertyNameInsert) == true)
                            {
                                meshCombinedSMR.sharedMaterials[0].SetTexture(secondNormalMapPropertyNameInsert, atlasSecondNormalMap);
                                //Forces update of material of merged mesh
                                meshCombinedSMR.sharedMaterials[0].EnableKeyword("_DETAIL_MULX2");
                                meshCombinedSMR.sharedMaterials[0].EnableKeyword(secondNormalMapPropertyNameInsert);
                                //Set the normal map scale
                                if (applySecondNormalMapScale == true && meshCombinedSMR.sharedMaterials[0].HasProperty(secondNormalMapScalePropertyName) == true)
                                {
                                    meshCombinedSMR.sharedMaterials[0].SetFloat(secondNormalMapScalePropertyName, secondNormalMapScale);
                                }
                                if (applySecondNormalMapScale == true && meshCombinedSMR.sharedMaterials[0].HasProperty(secondNormalMapScalePropertyName) == false)
                                {
                                    LaunchLog(LogType.Warning, "The second normal map scale could not be applied to the final merge material. Property \"" + secondNormalMapScalePropertyName + "\"  was not found in the final material.");
                                }
                            }
                            if (meshCombinedSMR.sharedMaterials[0].HasProperty(secondNormalMapPropertyNameInsert) == false)
                            {
                                LaunchLog(LogType.Warning, "The second normal merge map could not be applied to the final mesh. The \"" + secondNormalMapPropertyNameInsert + "\" property was not found in the final material shader. Make sure you have provided a correct property name.");
                            }
                        }
                        //Set the atlas of height map in material, if exists a property to storage them and height map support was enabled, and set the height map scale
                        if (heightMapSupport == true && atlasMipMap == true)
                        {
                            if (meshCombinedSMR.sharedMaterials[0].HasProperty(heightMapPropertyNameInsert) == true)
                            {
                                meshCombinedSMR.sharedMaterials[0].SetTexture(heightMapPropertyNameInsert, atlasHeightMap);
                                //Forces update of material of merged mesh
                                meshCombinedSMR.sharedMaterials[0].EnableKeyword("_PARALLAXMAP");
                                meshCombinedSMR.sharedMaterials[0].EnableKeyword(heightMapPropertyNameInsert);
                                //Set the height map scale
                                if (applyHeightMapScale == true && meshCombinedSMR.sharedMaterials[0].HasProperty(heightMapScalePropertyName) == true)
                                {
                                    meshCombinedSMR.sharedMaterials[0].SetFloat(heightMapScalePropertyName, heightMapScale);
                                }
                                if (applyHeightMapScale == true && meshCombinedSMR.sharedMaterials[0].HasProperty(heightMapScalePropertyName) == false)
                                {
                                    LaunchLog(LogType.Warning, "The height map scale could not be applied to the final merge material. Property \"" + heightMapScalePropertyName + "\"  was not found in the final material.");
                                }
                            }
                            if (meshCombinedSMR.sharedMaterials[0].HasProperty(heightMapPropertyNameInsert) == false)
                            {
                                LaunchLog(LogType.Warning, "The height merge map could not be applied to the final mesh. The \"" + heightMapPropertyNameInsert + "\" property was not found in the final material shader. Make sure you have provided a correct property name.");
                            }
                        }
                        //Set the atlas of occlusion map in material, if exists a property to storage them and occlusion map support was enabled, and set the occlusion map scale
                        if (occlusionMapSupport == true)
                        {
                            if (meshCombinedSMR.sharedMaterials[0].HasProperty(occlusionMapPropertyNameInsert) == true)
                            {
                                meshCombinedSMR.sharedMaterials[0].SetTexture(occlusionMapPropertyNameInsert, atlasOcclusionMap);
                                //Forces update of material of merged mesh
                                meshCombinedSMR.sharedMaterials[0].EnableKeyword("_DETAIL_MULX2");
                                meshCombinedSMR.sharedMaterials[0].EnableKeyword(occlusionMapPropertyNameInsert);
                                //Set the occlusion map scale
                                if (applyOcclusionMapScale == true && meshCombinedSMR.sharedMaterials[0].HasProperty(occlusionMapScalePropertyName) == true)
                                {
                                    meshCombinedSMR.sharedMaterials[0].SetFloat(occlusionMapScalePropertyName, occlusionMapScale);
                                }
                                if (applyOcclusionMapScale == true && meshCombinedSMR.sharedMaterials[0].HasProperty(occlusionMapScalePropertyName) == false)
                                {
                                    LaunchLog(LogType.Warning, "The occlusion map scale could not be applied to the final merge material. Property \"" + occlusionMapScalePropertyName + "\"  was not found in the final material.");
                                }
                            }
                            if (meshCombinedSMR.sharedMaterials[0].HasProperty(occlusionMapPropertyNameInsert) == false)
                            {
                                LaunchLog(LogType.Warning, "The occlusion merge map could not be applied to the final mesh. The \"" + occlusionMapPropertyNameInsert + "\" property was not found in the final material shader. Make sure you have provided a correct property name.");
                            }
                        }
                        //Set the atlas of detail albedo map in material, if exists a property to storage them and detail albedo map support was enabled
                        if (detailAlbedoMapSupport == true)
                        {
                            if (meshCombinedSMR.sharedMaterials[0].HasProperty(detailAlbedoMapPropertyNameInsert) == true)
                            {
                                meshCombinedSMR.sharedMaterials[0].SetTexture(detailAlbedoMapPropertyNameInsert, atlasDetailAlbedoMap);
                                //Forces update of material of merged mesh
                                meshCombinedSMR.sharedMaterials[0].EnableKeyword("_DETAIL_MULX2");
                                meshCombinedSMR.sharedMaterials[0].EnableKeyword(detailAlbedoMapPropertyNameInsert);
                            }
                            if (meshCombinedSMR.sharedMaterials[0].HasProperty(detailAlbedoMapPropertyNameInsert) == false)
                            {
                                LaunchLog(LogType.Warning, "The detail albedo merge map could not be applied to the final mesh. The \"" + detailAlbedoMapPropertyNameInsert + "\" property was not found in the final material shader. Make sure you have provided a correct property name.");
                            }
                        }

                        //Prepare the UVs array
                        Vector2[] originalCombinedUVs = meshCombinedSMR.sharedMesh.uv;
                        Vector2[] newCombinedUVs = new Vector2[originalCombinedUVs.Length];
                        //Change all vertex UVs to positive
                        for (int i = 0; i < originalCombinedUVs.Length; i++)
                        {
                            if (originalCombinedUVs[i].x < 0) { originalCombinedUVs[i].x = originalCombinedUVs[i].x * -1; };
                            if (originalCombinedUVs[i].y < 0) { originalCombinedUVs[i].y = originalCombinedUVs[i].y * -1; };
                        }
                        //Calculates the highest point of the UV map of each mesh
                        Vector2[] maxSizeOfMeshesUV = new Vector2[meshesVerticesIndex.Count];
                        int currentMeshOffset = meshesVerticesIndex[0];
                        int currentMeshUV = 0;
                        for (int i = 0; i < originalCombinedUVs.Length; i++)
                        {
                            //Verifies which mesh this vertex belongs to
                            if (i >= currentMeshOffset)
                            {
                                //Verify if the current mesh UV is in the end, before add more
                                if (currentMeshUV < meshesVerticesIndex.Count - 1)
                                    currentMeshUV += 1;
                                currentMeshOffset += meshesVerticesIndex[currentMeshUV];
                            }

                            maxSizeOfMeshesUV[currentMeshUV].x = Mathf.Max(originalCombinedUVs[i].x, maxSizeOfMeshesUV[currentMeshUV].x);
                            maxSizeOfMeshesUV[currentMeshUV].y = Mathf.Max(originalCombinedUVs[i].y, maxSizeOfMeshesUV[currentMeshUV].y);
                        }
                        //Resizes the uvs to the atlas so that it ignores the edges of each texture
                        currentMeshOffset = meshesVerticesIndex[0];
                        currentMeshUV = 0;
                        for (int i = 0; i < originalCombinedUVs.Length; i++)
                        {
                            //Verifies which mesh this vertex belongs to
                            if (i >= currentMeshOffset)
                            {
                                //Verify if the current mesh UV is in the end, before add more
                                if (currentMeshUV < meshesVerticesIndex.Count - 1)
                                    currentMeshUV += 1;
                                currentMeshOffset += meshesVerticesIndex[currentMeshUV];
                            }

                            float percentEdgeOfCurrentTextureX = 0;
                            float percentEdgeOfCurrentTextureY = 0;
                            if (atlasMipMap == true)
                            {
                                //calculates the size of the uv for each texture, to ignore the edges
                                percentEdgeOfCurrentTextureX = (1 - ((float)texturesToMerge[currentMeshUV].width / (float)texturesToMergeCompatibleWithMipMaps[currentMeshUV].width)) / 2;
                                percentEdgeOfCurrentTextureY = (1 - ((float)texturesToMerge[currentMeshUV].height / (float)texturesToMergeCompatibleWithMipMaps[currentMeshUV].height)) / 2;
                            }

                            //If the UV is not larger than the texture
                            if (maxSizeOfMeshesUV[currentMeshUV].x <= 1)
                            {
                                if (atlasMipMap == true)
                                {
                                    newCombinedUVs[i].x = Mathf.Lerp(atlasRects[currentMeshUV].xMin, atlasRects[currentMeshUV].xMax, Mathf.Lerp(percentEdgeOfCurrentTextureX, 1 - percentEdgeOfCurrentTextureX, originalCombinedUVs[i].x));
                                }
                                if (atlasMipMap == false)
                                {
                                    newCombinedUVs[i].x = Mathf.Lerp(atlasRects[currentMeshUV].xMin, atlasRects[currentMeshUV].xMax, originalCombinedUVs[i].x);
                                }
                            }
                            if (maxSizeOfMeshesUV[currentMeshUV].y <= 1)
                            {
                                if (atlasMipMap == true)
                                {
                                    newCombinedUVs[i].y = Mathf.Lerp(atlasRects[currentMeshUV].yMin, atlasRects[currentMeshUV].yMax, Mathf.Lerp(percentEdgeOfCurrentTextureY, 1 - percentEdgeOfCurrentTextureY, originalCombinedUVs[i].y));
                                }
                                if (atlasMipMap == false)
                                {
                                    newCombinedUVs[i].y = Mathf.Lerp(atlasRects[currentMeshUV].yMin, atlasRects[currentMeshUV].yMax, originalCombinedUVs[i].y);
                                }
                            }

                            //If the UV is larger than the texture
                            if (maxSizeOfMeshesUV[currentMeshUV].x > 1)
                            {
                                if (atlasMipMap == true)
                                {
                                    newCombinedUVs[i].x = Mathf.Lerp(atlasRects[currentMeshUV].xMin, atlasRects[currentMeshUV].xMax, Mathf.Lerp(percentEdgeOfCurrentTextureX, 1 - percentEdgeOfCurrentTextureX, originalCombinedUVs[i].x / maxSizeOfMeshesUV[currentMeshUV].x));
                                }
                                if (atlasMipMap == false)
                                {
                                    newCombinedUVs[i].x = Mathf.Lerp(atlasRects[currentMeshUV].xMin, atlasRects[currentMeshUV].xMax, originalCombinedUVs[i].x / maxSizeOfMeshesUV[currentMeshUV].x);
                                }
                            }
                            if (maxSizeOfMeshesUV[currentMeshUV].y > 1)
                            {
                                if (atlasMipMap == true)
                                {
                                    newCombinedUVs[i].y = Mathf.Lerp(atlasRects[currentMeshUV].yMin, atlasRects[currentMeshUV].yMax, Mathf.Lerp(percentEdgeOfCurrentTextureY, 1 - percentEdgeOfCurrentTextureY, originalCombinedUVs[i].y / maxSizeOfMeshesUV[currentMeshUV].y));
                                }
                                if (atlasMipMap == false)
                                {
                                    newCombinedUVs[i].y = Mathf.Lerp(atlasRects[currentMeshUV].yMin, atlasRects[currentMeshUV].yMax, originalCombinedUVs[i].y / maxSizeOfMeshesUV[currentMeshUV].y);
                                }
                            }
                        }
                        //Apply the new UV map
                        meshCombinedSMR.sharedMesh.uv = newCombinedUVs;
                    }
                    //Show tip for enable Read/Write in log if textures is not readable
                    if (texturesReadable == false)
                    {
                        LaunchLog(LogType.Log, "It looks like there was an error trying to read the textures to merge them. Verify that the textures to be merged are with the \"Read/Write Enabled\" option in the Unity import settings.");
                    }
                    //Show UV vertices in all atlas
                    if (showUvVerticesInAtlas == true)
                    {
                        for (int i = 0; i < meshCombinedSMR.sharedMesh.uv.Length; i++)
                        {
                            atlasTexture.SetPixel((int)(atlasTexture.width * meshCombinedSMR.sharedMesh.uv[i].x), (int)(atlasTexture.height * meshCombinedSMR.sharedMesh.uv[i].y), Color.yellow);
                        }
                        atlasTexture.Apply();
                        if (metallicMapSupport == true)
                        {
                            for (int i = 0; i < meshCombinedSMR.sharedMesh.uv.Length; i++)
                            {
                                atlasMetallicMap.SetPixel((int)(atlasMetallicMap.width * meshCombinedSMR.sharedMesh.uv[i].x), (int)(atlasMetallicMap.height * meshCombinedSMR.sharedMesh.uv[i].y), Color.yellow);
                            }
                            atlasMetallicMap.Apply();
                        }
                        if (specularMapSupport == true)
                        {
                            for (int i = 0; i < meshCombinedSMR.sharedMesh.uv.Length; i++)
                            {
                                atlasSpecularMap.SetPixel((int)(atlasSpecularMap.width * meshCombinedSMR.sharedMesh.uv[i].x), (int)(atlasSpecularMap.height * meshCombinedSMR.sharedMesh.uv[i].y), Color.yellow);
                            }
                            atlasSpecularMap.Apply();
                        }
                        if (normalMapSupport == true)
                        {
                            for (int i = 0; i < meshCombinedSMR.sharedMesh.uv.Length; i++)
                            {
                                atlasNormalMap.SetPixel((int)(atlasNormalMap.width * meshCombinedSMR.sharedMesh.uv[i].x), (int)(atlasNormalMap.height * meshCombinedSMR.sharedMesh.uv[i].y), Color.yellow);
                            }
                            atlasNormalMap.Apply();
                        }
                        if (secondNormalMapSupport == true)
                        {
                            for (int i = 0; i < meshCombinedSMR.sharedMesh.uv.Length; i++)
                            {
                                atlasSecondNormalMap.SetPixel((int)(atlasSecondNormalMap.width * meshCombinedSMR.sharedMesh.uv[i].x), (int)(atlasSecondNormalMap.height * meshCombinedSMR.sharedMesh.uv[i].y), Color.yellow);
                            }
                            atlasSecondNormalMap.Apply();
                        }
                        if (heightMapSupport == true && atlasMipMap == true)
                        {
                            for (int i = 0; i < meshCombinedSMR.sharedMesh.uv.Length; i++)
                            {
                                atlasHeightMap.SetPixel((int)(atlasHeightMap.width * meshCombinedSMR.sharedMesh.uv[i].x), (int)(atlasHeightMap.height * meshCombinedSMR.sharedMesh.uv[i].y), Color.yellow);
                            }
                            atlasHeightMap.Apply();
                        }
                        if (occlusionMapSupport == true)
                        {
                            for (int i = 0; i < meshCombinedSMR.sharedMesh.uv.Length; i++)
                            {
                                atlasOcclusionMap.SetPixel((int)(atlasOcclusionMap.width * meshCombinedSMR.sharedMesh.uv[i].x), (int)(atlasOcclusionMap.height * meshCombinedSMR.sharedMesh.uv[i].y), Color.yellow);
                            }
                            atlasOcclusionMap.Apply();
                        }
                        if (detailAlbedoMapSupport == true)
                        {
                            for (int i = 0; i < meshCombinedSMR.sharedMesh.uv.Length; i++)
                            {
                                atlasDetailAlbedoMap.SetPixel((int)(atlasDetailAlbedoMap.width * meshCombinedSMR.sharedMesh.uv[i].x), (int)(atlasDetailAlbedoMap.height * meshCombinedSMR.sharedMesh.uv[i].y), Color.yellow);
                            }
                            atlasDetailAlbedoMap.Apply();
                        }
                    }
#if UNITY_EDITOR
                    //Time stats
                    timeMonitor.Stop();
                    //Creates the asset
                    if (isEditor == true && saveDataInAssets == true && Application.isPlaying == false)
                    {
                        Editor_CreateDirectory();
                        string sceneName = SceneManager.GetActiveScene().name;
                        DateTime dateTime = new DateTime();
                        dateTime = DateTime.Now;

                        if(exportTexturesAsPng == true)
                        {
                            if (!AssetDatabase.IsValidFolder("Assets/_Exported"))
                            {
                                AssetDatabase.CreateFolder("Assets", "_Exported");
                            }
                        }

                        AssetDatabase.CreateAsset(meshCombinedSMR.sharedMesh, "Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Mesh/" + this.gameObject.name + " (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                        pathsOfDataSavedInAssets.Add("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Mesh/" + this.gameObject.name + " (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                        AssetDatabase.CreateAsset(meshCombinedSMR.sharedMaterials[0].mainTexture, "Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Texture/" + this.gameObject.name + " (MainTexture) (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                        pathsOfDataSavedInAssets.Add("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Texture/" + this.gameObject.name + " (MainTexture) (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                        if(exportTexturesAsPng == true)
                        {
                            Texture2D mainTexture = meshCombinedSMR.sharedMaterials[0].mainTexture as Texture2D;
                            byte[] mainTextureBytes = mainTexture.EncodeToPNG();
                            File.WriteAllBytes("Assets/_Exported/" + nameOfTextures + " (MainTexture).png", mainTextureBytes);
                        }
                        if (specularMapSupport == true)
                        {
                            if (meshCombinedSMR.sharedMaterials[0].HasProperty(specularMapPropertyNameInsert) == true && meshCombinedSMR.sharedMaterials[0].GetTexture(specularMapPropertyNameInsert) != null)
                            {
                                AssetDatabase.CreateAsset(meshCombinedSMR.sharedMaterials[0].GetTexture(specularMapPropertyNameInsert), "Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Texture/" + this.gameObject.name + " (SpecularMap) (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                                pathsOfDataSavedInAssets.Add("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Texture/" + this.gameObject.name + " (SpecularMap) (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                                if (exportTexturesAsPng == true)
                                {
                                    Texture2D texture = meshCombinedSMR.sharedMaterials[0].GetTexture(specularMapPropertyNameInsert) as Texture2D;
                                    byte[] textureBytes = texture.EncodeToPNG();
                                    File.WriteAllBytes("Assets/_Exported/" + nameOfTextures + " (SpecularMap).png", textureBytes);
                                }
                            }
                        }
                        if (metallicMapSupport == true)
                        {
                            if (meshCombinedSMR.sharedMaterials[0].HasProperty(metallicMapPropertyNameInsert) == true && meshCombinedSMR.sharedMaterials[0].GetTexture(metallicMapPropertyNameInsert) != null)
                            {
                                AssetDatabase.CreateAsset(meshCombinedSMR.sharedMaterials[0].GetTexture(metallicMapPropertyNameInsert), "Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Texture/" + this.gameObject.name + " (MetallicMap) (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                                pathsOfDataSavedInAssets.Add("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Texture/" + this.gameObject.name + " (MetallicMap) (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                                if (exportTexturesAsPng == true)
                                {
                                    Texture2D texture = meshCombinedSMR.sharedMaterials[0].GetTexture(metallicMapPropertyNameInsert) as Texture2D;
                                    byte[] textureBytes = texture.EncodeToPNG();
                                    File.WriteAllBytes("Assets/_Exported/" + nameOfTextures + " (MetallicMap).png", textureBytes);
                                }
                            }
                        }
                        if (normalMapSupport == true)
                        {
                            if (meshCombinedSMR.sharedMaterials[0].HasProperty(normalMapPropertyNameInsert) == true && meshCombinedSMR.sharedMaterials[0].GetTexture(normalMapPropertyNameInsert) != null)
                            {
                                AssetDatabase.CreateAsset(meshCombinedSMR.sharedMaterials[0].GetTexture(normalMapPropertyNameInsert), "Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Texture/" + this.gameObject.name + " (NormalMap) (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                                pathsOfDataSavedInAssets.Add("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Texture/" + this.gameObject.name + " (NormalMap) (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                                if (exportTexturesAsPng == true)
                                {
                                    Texture2D texture = meshCombinedSMR.sharedMaterials[0].GetTexture(normalMapPropertyNameInsert) as Texture2D;
                                    byte[] textureBytes = texture.EncodeToPNG();
                                    File.WriteAllBytes("Assets/_Exported/" + nameOfTextures + " (NormalMap).png", textureBytes);
                                }
                            }
                        }
                        if (secondNormalMapSupport == true)
                        {
                            if (meshCombinedSMR.sharedMaterials[0].HasProperty(secondNormalMapPropertyNameInsert) == true && meshCombinedSMR.sharedMaterials[0].GetTexture(secondNormalMapPropertyNameInsert) != null)
                            {
                                AssetDatabase.CreateAsset(meshCombinedSMR.sharedMaterials[0].GetTexture(secondNormalMapPropertyNameInsert), "Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Texture/" + this.gameObject.name + " (NormalMap2x) (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                                pathsOfDataSavedInAssets.Add("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Texture/" + this.gameObject.name + " (NormalMap2x) (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                                if (exportTexturesAsPng == true)
                                {
                                    Texture2D texture = meshCombinedSMR.sharedMaterials[0].GetTexture(secondNormalMapPropertyNameInsert) as Texture2D;
                                    byte[] textureBytes = texture.EncodeToPNG();
                                    File.WriteAllBytes("Assets/_Exported/" + nameOfTextures + " (NormalMap2x).png", textureBytes);
                                }
                            }
                        }
                        if (heightMapSupport == true && atlasMipMap == true)
                        {
                            if (meshCombinedSMR.sharedMaterials[0].HasProperty(heightMapPropertyNameInsert) == true && meshCombinedSMR.sharedMaterials[0].GetTexture(heightMapPropertyNameInsert) != null)
                            {
                                AssetDatabase.CreateAsset(meshCombinedSMR.sharedMaterials[0].GetTexture(heightMapPropertyNameInsert), "Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Texture/" + this.gameObject.name + " (HeightMap) (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                                pathsOfDataSavedInAssets.Add("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Texture/" + this.gameObject.name + " (HeightMap) (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                                if (exportTexturesAsPng == true)
                                {
                                    Texture2D texture = meshCombinedSMR.sharedMaterials[0].GetTexture(heightMapPropertyNameInsert) as Texture2D;
                                    byte[] textureBytes = texture.EncodeToPNG();
                                    File.WriteAllBytes("Assets/_Exported/" + nameOfTextures + " (HeightMap).png", textureBytes);
                                }
                            }
                        }
                        if (occlusionMapSupport == true)
                        {
                            if (meshCombinedSMR.sharedMaterials[0].HasProperty(occlusionMapPropertyNameInsert) == true && meshCombinedSMR.sharedMaterials[0].GetTexture(occlusionMapPropertyNameInsert) != null)
                            {
                                AssetDatabase.CreateAsset(meshCombinedSMR.sharedMaterials[0].GetTexture(occlusionMapPropertyNameInsert), "Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Texture/" + this.gameObject.name + " (OcclusionMap) (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                                pathsOfDataSavedInAssets.Add("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Texture/" + this.gameObject.name + " (OcclusionMap) (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                                if (exportTexturesAsPng == true)
                                {
                                    Texture2D texture = meshCombinedSMR.sharedMaterials[0].GetTexture(occlusionMapPropertyNameInsert) as Texture2D;
                                    byte[] textureBytes = texture.EncodeToPNG();
                                    File.WriteAllBytes("Assets/_Exported/" + nameOfTextures + " (OcclusionMap).png", textureBytes);
                                }
                            }
                        }
                        if (detailAlbedoMapSupport == true)
                        {
                            if (meshCombinedSMR.sharedMaterials[0].HasProperty(detailAlbedoMapPropertyNameInsert) == true && meshCombinedSMR.sharedMaterials[0].GetTexture(detailAlbedoMapPropertyNameInsert) != null)
                            {
                                AssetDatabase.CreateAsset(meshCombinedSMR.sharedMaterials[0].GetTexture(detailAlbedoMapPropertyNameInsert), "Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Texture/" + this.gameObject.name + " (DetailAlbedoMap) (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                                pathsOfDataSavedInAssets.Add("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Texture/" + this.gameObject.name + " (DetailAlbedoMap) (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                                if (exportTexturesAsPng == true)
                                {
                                    Texture2D texture = meshCombinedSMR.sharedMaterials[0].GetTexture(detailAlbedoMapPropertyNameInsert) as Texture2D;
                                    byte[] textureBytes = texture.EncodeToPNG();
                                    File.WriteAllBytes("Assets/_Exported/" + nameOfTextures + " (DetailAlbedoMap).png", textureBytes);
                                }
                            }
                        }
                        AssetDatabase.CreateAsset(meshCombinedSMR.sharedMaterials[0], "Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Material/" + this.gameObject.name + " (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                        pathsOfDataSavedInAssets.Add("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Material/" + this.gameObject.name + " (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                        if(savePrefabOfThis == true)
                        {
                            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                            {
                                AssetDatabase.CreateFolder("Assets", "Prefabs");
                            }
                            if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Skinned Mesh Combiner"))
                            {
                                AssetDatabase.CreateFolder("Assets/Prefabs", "Skinned Mesh Combiner");
                            }
                            if (AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Skinned Mesh Combiner/" + prefabName + ".prefab", typeof(GameObject)) != null)
                            {
                                LaunchLog(LogType.Log, "Prefab \"" + prefabName + "\" already exists in your project files. Therefore, a new file was not created. You can update the existing prefab by clicking \"Apply\" here in the Inspector window.");
                            }
                            if (AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Skinned Mesh Combiner/" + prefabName + ".prefab", typeof(GameObject)) == null)
                            {
                                UnityEngine.Object prefab = PrefabUtility.CreatePrefab("Assets/Prefabs/Skinned Mesh Combiner/" + prefabName + ".prefab", this.gameObject);
                                PrefabUtility.ReplacePrefab(this.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
                                LaunchLog(LogType.Log, "The prefab \"" + prefabName + "\" was created in your project files! The path to the prefabs that the Skinned Mesh Combiner creates is the \"Prefabs/Skinned Mesh Combiner\"");
                            }
                        }
                        //Refresh the project explorer
                        AssetDatabase.Refresh();

                        Editor_ShowDialog();
                    }
                    if(isEditor == true && Application.isPlaying == false)
                    {
                        //Create string of stats
                        statsOfMerge = "The Skinned Mesh children of this GameObject were combined. See the optimization statistics below!"
                            + "\n\n"
                            + "Merge Method: All In one"
                            + "\n"
                            + "Processed Vertices: " + processedVertices.ToString()
                            + "\n"
                            + "Processing time: " + ((double)(timeMonitor.ElapsedMilliseconds) / (double)(1000)).ToString() + "s"
                            + "\n"
                            + "Combined Meshes: " + mergeMeshesCount.ToString()
                            + "\n"
                            + "Mesh Count Before: " + meshesCountBefore.ToString()
                            + "\n"
                            + "Mesh Count After: " + ((meshesCountBefore - mergeMeshesCount) + 1).ToString()
                            + "\n"
                            + "Draw Call Reduction: ≥" + (mergeMeshesCount - 1).ToString()
                            + "\n"
                            + "Materials Generated: 1"
                            + "\n"
                            + "Atlas Resolution: " + atlasTexture.width + "x" + atlasTexture.height + "p"
                            + "\n"
                            + "Packaged Textures: " + texturesToMerge.Count.ToString()
                            + "\n"
                            + "Optimization rate: 100%"
                            + "\n\n"
                            + "- Statistics are only generated when combined using the Inspector.";
                    }
                    if (isEditor == true && Application.isPlaying == true)
                    {
                        statsOfMerge = "You can not generate statistics while the game is running or outside the Editor.";
                    }
#endif

                    //Set variable as merged
                    isMeshesCombineds = true;
#if UNITY_EDITOR
                    //Notify
                    LaunchLog(LogType.Log, "The merge completed successfully. View the merge statistics in the \"Stats\" tab.");
#endif
                }
            }

            yield return null;
        }

        private IEnumerator MergeMeshes_JustMaterialColors(bool isEditor)
        {
            //Checks if the meshes are already combined.
            if (isMeshesCombineds == true)
            {
                LaunchLog(LogType.Error, "The \"" + this.transform.name + "\" meshes are already combined!");
            }
            if (isMeshesCombineds == false)
            {
#if UNITY_EDITOR
                //Time stats
                System.Diagnostics.Stopwatch timeMonitor = new System.Diagnostics.Stopwatch();
                timeMonitor.Start();
#endif
                //Find the meshes to merge
                FindMeshes();

                //Mesh and submeshes list to be combined
                List<SkinnedMeshRenderer> meshesToCombine = new List<SkinnedMeshRenderer>();

                if (ExistsMeshesToMerge() == true)
                {
                    //Gets the meshes that are to be merged
                    for (int i = 0; i < meshesToMerge.Length; i++)
                    {
                        //Valid the mesh
                        if (ValidMesh(meshesToMerge[i]) == false)
                        {
                            meshesToMerge[i] = null;
                            continue;
                        }

                        //Keep the current mesh according to your material
                        meshesToCombine.Add(meshesToMerge[i]);
                    }
                }

                bool canContinue = true;
                //Checks whether the resulting mesh will have more than 65500 vertices, if support for more than 65k vertices is disabled
                if (moreThan65kVertices == false)
                {
                    canContinue = !MeshesMoreThan65kVertices(meshesToCombine.ToArray());
                }
                //Check if there are different root bones, if desired.
                if (mergeOnlyEqualsRootBones == true)
                {
                    Transform lastRootBone = null;
                    //Get reference Root Bone
                    if (meshesToCombine.Count > 0)
                    {
                        lastRootBone = meshesToCombine.ToArray()[0].rootBone;
                    }
                    //Verify if root bones is equal to reference
                    foreach (var skinnedMesh in meshesToCombine)
                    {
                        if (skinnedMesh.rootBone != lastRootBone)
                        {
                            LaunchLog(LogType.Error, "The GameObject \"" + this.gameObject.name + "\" merge was canceled, because there are one or more meshes with different root bones. This can cause mesh deformations during merge." +
                                " The mesh with different bone that has been detected is \"" + skinnedMesh.transform.name + "\". You might consider adding it to the list of meshes to ignore. If you prefer you can force the Skinned Mesh" +
                                " Combiner to combine it by unchecking the \"Only Equal Root Bones\" option. A great way to fix this problem is to use a modeling program and re-implement the skeleton of your model so that all meshes have the same root bone.");
                            canContinue = false;
                        }
                    }
                }

                if(ExistsMeshesToMerge() == true && canContinue == true)
                {
#if UNITY_EDITOR
                    //Collect stats
                    int processedVertices = 0;
                    int mergeMeshesCount = meshesToCombine.Count;
                    int meshesCountBefore = meshesToMerge.Length;
#endif
                    //Data of meshes will be combined
                    List<Transform> bonesToMerge = new List<Transform>();
                    List<Matrix4x4> bindPosesToMerge = new List<Matrix4x4>();
                    List<CombineInstance> combinesToMerge = new List<CombineInstance>();
                    List<Texture2D> texturesToMerge = new List<Texture2D>();
                    List<int> meshesVerticesIndex = new List<int>();

                    //Obtains the data of each mesh in the list of meshes to merge
                    for (int i = 0; i < meshesToCombine.Count; i++)
                    {
                        SkinnedMeshRenderer currentMesh = meshesToCombine[i];
#if UNITY_EDITOR
                        //Collect stats
                        processedVertices += currentMesh.sharedMesh.vertexCount;
#endif
                        //Obtains the data of each submesh of current mesh
                        for (int ii = 0; ii < currentMesh.sharedMesh.subMeshCount; ii++)
                        {
                            //Add bone to list of bones to merge and set bones bindposes
                            Transform[] currentMeshBones = currentMesh.bones;
                            for (int iii = 0; iii < currentMeshBones.Length; iii++)
                            {
                                bonesToMerge.Add(currentMeshBones[iii]);
                                if (compatibilityMode == true)
                                {
                                    bindPosesToMerge.Add(currentMesh.sharedMesh.bindposes[iii] * currentMesh.transform.worldToLocalMatrix);
                                }
                                if (compatibilityMode == false)
                                {
                                    bindPosesToMerge.Add(currentMeshBones[iii].worldToLocalMatrix * currentMesh.transform.worldToLocalMatrix);
                                }
                            }
                            //Configure the Combine Instances
                            CombineInstance combineInstance = new CombineInstance();
                            combineInstance.mesh = currentMesh.sharedMesh;
                            combineInstance.subMeshIndex = ii;
                            combineInstance.transform = currentMesh.transform.localToWorldMatrix;
                            combinesToMerge.Add(combineInstance);
                            //Add the vertices count of this submesh to index of meshes
                            meshesVerticesIndex.Add(combineInstance.mesh.GetSubmesh(ii).vertexCount);
                            //Get color of material of current mesh and create a texture of them
                            Texture2D textureOfColor = new Texture2D(64, 64, GetSelectedFormatOfAtlas(), false, atlasLinearFilter);
                            Color[] color = new Color[64 * 64];
                            for(int iii = 0; iii < color.Length; iii++)
                            {
                                color[iii] = currentMesh.sharedMaterials[ii].color;
                            }
                            textureOfColor.SetPixels(0, 0, 64, 64, color);
                            //Add the texture to textures to merge
                            texturesToMerge.Add(textureOfColor);
                        }
                    }

                    //Enable "Always Animate" in Animator
                    AnimatorCullingMode oldModeAnimator = GetComponent<Animator>().cullingMode;
                    GetComponent<Animator>().cullingMode = AnimatorCullingMode.AlwaysAnimate;
                    //Create a GameObject to store merged meshes
                    GameObject combinedMeshesOBJ = new GameObject("Combined Mesh");
                    combinedMeshesOBJ.transform.SetParent(this.transform);
                    //Create GameObject with merged mesh
                    GameObject meshCombinedOBJ = new GameObject("Mesh (Just Material Colors)");
                    meshCombinedOBJ.transform.SetParent(combinedMeshesOBJ.transform);
                    //Configure the Renderer to the merged mesh
                    SkinnedMeshRenderer meshCombinedSMR = meshCombinedOBJ.AddComponent<SkinnedMeshRenderer>();
                    if (qualityOfAnim == AnimQuality.UseQualitySettings) { meshCombinedSMR.quality = SkinQuality.Auto; }
                    if (qualityOfAnim == AnimQuality.Bad) { meshCombinedSMR.quality = SkinQuality.Bone1; }
                    if (qualityOfAnim == AnimQuality.Good) { meshCombinedSMR.quality = SkinQuality.Bone2; }
                    if (qualityOfAnim == AnimQuality.VeryGood) { meshCombinedSMR.quality = SkinQuality.Bone4; }
                    meshCombinedSMR.rootBone = meshesToCombine[0].rootBone;
                    meshCombinedSMR.updateWhenOffscreen = updateWhenOffScreen;
                    meshCombinedSMR.skinnedMotionVectors = skinnedMotionVectors;
                    //Create the merged mesh, and configure the limitation of vertices
                    meshCombinedSMR.sharedMesh = new Mesh();
                    if (moreThan65kVertices == false)
                        meshCombinedSMR.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
                    if (moreThan65kVertices == true)
                        meshCombinedSMR.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                    meshCombinedSMR.sharedMesh.CombineMeshes(combinesToMerge.ToArray(), true, true);
                    meshCombinedSMR.sharedMesh.name = "Combined Mesh";
                    //Apply tem generated material to the merged mesh
                    meshCombinedSMR.sharedMaterials = GetSelectedMaterialInListOfMaterials();
                    //Pass the data to the new merged mesh
                    meshCombinedSMR.bones = bonesToMerge.ToArray();
                    meshCombinedSMR.sharedMesh.bindposes = bindPosesToMerge.ToArray();
                    meshCombinedSMR.sharedMesh.RecalculateBounds();
                    //Deactive the old meshes not merged
                    foreach (SkinnedMeshRenderer oldMesh in meshesToMerge)
                    {
                        if (oldMesh != null)
                        {
                            oldMesh.gameObject.SetActive(false);
                        }
                    }
                    //Enable the old mode of culling in Animator
                    GetComponent<Animator>().cullingMode = oldModeAnimator;

                    //Create Atlas texture
                    //Get textures to merge, converted with edges to support mipmapsm, if mipmaps activated
                    Texture2D atlasTexture = new Texture2D(16, 16);
                    Rect[] atlasRects = atlasTexture.PackTextures(texturesToMerge.ToArray(), PADDING_OF_ATLAS, 256, false);

                    //Set the atlas in material
                    meshCombinedSMR.sharedMaterials[0].mainTexture = atlasTexture;
#if UNITY_EDITOR
                    atlasMergedInAllInOne = meshCombinedSMR.sharedMaterials[0].mainTexture;
#endif
                    //Prepare the UVs array
                    Vector2[] originalCombinedUVs = meshCombinedSMR.sharedMesh.uv;
                    Vector2[] newCombinedUVs = new Vector2[originalCombinedUVs.Length];
                    //Change all vertex UVs to positive
                    for (int i = 0; i < originalCombinedUVs.Length; i++)
                    {
                        if (originalCombinedUVs[i].x < 0) { originalCombinedUVs[i].x = originalCombinedUVs[i].x * -1; };
                        if (originalCombinedUVs[i].y < 0) { originalCombinedUVs[i].y = originalCombinedUVs[i].y * -1; };
                    }
                    //Resizes the uvs to the atlas, uv of each mesh in center of respective texture
                    int currentMeshOffset = meshesVerticesIndex[0];
                    int currentMeshUV = 0;
                    for (int i = 0; i < originalCombinedUVs.Length; i++)
                    {
                        //Verifies which mesh this vertex belongs to
                        if (i >= currentMeshOffset)
                        {
                            //Verify if the current mesh UV is in the end, before add more
                            if (currentMeshUV < meshesVerticesIndex.Count - 1)
                                currentMeshUV += 1;
                            currentMeshOffset += meshesVerticesIndex[currentMeshUV];
                        }

                        newCombinedUVs[i].x = Mathf.Lerp(atlasRects[currentMeshUV].xMin, atlasRects[currentMeshUV].xMax, Mathf.Lerp(0.45f, 0.55f, originalCombinedUVs[i].x));
                        newCombinedUVs[i].y = Mathf.Lerp(atlasRects[currentMeshUV].yMin, atlasRects[currentMeshUV].yMax, Mathf.Lerp(0.45f, 0.55f, originalCombinedUVs[i].y));
                    }
                    //Apply the new UV map
                    meshCombinedSMR.sharedMesh.uv = newCombinedUVs;

                    //Show UV vertices in all atlas
                    if (showUvVerticesInAtlas == true)
                    {
                        for (int i = 0; i < meshCombinedSMR.sharedMesh.uv.Length; i++)
                        {
                            atlasTexture.SetPixel((int)(atlasTexture.width * meshCombinedSMR.sharedMesh.uv[i].x), (int)(atlasTexture.height * meshCombinedSMR.sharedMesh.uv[i].y), Color.yellow);
                        }
                        atlasTexture.Apply();
                    }

#if UNITY_EDITOR
                    //Time stats
                    timeMonitor.Stop();
                    //Creates the asset
                    if (isEditor == true && saveDataInAssets == true && Application.isPlaying == false)
                    {
                        Editor_CreateDirectory();
                        string sceneName = SceneManager.GetActiveScene().name;
                        DateTime dateTime = new DateTime();
                        dateTime = DateTime.Now;

                        if (exportTexturesAsPng == true)
                        {
                            if (!AssetDatabase.IsValidFolder("Assets/_Exported"))
                            {
                                AssetDatabase.CreateFolder("Assets", "_Exported");
                            }
                        }

                        AssetDatabase.CreateAsset(meshCombinedSMR.sharedMesh, "Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Mesh/" + this.gameObject.name + " (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                        pathsOfDataSavedInAssets.Add("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Mesh/" + this.gameObject.name + " (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                        AssetDatabase.CreateAsset(meshCombinedSMR.sharedMaterials[0].mainTexture, "Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Texture/" + this.gameObject.name + " (MainTexture) (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                        pathsOfDataSavedInAssets.Add("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Texture/" + this.gameObject.name + " (MainTexture) (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                        if (exportTexturesAsPng == true)
                        {
                            Texture2D mainTexture = meshCombinedSMR.sharedMaterials[0].mainTexture as Texture2D;
                            byte[] mainTextureBytes = mainTexture.EncodeToPNG();
                            File.WriteAllBytes("Assets/_Exported/" + nameOfTextures + " (MainTexture).png", mainTextureBytes);
                        }
                        AssetDatabase.CreateAsset(meshCombinedSMR.sharedMaterials[0], "Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Material/" + this.gameObject.name + " (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                        pathsOfDataSavedInAssets.Add("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Material/" + this.gameObject.name + " (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                        if (savePrefabOfThis == true)
                        {
                            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                            {
                                AssetDatabase.CreateFolder("Assets", "Prefabs");
                            }
                            if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Skinned Mesh Combiner"))
                            {
                                AssetDatabase.CreateFolder("Assets/Prefabs", "Skinned Mesh Combiner");
                            }
                            if (AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Skinned Mesh Combiner/" + prefabName + ".prefab", typeof(GameObject)) != null)
                            {
                                LaunchLog(LogType.Log, "Prefab \"" + prefabName + "\" already exists in your project files. Therefore, a new file was not created. You can update the existing prefab by clicking \"Apply\" here in the Inspector window.");
                            }
                            if (AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Skinned Mesh Combiner/" + prefabName + ".prefab", typeof(GameObject)) == null)
                            {
                                UnityEngine.Object prefab = PrefabUtility.CreatePrefab("Assets/Prefabs/Skinned Mesh Combiner/" + prefabName + ".prefab", this.gameObject);
                                PrefabUtility.ReplacePrefab(this.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
                                LaunchLog(LogType.Log, "The prefab \"" + prefabName + "\" was created in your project files! The path to the prefabs that the Skinned Mesh Combiner creates is the \"Prefabs/Skinned Mesh Combiner\"");
                            }
                        }
                        //Refresh the project explorer
                        AssetDatabase.Refresh();

                        Editor_ShowDialog();
                    }
                    if (isEditor == true && Application.isPlaying == false)
                    {
                        //Create string of stats
                        statsOfMerge = "The Skinned Mesh children of this GameObject were combined. See the optimization statistics below!"
                        + "\n\n"
                        + "Merge Method: Just Material Colors"
                        + "\n"
                        + "Processed Vertices: " + processedVertices.ToString()
                        + "\n"
                        + "Processing time: " + ((double)(timeMonitor.ElapsedMilliseconds) / (double)(1000)).ToString() + "s"
                        + "\n"
                        + "Combined Meshes: " + mergeMeshesCount.ToString()
                        + "\n"
                        + "Mesh Count Before: " + meshesCountBefore.ToString()
                        + "\n"
                        + "Mesh Count After: " + ((meshesCountBefore - mergeMeshesCount) + 1).ToString()
                        + "\n"
                        + "Draw Call Reduction: ≥" + (mergeMeshesCount - 1).ToString()
                        + "\n"
                        + "Materials Generated: 1"
                        + "\n"
                        + "Atlas Resolution: " + 256 + "x" + 256 + "p"
                        + "\n"
                        + "Packaged Textures: " + texturesToMerge.Count.ToString()
                        + "\n"
                        + "Optimization rate: 100%"
                        + "\n\n"
                        + "- Statistics are only generated when combined using the Inspector.";
                    }
                    if (isEditor == true && Application.isPlaying == true)
                    {
                        statsOfMerge = "You can not generate statistics while the game is running or outside the Editor.";
                    }
#endif

                    //Set variable as merged
                    isMeshesCombineds = true;
#if UNITY_EDITOR
                    //Notify
                    LaunchLog(LogType.Log, "The merge completed successfully. View the merge statistics in the \"Stats\" tab.");
#endif
                }
            }

            yield return null;
        }

        private IEnumerator MergeMeshes_OnlyAnima2dMeshes(bool isEditor)
        {
#if MTAssets_Anima2D_Available
            //Checks if the meshes are already combined.
            if (isMeshesCombineds == true)
            {
                LaunchLog(LogType.Error, "The \"" + this.transform.name + "\" meshes are already combined!");
            }
            if (isMeshesCombineds == false)
            {
#if UNITY_EDITOR
                //Time stats
                System.Diagnostics.Stopwatch timeMonitor = new System.Diagnostics.Stopwatch();
                timeMonitor.Start();
#endif
                //Find the sprite meshes instance to merge
                spriteMeshInstances = GetComponentsInChildren<SpriteMeshInstance>(combineInactives);

                //Mesh and submeshes list to be combined
                List<SpriteMeshInstance> spriteMeshInstancesToCombine = new List<SpriteMeshInstance>();
                List<Texture2D> spriteTexturesToCombine = new List<Texture2D>();
                List<int> meshesVertexIndex = new List<int>();

                //Gets the valid meshes that are to be merged
                for (int i = 0; i < spriteMeshInstances.Length; i++)
                {
                    //Checks if the mesh is null
                    if (spriteMeshInstances[i].spriteMesh == null)
                    {
                        LaunchLog(LogType.Log, "The \"" + spriteMeshInstances[i].transform.name + "\" does not have an associated Sprite Mesh. Insert a Sprite Mesh in this Sprite Mesh Instance.");
                        spriteMeshInstances[i] = null;
                        continue;
                    }
                    //Checks whether the mesh should be skipped
                    if (meshesToIgnoreOfAnima2dMergeMethod.Contains(spriteMeshInstances[i]) == true)
                    {
                        LaunchLog(LogType.Log, "The \"" + spriteMeshInstances[i].transform.name + "\" sprite mesh instance was skipped during merge. This mesh was registered to be ignored.");
                        spriteMeshInstances[i] = null;
                        continue;
                    }
                    //Check if the mesh is without material
                    if (spriteMeshInstances[i].sharedMaterial == null)
                    {
                        LaunchLog(LogType.Log, "The \"" + spriteMeshInstances[i].transform.name + "\" sprite mesh instance does not have any material associated with it. Associate 1 material so it can be combined.");
                        spriteMeshInstances[i] = null;
                        continue;
                    }

                    //Keep the current mesh
                    spriteMeshInstancesToCombine.Add(spriteMeshInstances[i]);
                    spriteTexturesToCombine.Add(spriteMeshInstances[i].spriteMesh.sprite.texture);
                    meshesVertexIndex.Add(spriteMeshInstances[i].spriteMesh.sharedMesh.vertexCount);
                }

                //Checks whether the resulting mesh will have more than 65500 vertices, if support for more than 65k vertices is disabled
                bool canContinue = true;
                int verticesCount = 0;
                foreach (SpriteMeshInstance mesh in spriteMeshInstancesToCombine)
                {
                    verticesCount += mesh.sharedMesh.vertexCount;
                }
                if (verticesCount >= 65500)
                {
                    canContinue = false;
                    LaunchLog(LogType.Error, "The GameObject \"" + this.gameObject.name + "\" merge has been canceled because the resulting mesh will have more than 65,000 vertices. The mesh resulting from the combination would have more than " + verticesCount.ToString() + "" +
                        " vertices.\n\nYou may consider enabling the \"More Than 65k Vertices\" option if you want to merge meshes without worrying about vertex limitations.");
                }

                //Verify if exists meshes to merge
                bool existsMeshesToMerge = true;
                //Verify quantity of objetcs
                if (spriteMeshInstances.Length == 0)
                {
                    LaunchLog(LogType.Error, "The \"" + this.transform.name + "\" merge was canceled because there are not enough meshes to merge.");
                    existsMeshesToMerge = false;
                }
                //Verify if all is null
                int nullMeshes = 0;
                foreach (SpriteMeshInstance obj in spriteMeshInstancesToCombine)
                {
                    if (obj == null) { nullMeshes += 1; };
                }
                if (nullMeshes >= spriteMeshInstances.Length)
                {
                    LaunchLog(LogType.Error, "The \"" + this.transform.name + "\" merge was canceled because there are not enough meshes to merge.");
                    existsMeshesToMerge = false;
                }

                //Prepare for read textures
                bool texturesReadable = true;
                //Create the atlas texture from sprites
                Texture2D atlasTexure = new Texture2D(16, 16);
                Rect[] atlasRects = new Rect[0];
                try
                {
                    atlasRects = atlasTexure.PackTextures(spriteTexturesToCombine.ToArray(), PADDING_OF_ATLAS, GetSelectedResolutionOfAtlas(), false);
                }
                catch (UnityException e) { texturesReadable = false; LaunchLog(LogType.Error, ERROR_MESSAGE_ON_TRY_READ_TEXTURES + e.Message); }
                //Show tip for enable Read/Write in log if textures is not readable
                if (texturesReadable == false)
                {
                    LaunchLog(LogType.Log, "It looks like there was an error trying to read the textures to merge them. Verify that the textures to be merged are with the \"Read/Write Enabled\" option in the Unity import settings.");
                }

                //Inicialize the merge
                if (existsMeshesToMerge == true && canContinue == true && texturesReadable == true)
                {
#if UNITY_EDITOR
                    //Collect stats
                    int mergeMeshesCount = spriteMeshInstancesToCombine.Count;
                    int meshesCountBefore = spriteMeshInstances.Length;
#endif

                    Vector3 position = transform.position;
                    Quaternion rotation = transform.rotation;
                    Vector3 scale = transform.localScale;

                    transform.position = Vector3.zero;
                    transform.rotation = Quaternion.identity;
                    transform.localScale = Vector3.one;

                    List<Transform> bones = new List<Transform>();
                    List<BoneWeight> boneWeights = new List<BoneWeight>();
                    List<CombineInstance> combineInstances = new List<CombineInstance>();

                    int numSubmeshes = 0;

                    for (int i = 0; i < spriteMeshInstancesToCombine.Count; i++)
                    {
                        SpriteMeshInstance spriteMesh = spriteMeshInstancesToCombine[i];

                        if (spriteMesh.cachedSkinnedRenderer)
                        {
                            numSubmeshes += spriteMesh.mesh.subMeshCount;
                        }
                    }

                    int[] meshIndex = new int[numSubmeshes];
                    int boneOffset = 0;
                    for (int i = 0; i < spriteMeshInstancesToCombine.Count; ++i)
                    {
                        SpriteMeshInstance spriteMesh = spriteMeshInstancesToCombine[i];

                        if (spriteMesh.cachedSkinnedRenderer)
                        {
                            SkinnedMeshRenderer skinnedMeshRenderer = spriteMesh.cachedSkinnedRenderer;

                            BoneWeight[] meshBoneweight = spriteMesh.sharedMesh.boneWeights;

                            // May want to modify this if the renderer shares bones as unnecessary bones will get added.
                            for (int j = 0; j < meshBoneweight.Length; ++j)
                            {
                                BoneWeight bw = meshBoneweight[j];
                                BoneWeight bWeight = bw;
                                bWeight.boneIndex0 += boneOffset;
                                bWeight.boneIndex1 += boneOffset;
                                bWeight.boneIndex2 += boneOffset;
                                bWeight.boneIndex3 += boneOffset;
                                boneWeights.Add(bWeight);
                            }

                            boneOffset += spriteMesh.bones.Count;

                            Transform[] meshBones = skinnedMeshRenderer.bones;
                            for (int j = 0; j < meshBones.Length; j++)
                            {
                                Transform bone = meshBones[j];
                                bones.Add(bone);
                            }

                            CombineInstance combineInstance = new CombineInstance();
                            Mesh mesh = new Mesh();
                            skinnedMeshRenderer.BakeMesh(mesh);
                            mesh.uv = spriteMesh.spriteMesh.sprite.uv;
                            combineInstance.mesh = mesh;
                            meshIndex[i] = combineInstance.mesh.vertexCount;
                            combineInstance.transform = skinnedMeshRenderer.localToWorldMatrix;
                            combineInstances.Add(combineInstance);

                            skinnedMeshRenderer.gameObject.SetActive(false);
                        }
                    }

                    //Set the bind poses
                    List<Matrix4x4> bindposes = new List<Matrix4x4>();

                    for (int b = 0; b < bones.Count; b++)
                    {
                        if (compatibilityMode == true)
                        {
                            bindposes.Add(bones[b].worldToLocalMatrix * transform.worldToLocalMatrix);
                        }
                        if (compatibilityMode == false)
                        {
                            bindposes.Add(bones[b].worldToLocalMatrix * transform.worldToLocalMatrix);
                        }
                    }

                    //Enable "Always Animate" in Animator
                    AnimatorCullingMode oldModeAnimator = GetComponent<Animator>().cullingMode;
                    GetComponent<Animator>().cullingMode = AnimatorCullingMode.AlwaysAnimate;
                    //Create a GameObject to store merged meshes
                    GameObject combinedMeshesOBJ = new GameObject("Combined Mesh");
                    combinedMeshesOBJ.transform.SetParent(this.transform);
                    //Create GameObject with merged mesh
                    GameObject meshCombinedOBJ = new GameObject("Mesh (Only Anima2D Mesh)");
                    meshCombinedOBJ.transform.SetParent(combinedMeshesOBJ.transform);

                    //Create and configure the skinned mesh renderer of merged mesh
                    SkinnedMeshRenderer meshCombinedSMR = meshCombinedOBJ.AddComponent<SkinnedMeshRenderer>();
                    if (qualityOfAnim == AnimQuality.UseQualitySettings) { meshCombinedSMR.quality = SkinQuality.Auto; }
                    if (qualityOfAnim == AnimQuality.Bad) { meshCombinedSMR.quality = SkinQuality.Bone1; }
                    if (qualityOfAnim == AnimQuality.Good) { meshCombinedSMR.quality = SkinQuality.Bone2; }
                    if (qualityOfAnim == AnimQuality.VeryGood) { meshCombinedSMR.quality = SkinQuality.Bone4; }
                    meshCombinedSMR.updateWhenOffscreen = updateWhenOffScreen;
                    meshCombinedSMR.skinnedMotionVectors = skinnedMotionVectors;
                    Mesh combinedMesh = new Mesh();
                    if (moreThan65kVertices == false)
                        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
                    if (moreThan65kVertices == true)
                        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                    combinedMesh.name = "Combined Mesh";
                    combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);
                    meshCombinedSMR.sharedMesh = combinedMesh;
                    meshCombinedSMR.bones = bones.ToArray();
                    meshCombinedSMR.sharedMesh.boneWeights = boneWeights.ToArray();
                    meshCombinedSMR.sharedMesh.bindposes = bindposes.ToArray();
                    meshCombinedSMR.sharedMesh.RecalculateBounds();

                    //Start the modification of UV of merged mesh, to receive atlas sprite
                    //Prepare the UVs array
                    Vector2[] originalCombinedUVs = meshCombinedSMR.sharedMesh.uv;
                    Vector2[] newCombinedUVs = new Vector2[originalCombinedUVs.Length];
                    //Resizes the uvs to the atlas so that it ignores the edges of each texture
                    int currentMeshOffset = meshesVertexIndex[0];
                    int currentMeshUV = 0;
                    for (int i = 0; i < originalCombinedUVs.Length; i++)
                    {
                        //Verifies which mesh this vertex belongs to
                        if (i >= currentMeshOffset)
                        {
                            //Verify if the current mesh UV is in the end, before add more
                            if (currentMeshUV < meshesVertexIndex.Count - 1)
                                currentMeshUV += 1;
                            currentMeshOffset += meshesVertexIndex[currentMeshUV];
                        }

                        //If the UV is not larger than the texture
                        newCombinedUVs[i].x = Mathf.Lerp(atlasRects[currentMeshUV].xMin, atlasRects[currentMeshUV].xMax, originalCombinedUVs[i].x);
                        newCombinedUVs[i].y = Mathf.Lerp(atlasRects[currentMeshUV].yMin, atlasRects[currentMeshUV].yMax, originalCombinedUVs[i].y);
                    }
                    //Apply the new UV map
                    meshCombinedSMR.sharedMesh.uv = newCombinedUVs;

#if UNITY_EDITOR
                    //Show atlas of merged meshes in stats tab
                    atlasMergedInAllInOne = atlasTexure;
#endif

                    //Apply the script responsible for render the sprite in merged mesh
                    RendererOfCombinedAnima2D rendererOfCombinedAnima2D = meshCombinedOBJ.AddComponent<RendererOfCombinedAnima2D>();
                    rendererOfCombinedAnima2D.cachedSkinnedRenderer = meshCombinedSMR;
                    rendererOfCombinedAnima2D.materialPropertyBlock = new MaterialPropertyBlock();
                    rendererOfCombinedAnima2D.atlasForRenderInChar = atlasTexure;
                    rendererOfCombinedAnima2D.atlasForRenderInChar.name = this.gameObject.name + " (MainTexture)";
                    rendererOfCombinedAnima2D.rootGameObject = this.gameObject;

                    //Set the material of merged mesh
                    meshCombinedSMR.materials = spriteMeshInstancesToCombine[0].sharedMaterials;

                    //REturn the original positions to new mesh
                    transform.position = position;
                    transform.rotation = rotation;
                    transform.localScale = scale;

                    //Show UV vertices in all atlas
                    if (showUvVerticesInAtlas == true)
                    {
                        for (int i = 0; i < meshCombinedSMR.sharedMesh.uv.Length; i++)
                        {
                            atlasTexure.SetPixel((int)(atlasTexure.width * meshCombinedSMR.sharedMesh.uv[i].x), (int)(atlasTexure.height * meshCombinedSMR.sharedMesh.uv[i].y), Color.yellow);
                        }
                        atlasTexure.Apply();
                    }

#if UNITY_EDITOR
                    //Time stats
                    timeMonitor.Stop();
                    //Creates the asset
                    if (isEditor == true && saveDataInAssets == true && Application.isPlaying == false)
                    {
                        Editor_CreateDirectory();
                        string sceneName = SceneManager.GetActiveScene().name;
                        DateTime dateTime = new DateTime();
                        dateTime = DateTime.Now;

                        if (exportTexturesAsPng == true)
                        {
                            if (!AssetDatabase.IsValidFolder("Assets/_Exported"))
                            {
                                AssetDatabase.CreateFolder("Assets", "_Exported");
                            }
                        }

                        AssetDatabase.CreateAsset(meshCombinedSMR.sharedMesh, "Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Mesh/" + this.gameObject.name + " (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                        pathsOfDataSavedInAssets.Add("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Mesh/" + this.gameObject.name + " (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                        AssetDatabase.CreateAsset(rendererOfCombinedAnima2D.atlasForRenderInChar, "Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Texture/" + this.gameObject.name + " (MainTexture) (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                        pathsOfDataSavedInAssets.Add("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Texture/" + this.gameObject.name + " (MainTexture) (" + dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond.ToString() + ").asset");
                        if (exportTexturesAsPng == true)
                        {
                            Texture2D mainTexture = rendererOfCombinedAnima2D.atlasForRenderInChar as Texture2D;
                            byte[] mainTextureBytes = mainTexture.EncodeToPNG();
                            File.WriteAllBytes("Assets/_Exported/" + nameOfTextures + " (MainTexture).png", mainTextureBytes);
                        }

                        if (savePrefabOfThis == true)
                        {
                            LaunchLog(LogType.Log, "This merge method does not support the creation of prefabs. Please note that the creation of prefabs from merged meshes of Anima2D is not 100% supported by Anima2D. A good way to work around this problem is to copy the root GameObject of your character, so all the merge properties will remain. You can also create a non-merged character prefab and merge all instances individually by using the Skinned Mesh Combiner API at runtime or the Editor.");
                        }

                        //Refresh the project explorer
                        AssetDatabase.Refresh();

                        Editor_ShowDialog();
                    }
                    if (isEditor == true && Application.isPlaying == false)
                    {
                        //Create string of stats
                        statsOfMerge = "The Skinned Mesh children of this GameObject were combined. See the optimization statistics below!"
                        + "\n\n"
                        + "Merge Method: Only Anima2D Meshes"
                        + "\n"
                        + "Processed Vertices: " + meshCombinedSMR.sharedMesh.vertexCount.ToString()
                        + "\n"
                        + "Processing time: " + ((double)(timeMonitor.ElapsedMilliseconds) / (double)(1000)).ToString() + "s"
                        + "\n"
                        + "Combined Meshes: " + mergeMeshesCount.ToString()
                        + "\n"
                        + "Mesh Count Before: " + meshesCountBefore.ToString()
                        + "\n"
                        + "Mesh Count After: " + ((meshesCountBefore - mergeMeshesCount) + 1).ToString()
                        + "\n"
                        + "Draw Call Reduction: ≥" + (mergeMeshesCount - 1).ToString()
                        + "\n"
                        + "Materials Generated: 1"
                        + "\n"
                        + "Optimization rate: 100%"
                        + "\n\n"
                        + "- Statistics are only generated when combined using the Inspector.";
                    }
                    if (isEditor == true && Application.isPlaying == true)
                    {
                        statsOfMerge = "You can not generate statistics while the game is running or outside the Editor.";
                    }
#endif

                    //Set variable as merged
                    isMeshesCombineds = true;
#if UNITY_EDITOR
                    //Notify
                    LaunchLog(LogType.Log, "The merge completed successfully. View the merge statistics in the \"Stats\" tab.");
#endif
                }
            }
#else
            //Notify if Anima2D was not found
            LaunchLog(LogType.Error, "Blending can not be performed. The current merge method you selected does not work because it depends on the existence of the Anima2D tool in your project. This merge method was exclusively developed for Anima2D. If you want to merge 3D meshes, use other merge methods.");
#endif
            yield return null;
        }

        private int GetSelectedResolutionOfAtlas()
        {
            //Configure resolution of atlas
            switch (atlasResolution)
            {
                case AtlasSize.Pixels32x32: return 32;
                case AtlasSize.Pixels64x64: return 64;
                case AtlasSize.Pixels128x128: return 128;
                case AtlasSize.Pixels256x256: return 256;
                case AtlasSize.Pixels512x512: return 512;
                case AtlasSize.Pixels1024x1024: return 1024;
                case AtlasSize.Pixels2048x2048: return 2048;
                case AtlasSize.Pixels4096x4096: return 4096;
                case AtlasSize.Pixels8192x8192: return 8192;
            }

            return 0;
        }

        private TextureFormat GetSelectedFormatOfAtlas()
        {
            //Configure format of atlas
            switch (atlasFormat)
            {
                case AtlasFormat.RGBA16Bits: return TextureFormat.ARGB4444;
                case AtlasFormat.RGBA32Bits: return TextureFormat.ARGB32;
            }

            return TextureFormat.ARGB32;
        }

        private Material[] GetSelectedMaterialInListOfMaterials()
        {
            //Generate material and return it

            if (materialType == MaterialType.CustomMaterial)
            {
                Material mat = new Material(materialCustom);
                mat.CopyPropertiesFromMaterial(materialCustom);
                mat.name = "Custom Material Of Combined Mesh";
                return new Material[] { mat };
            }

            if (materialType == MaterialType.InternalMaterial)
            {
                Material mat = new Material(Shader.Find("MT Assets/Skinned Mesh Combiner/Light VertexLit"));

                switch (internalMaterialList)
                {
                    case ListInternalMaterial.ProDiffuseCutout:
                        mat = new Material(Shader.Find("MT Assets/Skinned Mesh Combiner/Pro Diffuse Cutout"));
                        break;
                    case ListInternalMaterial.ProDiffuseCutoutCullingOff:
                        mat = new Material(Shader.Find("MT Assets/Skinned Mesh Combiner/Pro Diffuse Cutout (Culling Off)"));
                        break;
                    case ListInternalMaterial.LightVertexLit:
                        mat = new Material(Shader.Find("MT Assets/Skinned Mesh Combiner/Light VertexLit"));
                        break;
                    case ListInternalMaterial.LightVertexLitCullingOff:
                        mat = new Material(Shader.Find("MT Assets/Skinned Mesh Combiner/Light VertexLit (Culling Off)"));
                        break;
                    case ListInternalMaterial.StandardMetallic:
                        mat = new Material(Shader.Find("MT Assets/Skinned Mesh Combiner/Standard"));
                        break;
                    case ListInternalMaterial.StandardMetallicCullingOff:
                        mat = new Material(Shader.Find("MT Assets/Skinned Mesh Combiner/Standard (Culling Off)"));
                        break;
                    case ListInternalMaterial.StandardSpecular:
                        mat = new Material(Shader.Find("MT Assets/Skinned Mesh Combiner/Standard Specular"));
                        break;
                    case ListInternalMaterial.StandardSpecularCullingOff:
                        mat = new Material(Shader.Find("MT Assets/Skinned Mesh Combiner/Standard Specular (Culling Off)"));
                        break;
                }

                mat.name = "Pre-Built Material Of Combined Mesh";
                //Apply changes if is standard
                if (internalMaterialList == ListInternalMaterial.StandardMetallic || internalMaterialList == ListInternalMaterial.StandardMetallicCullingOff ||
                    internalMaterialList == ListInternalMaterial.StandardSpecular || internalMaterialList == ListInternalMaterial.StandardSpecularCullingOff)
                {
                    switch (renderMode)
                    {
                        case RenderMode.Opaque:
                            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                            mat.SetInt("_ZWrite", 1);
                            mat.DisableKeyword("_ALPHATEST_ON");
                            mat.DisableKeyword("_ALPHABLEND_ON");
                            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                            mat.renderQueue = -1;
                            break;
                        case RenderMode.Cutout:
                            mat.SetFloat("_Cutoff", alphaCutOff);

                            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                            mat.SetInt("_ZWrite", 1);
                            mat.EnableKeyword("_ALPHATEST_ON");
                            mat.DisableKeyword("_ALPHABLEND_ON");
                            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                            mat.renderQueue = 2450;
                            break;
                        case RenderMode.Fade:
                            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                            mat.SetInt("_ZWrite", 0);
                            mat.DisableKeyword("_ALPHATEST_ON");
                            mat.EnableKeyword("_ALPHABLEND_ON");
                            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                            mat.renderQueue = 3000;
                            break;
                        case RenderMode.Transparent:
                            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                            mat.SetInt("_ZWrite", 0);
                            mat.DisableKeyword("_ALPHATEST_ON");
                            mat.DisableKeyword("_ALPHABLEND_ON");
                            mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                            mat.renderQueue = 3000;
                            break;
                    }
                    if(internalMaterialList == ListInternalMaterial.StandardMetallic || internalMaterialList == ListInternalMaterial.StandardMetallicCullingOff)
                    {
                        mat.SetFloat("_Metallic", metallic);
                    }
                    if (internalMaterialList == ListInternalMaterial.StandardSpecular || internalMaterialList == ListInternalMaterial.StandardSpecularCullingOff)
                    {
                        mat.SetColor("_SpecColor", specular);
                    }
                    if(metallicMapSupport == false && specularMapSupport == false)
                    {
                        mat.SetFloat("_Glossiness", smoothness);
                    }
                    if (metallicMapSupport == true || specularMapSupport == true)
                    {
                        mat.SetFloat("_GlossMapScale", smoothness);
                    }
                    mat.SetFloat("_SpecularHighlights", (specularHighlights == true) ? 1.0f : 0f);
                    if (specularHighlights == true)
                        mat.DisableKeyword("_SPECULARHIGHLIGHTS_OFF");
                    if (specularHighlights == false)
                        mat.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
                    mat.SetFloat("_GlossyReflections", (reflections == true) ? 1.0f : 0f);
                    if (reflections == true)
                        mat.DisableKeyword("_GLOSSYREFLECTIONS_OFF");
                    if (reflections == false)
                        mat.EnableKeyword("_GLOSSYREFLECTIONS_OFF");
                }
                return new Material[] { mat };
            }

            return null;
        }

        private List<Texture2D> GetConvertedTexturesToMergeForSupportMipMaps(List<Texture2D> texturesToMerge)
        {
            //Creates new textures that support mipmaps in atlas, based on the original textures
            List<Texture2D> texturesToMergeCompatibleWithMipMaps = new List<Texture2D>();
            for (int i = 0; i < texturesToMerge.Count; i++)
            {
                //Checks whether the texture has already been created and repeats it in the list
                bool haveCurrentTexture = false;
                for (int ii = 0; ii < texturesToMergeCompatibleWithMipMaps.Count; ii++)
                {
                    if (texturesToMergeCompatibleWithMipMaps[ii].name.Equals(texturesToMerge[i].GetInstanceID().ToString()) == true)
                    {
                        texturesToMergeCompatibleWithMipMaps.Add(texturesToMergeCompatibleWithMipMaps[ii]);
                        haveCurrentTexture = true;
                        break;
                    }
                }
                //Jumps to the next texture because it has been duplicated
                if (haveCurrentTexture == true)
                {
                    continue;
                }

                //Create a texture with edge of SIZE_OF_EDGES_TEXTURES if no exists in list of textures with mipmap support
                if (texturesToMergeCompatibleWithMipMaps.Contains(texturesToMerge[i]) == false)
                {
                    texturesToMergeCompatibleWithMipMaps.Add(new Texture2D(texturesToMerge[i].width + SIZE_OF_EDGES_TEXTURES, texturesToMerge[i].height + SIZE_OF_EDGES_TEXTURES, GetSelectedFormatOfAtlas(), atlasMipMap, atlasLinearFilter));
                    texturesToMergeCompatibleWithMipMaps[i].name = texturesToMerge[i].GetInstanceID().ToString();
                }

                //Copy pixels of original texture to center of current new texture
                for (int x = 0; x < texturesToMerge[i].width; x++)
                {
                    for (int y = 0; y < texturesToMerge[i].height; y++)
                    {
                        texturesToMergeCompatibleWithMipMaps[i].SetPixel(x + (SIZE_OF_EDGES_TEXTURES / 2), y + (SIZE_OF_EDGES_TEXTURES / 2), texturesToMerge[i].GetPixel(x, y));
                    }
                }
                //Copy right (original) border to left of current texture
                for (int x = 0; x < SIZE_OF_EDGES_TEXTURES / 2; x++)
                {
                    for (int y = 0; y < texturesToMerge[i].height; y++)
                    {
                        texturesToMergeCompatibleWithMipMaps[i].SetPixel(x, y + (SIZE_OF_EDGES_TEXTURES / 2), texturesToMerge[i].GetPixel((texturesToMerge[i].width - (SIZE_OF_EDGES_TEXTURES / 2)) + x, y));
                    }
                }

                //Copy left (original) border to right of current texture
                for (int x = texturesToMerge[i].width - (SIZE_OF_EDGES_TEXTURES / 2); x < texturesToMerge[i].width; x++)
                {
                    for (int y = 0; y < texturesToMerge[i].height; y++)
                    {
                        texturesToMergeCompatibleWithMipMaps[i].SetPixel((texturesToMergeCompatibleWithMipMaps[i].width - (SIZE_OF_EDGES_TEXTURES / 2)) + ((SIZE_OF_EDGES_TEXTURES / 2) - (texturesToMerge[i].width - x)), y + (SIZE_OF_EDGES_TEXTURES / 2), texturesToMerge[i].GetPixel((SIZE_OF_EDGES_TEXTURES / 2) - (texturesToMerge[i].width - x), y));
                    }
                }

                //Copy bottom (original) border to top of current texture
                for (int x = 0; x < texturesToMerge[i].width; x++)
                {
                    for (int y = 0; y < SIZE_OF_EDGES_TEXTURES / 2; y++)
                    {
                        texturesToMergeCompatibleWithMipMaps[i].SetPixel(x + (SIZE_OF_EDGES_TEXTURES / 2), y, texturesToMerge[i].GetPixel(x, (texturesToMerge[i].width - (SIZE_OF_EDGES_TEXTURES / 2)) + y));
                    }
                }

                //Copy top (original) border to bottom of current texture
                for (int x = 0; x < texturesToMerge[i].width; x++)
                {
                    for (int y = texturesToMerge[i].height - (SIZE_OF_EDGES_TEXTURES / 2); y < texturesToMerge[i].height; y++)
                    {
                        texturesToMergeCompatibleWithMipMaps[i].SetPixel(x + (SIZE_OF_EDGES_TEXTURES / 2), (texturesToMergeCompatibleWithMipMaps[i].height - (SIZE_OF_EDGES_TEXTURES / 2)) + ((SIZE_OF_EDGES_TEXTURES / 2) - (texturesToMerge[i].height - y)), texturesToMerge[i].GetPixel(x, (SIZE_OF_EDGES_TEXTURES / 2) - (texturesToMerge[i].height - y)));
                    }
                }

                //Copy bottom-left (original) border to top-right of current texture
                for (int x = 0; x < SIZE_OF_EDGES_TEXTURES / 2; x++)
                {
                    for (int y = 0; y < SIZE_OF_EDGES_TEXTURES / 2; y++)
                    {
                        texturesToMergeCompatibleWithMipMaps[i].SetPixel((texturesToMergeCompatibleWithMipMaps[i].width - (SIZE_OF_EDGES_TEXTURES / 2)) + x, (texturesToMergeCompatibleWithMipMaps[i].height - (SIZE_OF_EDGES_TEXTURES / 2)) + y, texturesToMerge[i].GetPixel(x, y));
                    }
                }

                //Copy top-right (original) border to bottom-left of current texture
                for (int x = texturesToMerge[i].width - (SIZE_OF_EDGES_TEXTURES / 2); x < texturesToMerge[i].width; x++)
                {
                    for (int y = texturesToMerge[i].height - (SIZE_OF_EDGES_TEXTURES / 2); y < texturesToMerge[i].height; y++)
                    {
                        texturesToMergeCompatibleWithMipMaps[i].SetPixel((SIZE_OF_EDGES_TEXTURES / 2) - (texturesToMerge[i].width - x), (SIZE_OF_EDGES_TEXTURES / 2) - (texturesToMerge[i].height - y), texturesToMerge[i].GetPixel(x, y));
                    }
                }

                //Copy bottom-right (original) border to top-left of current texture
                for (int x = texturesToMerge[i].width - (SIZE_OF_EDGES_TEXTURES / 2); x < texturesToMerge[i].width; x++)
                {
                    for (int y = 0; y < SIZE_OF_EDGES_TEXTURES / 2; y++)
                    {
                        texturesToMergeCompatibleWithMipMaps[i].SetPixel((SIZE_OF_EDGES_TEXTURES / 2) - (texturesToMerge[i].width - x), (texturesToMergeCompatibleWithMipMaps[i].height - (SIZE_OF_EDGES_TEXTURES / 2)) + y, texturesToMerge[i].GetPixel(x, y));
                    }
                }

                //Copy top-left (original) border to bottom-right of current texture
                for (int x = 0; x < SIZE_OF_EDGES_TEXTURES / 2; x++)
                {
                    for (int y = texturesToMerge[i].height - (SIZE_OF_EDGES_TEXTURES / 2); y < texturesToMerge[i].height; y++)
                    {
                        texturesToMergeCompatibleWithMipMaps[i].SetPixel((texturesToMergeCompatibleWithMipMaps[i].width - (SIZE_OF_EDGES_TEXTURES / 2)) + x, (SIZE_OF_EDGES_TEXTURES / 2) - (texturesToMerge[i].height - y), texturesToMerge[i].GetPixel(x, y));
                    }
                }
            }
            return texturesToMergeCompatibleWithMipMaps;
        }

        private void FindMeshes()
        {
            //Get the skinned mesh renderer children of this gameobject
            meshesToMerge = GetComponentsInChildren<SkinnedMeshRenderer>(combineInactives);
        }

        private bool ValidMesh(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            bool valid = true;

            //Checks if the mesh is null
            if (skinnedMeshRenderer.sharedMesh == null)
            {
                LaunchLog(LogType.Log, "The \"" + skinnedMeshRenderer.transform.name + "\" does not have an associated mesh. Insert a mesh in this Skinned Mesh Renderer.");
                return false;
            }

            //Checks whether the mesh should be skipped
            if (meshesToIgnore.Contains(skinnedMeshRenderer) == true)
            {
                LaunchLog(LogType.Log, "The \"" + skinnedMeshRenderer.transform.name + "\" mesh was skipped during merge. This mesh was registered to be ignored.");
                valid = false;
            }
            //Check if the mesh is without material
            if (skinnedMeshRenderer.sharedMaterials != null && skinnedMeshRenderer.sharedMaterials.Length == 0)
            {
                LaunchLog(LogType.Log, "The \"" + skinnedMeshRenderer.transform.name + "\" mesh does not have any material associated with it. Associate 1 material so it can be combined.");
                valid = false;
            }
            //Make sure there are enough materials for the sub-meshes
            if (skinnedMeshRenderer.sharedMaterials != null && skinnedMeshRenderer.sharedMaterials.Length != skinnedMeshRenderer.sharedMesh.subMeshCount)
            {
                LaunchLog(LogType.Log, "The \"" + skinnedMeshRenderer.transform.name + "\" does not have enough materials for all the sub-meshes. Check this mesh, if there are missing materials or if there are more materials than necessary.");
                valid = false;
            }
            //Checks if the mesh has blendshapes
            if (skinnedMeshRenderer.sharedMesh != null && skinnedMeshRenderer.sharedMesh.blendShapeCount > 0)
            {
                LaunchLog(LogType.Log, "The \"" + skinnedMeshRenderer.transform.name + "\" mesh has Blendshapes and therefore was ignored when combining. This was done to avoid problems when handling the blendshapes, and to avoid losses.");
                valid = false;
            }
            //Checks if the material is null
            if (skinnedMeshRenderer.sharedMaterials != null)
            {
                bool haveNullMaterial = false;
                for (int i = 0; i < skinnedMeshRenderer.sharedMaterials.Length; i++)
                {
                    if (skinnedMeshRenderer.sharedMaterials[i] == null)
                    {
                        haveNullMaterial = true;
                    }
                }
                if (haveNullMaterial == true)
                {
                    LaunchLog(LogType.Log, "The \"" + skinnedMeshRenderer.transform.name + "\" has one or more null materials in the array of materials. Please associate the necessary materials in this mesh.");
                    valid = false;
                }
            }
            //Checks if any material has null texture
            if (skinnedMeshRenderer.sharedMaterials != null)
            {
                if (mergeMethod == CombineMethod.AllInOne)
                {
                    bool haveNullTexture = false;
                    for (int i = 0; i < skinnedMeshRenderer.sharedMaterials.Length; i++)
                    {
                        if (skinnedMeshRenderer.sharedMaterials[i].mainTexture == null)
                        {
                            haveNullTexture = true;
                        }
                    }
                    if (haveNullTexture == true)
                    {
                        LaunchLog(LogType.Log, "The \"" + skinnedMeshRenderer.transform.name + "\" has one or more materials with empty main textures. Please associate main textures to these materials so that your meshes can be matched correctly.");
                        valid = false;
                    }
                }
            }

            return valid;
        }

        private bool ExistsMeshesToMerge()
        {
            //Verify quantity of objetcs
            if (meshesToMerge.Length == 0)
            {
                LaunchLog(LogType.Error, "The \"" + this.transform.name + "\" merge was canceled because there are not enough meshes to merge.");
                return false;
            }
            //Verify if all is null
            int nullMeshes = 0;
            foreach (SkinnedMeshRenderer obj in meshesToMerge)
            {
                if (obj == null) { nullMeshes += 1; };
            }
            if (nullMeshes >= meshesToMerge.Length)
            {
                LaunchLog(LogType.Error, "The \"" + this.transform.name + "\" merge was canceled because there are not enough meshes to merge.");
                return false;
            }
            return true;
        }

        private bool MeshesMoreThan65kVertices(SkinnedMeshRenderer[] skinnedMeshRenderer)
        {
            //Count the vertices of meshes
            bool verticesMoreThan65k = false;

            int verticesCount = 0;
            foreach (SkinnedMeshRenderer mesh in skinnedMeshRenderer)
            {
                verticesCount += mesh.sharedMesh.vertexCount;
            }
            if (verticesCount >= 65500)
            {
                verticesMoreThan65k = true;
                LaunchLog(LogType.Error, "The GameObject \"" + this.gameObject.name + "\" merge has been canceled because the resulting mesh will have more than 65,000 vertices. The mesh resulting from the combination would have more than " + verticesCount.ToString() + "" +
                    " vertices.\n\nYou may consider enabling the \"More Than 65k Vertices\" option if you want to merge meshes without worrying about vertex limitations.");
            }

            return verticesMoreThan65k;
        }

        private Texture2D GetCopyOfTexture(Texture2D textureToCopy)
        {
            //Copy and return the copy of texture
            Texture2D copyOfTexture= new Texture2D(textureToCopy.width, textureToCopy.height, TextureFormat.RGBA32, false, false);
            for (int x = 0; x < copyOfTexture.width; x++)
            {
                for (int y = 0; y < copyOfTexture.height; y++)
                {
                    copyOfTexture.SetPixel(x, y, textureToCopy.GetPixel(x, y));
                }
            }
            return copyOfTexture;
        }

        private Texture2D GetCopyOfNormalMapOrFakeIfNotExists(Material materialToFindNormalMap, int correspondingTextureWidth, int correspondingTextureHeight)
        {
            //Enable keyword of this material
            materialToFindNormalMap.EnableKeyword(normalMapPropertyNameFind);
            Texture2D normalMap = null;

            //If found a normal map in this material
            if (materialToFindNormalMap.HasProperty(normalMapPropertyNameFind) == true && materialToFindNormalMap.GetTexture(normalMapPropertyNameFind) != null)
            {
                //Copy texture of this mesh to merge in atlas
                Texture2D originalNormalMap = materialToFindNormalMap.GetTexture(normalMapPropertyNameFind) as Texture2D;
                Texture2D copyOfNormalMap = new Texture2D(originalNormalMap.width, originalNormalMap.height, TextureFormat.RGBA32, false, false);
                for (int x = 0; x < copyOfNormalMap.width; x++)
                {
                    for (int y = 0; y < copyOfNormalMap.height; y++)
                    {
                        copyOfNormalMap.SetPixel(x, y, originalNormalMap.GetPixel(x, y));
                    }
                }
                //Resize the normal to use same size of correspondent texture
                TextureResizer.Bilinear(copyOfNormalMap, correspondingTextureWidth, correspondingTextureHeight);
                //Return the copy of this normal map
                normalMap = copyOfNormalMap;
            }
            //If NOT found a normal map in this material
            if (materialToFindNormalMap.HasProperty(normalMapPropertyNameFind) == false || materialToFindNormalMap.GetTexture(normalMapPropertyNameFind) == null)
            {
                LaunchLog(LogType.Log, "No normal map stored in the \"" + normalMapPropertyNameFind + "\" property was found in the \"" + materialToFindNormalMap.name + "\" material. This mesh will not have normal maps after the merge.");
                //Create a empty texture (same size of texture correspondent) to fill the inexistent normal map
                Texture2D fakeNormalMap = new Texture2D(correspondingTextureWidth, correspondingTextureHeight, TextureFormat.ARGB32, false, false);
                //Fill the pixels with a color
                for (int x = 0; x < fakeNormalMap.width; x++)
                {
                    for (int y = 0; y < fakeNormalMap.height; y++)
                    {
                        fakeNormalMap.SetPixel(x, y, Color.clear);
                        fakeNormalMap.SetPixel(x, y, new Color(128f/255f, 128f/255f, 255f/255f, 255f/255f));
                    }
                }
                //Return this fake normal map instance to list of normal maps to merge
                normalMap = fakeNormalMap;
            }
            return normalMap;
        }

        private Texture2D GetCopyOfSecondNormalMapOrFakeIfNotExists(Material materialToFindSecondNormalMap, int correspondingTextureWidth, int correspondingTextureHeight)
        {
            //Enable keyword of this material
            materialToFindSecondNormalMap.EnableKeyword(secondNormalMapPropertyNameFind);
            Texture2D normalMap = null;

            //If found a normal map in this material
            if (materialToFindSecondNormalMap.HasProperty(secondNormalMapPropertyNameFind) == true && materialToFindSecondNormalMap.GetTexture(secondNormalMapPropertyNameFind) != null)
            {
                //Copy texture of this mesh to merge in atlas
                Texture2D originalNormalMap = materialToFindSecondNormalMap.GetTexture(secondNormalMapPropertyNameFind) as Texture2D;
                Texture2D copyOfNormalMap = new Texture2D(originalNormalMap.width, originalNormalMap.height, TextureFormat.RGBA32, false, false);
                for (int x = 0; x < copyOfNormalMap.width; x++)
                {
                    for (int y = 0; y < copyOfNormalMap.height; y++)
                    {
                        copyOfNormalMap.SetPixel(x, y, originalNormalMap.GetPixel(x, y));
                    }
                }
                //Resize the normal to use same size of correspondent texture
                TextureResizer.Bilinear(copyOfNormalMap, correspondingTextureWidth, correspondingTextureHeight);
                //Return the copy of this normal map
                normalMap = copyOfNormalMap;
            }
            //If NOT found a normal map in this material
            if (materialToFindSecondNormalMap.HasProperty(secondNormalMapPropertyNameFind) == false || materialToFindSecondNormalMap.GetTexture(secondNormalMapPropertyNameFind) == null)
            {
                LaunchLog(LogType.Log, "No normal map stored in the \"" + secondNormalMapPropertyNameFind + "\" property was found in the \"" + materialToFindSecondNormalMap.name + "\" material. This mesh will not have normal maps after the merge.");
                //Create a empty texture (same size of texture correspondent) to fill the inexistent normal map
                Texture2D fakeNormalMap = new Texture2D(correspondingTextureWidth, correspondingTextureHeight, TextureFormat.ARGB32, false, false);
                //Fill the pixels with a color
                for (int x = 0; x < fakeNormalMap.width; x++)
                {
                    for (int y = 0; y < fakeNormalMap.height; y++)
                    {
                        fakeNormalMap.SetPixel(x, y, Color.clear);
                        fakeNormalMap.SetPixel(x, y, new Color(128f / 255f, 128f / 255f, 255f / 255f, 255f / 255f));
                    }
                }
                //Return this fake normal map instance to list of normal maps to merge
                normalMap = fakeNormalMap;
            }
            return normalMap;
        }

        private Texture2D GetCopyOfHeightMapOrFakeIfNotExists(Material materialToFindHeightMap, int correspondingTextureWidth, int correspondingTextureHeight)
        {
            //Enable keyword of this material
            materialToFindHeightMap.EnableKeyword(heightMapPropertyNameFind);
            Texture2D heightMap = null;

            //If found a height map in this material
            if (materialToFindHeightMap.HasProperty(heightMapPropertyNameFind) == true && materialToFindHeightMap.GetTexture(heightMapPropertyNameFind) != null)
            {
                //Copy texture of this mesh to merge in atlas
                Texture2D originalHeightMap = materialToFindHeightMap.GetTexture(heightMapPropertyNameFind) as Texture2D;
                Texture2D copyOfHeightMap = new Texture2D(originalHeightMap.width, originalHeightMap.height, TextureFormat.RGBA32, false, false);
                for (int x = 0; x < copyOfHeightMap.width; x++)
                {
                    for (int y = 0; y < copyOfHeightMap.height; y++)
                    {
                        copyOfHeightMap.SetPixel(x, y, originalHeightMap.GetPixel(x, y));
                    }
                }
                //Resize the height map to use same size of correspondent texture
                TextureResizer.Bilinear(copyOfHeightMap, correspondingTextureWidth, correspondingTextureHeight);
                //Return the copy of this height map
                heightMap = copyOfHeightMap;
            }
            //If NOT found a height map in this material
            if (materialToFindHeightMap.HasProperty(heightMapPropertyNameFind) == false || materialToFindHeightMap.GetTexture(heightMapPropertyNameFind) == null)
            {
                LaunchLog(LogType.Log, "No height map stored in the \"" + heightMapPropertyNameFind + "\" property was found in the \"" + materialToFindHeightMap.name + "\" material. This mesh will not have height maps after the merge.");
                //Create a empty texture (same size of texture correspondent) to fill the inexistent height map
                Texture2D fakeHeightMap = new Texture2D(correspondingTextureWidth, correspondingTextureHeight, TextureFormat.ARGB32, false, false);
                //Fill the pixels with a color
                for (int x = 0; x < fakeHeightMap.width; x++)
                {
                    for (int y = 0; y < fakeHeightMap.height; y++)
                    {
                        fakeHeightMap.SetPixel(x, y, Color.clear);
                        fakeHeightMap.SetPixel(x, y, new Color(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f));
                    }
                }
                //Return this fake height map instance to list of height maps to merge
                heightMap = fakeHeightMap;
            }
            return heightMap;
        }

        private Texture2D GetCopyOfOcclusionMapOrFakeIfNotExists(Material materialToFindOcclusionMap, int correspondingTextureWidth, int correspondingTextureHeight)
        {
            //Enable keyword of this material
            materialToFindOcclusionMap.EnableKeyword(occlusionMapPropertyNameFind);
            Texture2D occlusionMap = null;

            //If found a occlusion map in this material
            if (materialToFindOcclusionMap.HasProperty(occlusionMapPropertyNameFind) == true && materialToFindOcclusionMap.GetTexture(occlusionMapPropertyNameFind) != null)
            {
                //Copy texture of this mesh to merge in atlas
                Texture2D originalOcclusionMap = materialToFindOcclusionMap.GetTexture(occlusionMapPropertyNameFind) as Texture2D;
                Texture2D copyOfOcclusionMap = new Texture2D(originalOcclusionMap.width, originalOcclusionMap.height, TextureFormat.RGBA32, false, false);
                for (int x = 0; x < copyOfOcclusionMap.width; x++)
                {
                    for (int y = 0; y < copyOfOcclusionMap.height; y++)
                    {
                        copyOfOcclusionMap.SetPixel(x, y, originalOcclusionMap.GetPixel(x, y));
                    }
                }
                //Resize the occlusion map to use same size of correspondent texture
                TextureResizer.Bilinear(copyOfOcclusionMap, correspondingTextureWidth, correspondingTextureHeight);
                //Return the copy of this occlusion map
                occlusionMap = copyOfOcclusionMap;
            }
            //If NOT found a occlusion map in this material
            if (materialToFindOcclusionMap.HasProperty(occlusionMapPropertyNameFind) == false || materialToFindOcclusionMap.GetTexture(occlusionMapPropertyNameFind) == null)
            {
                LaunchLog(LogType.Log, "No occlusion map stored in the \"" + occlusionMapPropertyNameFind + "\" property was found in the \"" + materialToFindOcclusionMap.name + "\" material. This mesh will not have occlusion maps after the merge.");
                //Create a empty texture (same size of texture correspondent) to fill the inexistent occlusion map
                Texture2D fakeOcclusionMap = new Texture2D(correspondingTextureWidth, correspondingTextureHeight, TextureFormat.ARGB32, false, false);
                //Fill the pixels with a color
                for (int x = 0; x < fakeOcclusionMap.width; x++)
                {
                    for (int y = 0; y < fakeOcclusionMap.height; y++)
                    {
                        fakeOcclusionMap.SetPixel(x, y, Color.clear);
                        fakeOcclusionMap.SetPixel(x, y, new Color(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f));
                    }
                }
                //Return this fake occlusion map instance to list of height maps to merge
                occlusionMap = fakeOcclusionMap;
            }
            return occlusionMap;
        }

        private Texture2D GetCopyOfDetailAlbedoMapOrFakeIfNotExists(Material materialToFindDetailAlbedoMap, int correspondingTextureWidth, int correspondingTextureHeight)
        {
            //Enable keyword of this material
            materialToFindDetailAlbedoMap.EnableKeyword(detailAlbedoMapPropertyNameFind);
            Texture2D detailAlbedoMap = null;

            //If found a detail albedo map in this material
            if (materialToFindDetailAlbedoMap.HasProperty(detailAlbedoMapPropertyNameFind) == true && materialToFindDetailAlbedoMap.GetTexture(detailAlbedoMapPropertyNameFind) != null)
            {
                //Copy texture of this mesh to merge in atlas
                Texture2D originalDetailAlbedoMap = materialToFindDetailAlbedoMap.GetTexture(detailAlbedoMapPropertyNameFind) as Texture2D;
                Texture2D copyOfDetailAlbedoMap = new Texture2D(originalDetailAlbedoMap.width, originalDetailAlbedoMap.height, TextureFormat.RGBA32, false, false);
                for (int x = 0; x < copyOfDetailAlbedoMap.width; x++)
                {
                    for (int y = 0; y < copyOfDetailAlbedoMap.height; y++)
                    {
                        copyOfDetailAlbedoMap.SetPixel(x, y, originalDetailAlbedoMap.GetPixel(x, y));
                    }
                }
                //Resize the detail albedo map to use same size of correspondent texture
                TextureResizer.Bilinear(copyOfDetailAlbedoMap, correspondingTextureWidth, correspondingTextureHeight);
                //Return the copy of this detail albedo map
                detailAlbedoMap = copyOfDetailAlbedoMap;
            }
            //If NOT found a detail albedo map in this material
            if (materialToFindDetailAlbedoMap.HasProperty(detailAlbedoMapPropertyNameFind) == false || materialToFindDetailAlbedoMap.GetTexture(detailAlbedoMapPropertyNameFind) == null)
            {
                LaunchLog(LogType.Log, "No detail albedo map stored in the \"" + detailAlbedoMapPropertyNameFind + "\" property was found in the \"" + materialToFindDetailAlbedoMap.name + "\" material. This mesh will not have detail albedo maps after the merge.");
                //Create a empty texture (same size of texture correspondent) to fill the inexistent detail albedo map
                Texture2D fakeDetailAlbedoMap = new Texture2D(correspondingTextureWidth, correspondingTextureHeight, TextureFormat.ARGB32, false, false);
                //Fill the pixels with a color
                for (int x = 0; x < fakeDetailAlbedoMap.width; x++)
                {
                    for (int y = 0; y < fakeDetailAlbedoMap.height; y++)
                    {
                        fakeDetailAlbedoMap.SetPixel(x, y, Color.clear);
                        fakeDetailAlbedoMap.SetPixel(x, y, new Color(255f / 255f, 255f / 255f, 255f / 255f, 0f / 255f));
                    }
                }
                //Return this fake detail albedo map instance to list of height maps to merge
                detailAlbedoMap = fakeDetailAlbedoMap;
            }
            return detailAlbedoMap;
        }

        private Texture2D GetCopyOfSpecularMapOrFakeIfNotExists(Material materialToFindSpecularMap, int correspondingTextureWidth, int correspondingTextureHeight)
        {
            //Enable keyword of this material
            materialToFindSpecularMap.EnableKeyword(specularMapPropertyNameFind);
            Texture2D specularMap = null;

            //If found a specular map in this material
            if (materialToFindSpecularMap.HasProperty(specularMapPropertyNameFind) == true && materialToFindSpecularMap.GetTexture(specularMapPropertyNameFind) != null)
            {
                //Copy texture of this mesh to merge in atlas
                Texture2D originalSpecularMap = materialToFindSpecularMap.GetTexture(specularMapPropertyNameFind) as Texture2D;
                Texture2D copyOfSpecularMap = new Texture2D(originalSpecularMap.width, originalSpecularMap.height, TextureFormat.RGBA32, false, false);
                for (int x = 0; x < copyOfSpecularMap.width; x++)
                {
                    for (int y = 0; y < copyOfSpecularMap.height; y++)
                    {
                        copyOfSpecularMap.SetPixel(x, y, originalSpecularMap.GetPixel(x, y));
                    }
                }
                //Resize the specular map to use same size of correspondent texture
                TextureResizer.Bilinear(copyOfSpecularMap, correspondingTextureWidth, correspondingTextureHeight);
                //Return the copy of this detail albedo map
                specularMap = copyOfSpecularMap;
            }
            //If NOT found a specular map in this material
            if (materialToFindSpecularMap.HasProperty(specularMapPropertyNameFind) == false || materialToFindSpecularMap.GetTexture(specularMapPropertyNameFind) == null)
            {
                LaunchLog(LogType.Log, "No specular map stored in the \"" + specularMapPropertyNameFind + "\" property was found in the \"" + materialToFindSpecularMap.name + "\" material. This mesh will not have specular map after the merge.");
                //Create a empty texture (same size of texture correspondent) to fill the inexistent specular map
                Texture2D fakeSpecularMap = new Texture2D(correspondingTextureWidth, correspondingTextureHeight, TextureFormat.ARGB32, false, false);
                //Fill the pixels with a color
                for (int x = 0; x < fakeSpecularMap.width; x++)
                {
                    for (int y = 0; y < fakeSpecularMap.height; y++)
                    {
                        fakeSpecularMap.SetPixel(x, y, Color.clear);
                        fakeSpecularMap.SetPixel(x, y, new Color(51f / 255f, 51f / 255f, 51f / 255f, 255f / 255f));
                    }
                }
                //Return this fake specular map instance to list of specular maps to merge
                specularMap = fakeSpecularMap;
            }
            return specularMap;
        }

        private Texture2D GetCopyOfMetallicMapOrFakeIfNotExists(Material materialToFindMetallicMap, int correspondingTextureWidth, int correspondingTextureHeight)
        {
            //Enable keyword of this material
            materialToFindMetallicMap.EnableKeyword(metallicMapPropertyNameFind);
            Texture2D metallicMap = null;

            //If found a metallic map in this material
            if (materialToFindMetallicMap.HasProperty(metallicMapPropertyNameFind) == true && materialToFindMetallicMap.GetTexture(metallicMapPropertyNameFind) != null)
            {
                //Copy texture of this mesh to merge in atlas
                Texture2D originalMetallicMap = materialToFindMetallicMap.GetTexture(metallicMapPropertyNameFind) as Texture2D;
                Texture2D copyOfMetallicMap = new Texture2D(originalMetallicMap.width, originalMetallicMap.height, TextureFormat.RGBA32, false, false);
                for (int x = 0; x < copyOfMetallicMap.width; x++)
                {
                    for (int y = 0; y < copyOfMetallicMap.height; y++)
                    {
                        copyOfMetallicMap.SetPixel(x, y, originalMetallicMap.GetPixel(x, y));
                    }
                }
                //Resize the metallic map to use same size of correspondent texture
                TextureResizer.Bilinear(copyOfMetallicMap, correspondingTextureWidth, correspondingTextureHeight);
                //Return the copy of this metallic map
                metallicMap = copyOfMetallicMap;
            }
            //If NOT found a metallic map in this material
            if (materialToFindMetallicMap.HasProperty(metallicMapPropertyNameFind) == false || materialToFindMetallicMap.GetTexture(metallicMapPropertyNameFind) == null)
            {
                LaunchLog(LogType.Log, "No metallic map stored in the \"" + metallicMapPropertyNameFind + "\" property was found in the \"" + materialToFindMetallicMap.name + "\" material. This mesh will not have metallic map after the merge.");
                //Create a empty texture (same size of texture correspondent) to fill the inexistent metallic map
                Texture2D fakeMetallicMap = new Texture2D(correspondingTextureWidth, correspondingTextureHeight, TextureFormat.ARGB32, false, false);
                //Fill the pixels with a color
                for (int x = 0; x < fakeMetallicMap.width; x++)
                {
                    for (int y = 0; y < fakeMetallicMap.height; y++)
                    {
                        fakeMetallicMap.SetPixel(x, y, Color.clear);
                    }
                }
                //Return this fake specular map instance to list of specular maps to merge
                metallicMap = fakeMetallicMap;
            }
            return metallicMap;
        }

        private IEnumerator UndoMergeMeshes()
        {
            //Checks if the meshes are already combined.
            if (isMeshesCombineds == false)
            {
                LaunchLog(LogType.Error, "The " + this.transform.name + " meshes are already divided!");
            }
            if (isMeshesCombineds == true)
            {
                //Search by gameobject containing merged meshes
                Transform objMerged = GetComponent<Transform>().Find("Combined Mesh");
                //Destroy the object, and clear memory
                if (objMerged != null)
                {
                    DestroyImmediate(objMerged.gameObject, true);
                }
                //Activate the old meshes
                if(meshesToMerge != null)
                {
                    foreach (SkinnedMeshRenderer oldMesh in meshesToMerge)
                    {
                        if (oldMesh != null)
                        {
                            oldMesh.gameObject.SetActive(true);
                        }
                    }
                }
#if MTAssets_Anima2D_Available
                try
                {
                    foreach (SpriteMeshInstance smi in spriteMeshInstances)
                    {
                        if (smi != null)
                        {
                            smi.transform.gameObject.SetActive(true);
                        }
                    }
                }
                catch {}
#endif
#if UNITY_EDITOR
                if (Application.isPlaying == false)
                {
                    //Exclude the unused assets in project, if exists
                    if (pathsOfDataSavedInAssets.Count > 0)
                    {
                        foreach (string path in pathsOfDataSavedInAssets)
                        {
                            if (File.Exists(path) == true)
                            {
                                AssetDatabase.DeleteAsset(path);
                            }
                        }
                        pathsOfDataSavedInAssets.Clear();
                    }
                    statsOfMerge = "";
                }
                if(clearOnUndoMerge == true)
                {
                    logs.Clear();
                }
#endif
                //Run the GC if is activated
                if (enableMonoGC == true)
                {
                    System.GC.Collect();
                }
                if (enableUnityGC == true)
                {
                    Resources.UnloadUnusedAssets();
                }
                //Set variable as not merged
                isMeshesCombineds = false;
            }

            yield return null;
        }

#if UNITY_EDITOR
        private void Editor_CreateDirectory()
        {
            //Get scene name
            string sceneName = SceneManager.GetActiveScene().name;

            //Create the directory in project
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets"))
            {
                AssetDatabase.CreateFolder("Assets", "MT Assets");
            }
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets/Skinned Mesh Combiner"))
            {
                AssetDatabase.CreateFolder("Assets/MT Assets", "Skinned Mesh Combiner");
            }
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets/Skinned Mesh Combiner/Combined"))
            {
                AssetDatabase.CreateFolder("Assets/MT Assets/Skinned Mesh Combiner", "Combined");
            }
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName))
            {
                AssetDatabase.CreateFolder("Assets/MT Assets/Skinned Mesh Combiner/Combined", sceneName);
            }
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Material"))
            {
                AssetDatabase.CreateFolder("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName, "Material");
            }
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Texture"))
            {
                AssetDatabase.CreateFolder("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName, "Texture");
            }
            if (!AssetDatabase.IsValidFolder("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "/Mesh"))
            {
                AssetDatabase.CreateFolder("Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName, "Mesh");
            }
        }

        private void Editor_ShowDialog()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            //Show the Dialog
            EditorUtility.DisplayDialog("GameObject \"" + this.gameObject.name + "\" Combined!", "The GameObject \"" + this.gameObject.name + "\" had its Skinned Mesh Renderer combined! Data has been saved to the directory...\n\n \"Assets/MT Assets/Skinned Mesh Combiner/Combined/" + sceneName + "\"", "Cool!");
            //Select de folder
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath("Assets/MT Assets", typeof(UnityEngine.Object));
            EditorGUIUtility.PingObject(obj);
        }
#endif
    }

#region SKINNED_MESH_COMBINER_PLUGINS
    namespace PluginsOfSMC
    {

#region MESH_CLASS_EXTENSION
        namespace ExtensionForMeshClass
        {
            public static class MeshClassExtension
            {
                /*
                 * 
                 * This is an extension class, which adds extra functions to the Mesh class. For example, counting vertices for each submesh.
                 * 
                 */

                public class Vertices
                {
                    List<Vector3> verts = null;
                    List<Vector2> uv1 = null;
                    List<Vector2> uv2 = null;
                    List<Vector2> uv3 = null;
                    List<Vector2> uv4 = null;
                    List<Vector3> normals = null;
                    List<Vector4> tangents = null;
                    List<Color32> colors = null;
                    List<BoneWeight> boneWeights = null;

                    public Vertices()
                    {
                        verts = new List<Vector3>();
                    }

                    public Vertices(Mesh aMesh)
                    {
                        verts = CreateList(aMesh.vertices);
                        uv1 = CreateList(aMesh.uv);
                        uv2 = CreateList(aMesh.uv2);
                        uv3 = CreateList(aMesh.uv3);
                        uv4 = CreateList(aMesh.uv4);
                        normals = CreateList(aMesh.normals);
                        tangents = CreateList(aMesh.tangents);
                        colors = CreateList(aMesh.colors32);
                        boneWeights = CreateList(aMesh.boneWeights);
                    }

                    private List<T> CreateList<T>(T[] aSource)
                    {
                        if (aSource == null || aSource.Length == 0)
                            return null;
                        return new List<T>(aSource);
                    }

                    private void Copy<T>(ref List<T> aDest, List<T> aSource, int aIndex)
                    {
                        if (aSource == null)
                            return;
                        if (aDest == null)
                            aDest = new List<T>();
                        aDest.Add(aSource[aIndex]);
                    }

                    public int Add(Vertices aOther, int aIndex)
                    {
                        int i = verts.Count;
                        Copy(ref verts, aOther.verts, aIndex);
                        Copy(ref uv1, aOther.uv1, aIndex);
                        Copy(ref uv2, aOther.uv2, aIndex);
                        Copy(ref uv3, aOther.uv3, aIndex);
                        Copy(ref uv4, aOther.uv4, aIndex);
                        Copy(ref normals, aOther.normals, aIndex);
                        Copy(ref tangents, aOther.tangents, aIndex);
                        Copy(ref colors, aOther.colors, aIndex);
                        Copy(ref boneWeights, aOther.boneWeights, aIndex);
                        return i;
                    }

                    public void AssignTo(Mesh aTarget)
                    {
                        //Removes the limitation of 65k vertices, in case Unity supports.
                        if (verts.Count > 65535)
                            aTarget.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                        aTarget.SetVertices(verts);
                        if (uv1 != null) aTarget.SetUVs(0, uv1);
                        if (uv2 != null) aTarget.SetUVs(1, uv2);
                        if (uv3 != null) aTarget.SetUVs(2, uv3);
                        if (uv4 != null) aTarget.SetUVs(3, uv4);
                        if (normals != null) aTarget.SetNormals(normals);
                        if (tangents != null) aTarget.SetTangents(tangents);
                        if (colors != null) aTarget.SetColors(colors);
                        if (boneWeights != null) aTarget.boneWeights = boneWeights.ToArray();
                    }
                }

                //Return count of vertices for submesh
                public static Mesh GetSubmesh(this Mesh aMesh, int aSubMeshIndex)
                {
                    if (aSubMeshIndex < 0 || aSubMeshIndex >= aMesh.subMeshCount)
                        return null;
                    int[] indices = aMesh.GetTriangles(aSubMeshIndex);
                    Vertices source = new Vertices(aMesh);
                    Vertices dest = new Vertices();
                    Dictionary<int, int> map = new Dictionary<int, int>();
                    int[] newIndices = new int[indices.Length];
                    for (int i = 0; i < indices.Length; i++)
                    {
                        int o = indices[i];
                        int n;
                        if (!map.TryGetValue(o, out n))
                        {
                            n = dest.Add(source, o);
                            map.Add(o, n);
                        }
                        newIndices[i] = n;
                    }
                    Mesh m = new Mesh();
                    dest.AssignTo(m);
                    m.triangles = newIndices;
                    return m;
                }
            }
        }
#endregion

#region TEXTURE_RESIZER
        namespace TextureResizerClass
        {
            /*
             * 
             * This class is responsible for resize a texture2D.
             * Only works on ARGB32, RGB24 and Alpha8 textures that are marked readable
             * Call "TextureResizer.Bilinear()" to resize using bilinear algoritm
             * Call "TextureResizer.Point()" to resize using point algoritm
             * 
             */

            using System.Threading;
            using UnityEngine;

            public class TextureResizer
            {
                public class ThreadData
                {
                    public int start;
                    public int end;
                    public ThreadData(int s, int e)
                    {
                        start = s;
                        end = e;
                    }
                }

                private static Color[] texColors;
                private static Color[] newColors;
                private static int w;
                private static float ratioX;
                private static float ratioY;
                private static int w2;
                private static int finishCount;
                private static Mutex mutex;

                public static void Point(Texture2D tex, int newWidth, int newHeight)
                {
                    ThreadedScale(tex, newWidth, newHeight, false);
                }

                public static void Bilinear(Texture2D tex, int newWidth, int newHeight)
                {
                    ThreadedScale(tex, newWidth, newHeight, true);
                }

                private static void ThreadedScale(Texture2D tex, int newWidth, int newHeight, bool useBilinear)
                {
                    texColors = tex.GetPixels();
                    newColors = new Color[newWidth * newHeight];
                    if (useBilinear)
                    {
                        ratioX = 1.0f / ((float)newWidth / (tex.width - 1));
                        ratioY = 1.0f / ((float)newHeight / (tex.height - 1));
                    }
                    else
                    {
                        ratioX = ((float)tex.width) / newWidth;
                        ratioY = ((float)tex.height) / newHeight;
                    }
                    w = tex.width;
                    w2 = newWidth;
                    var cores = Mathf.Min(SystemInfo.processorCount, newHeight);
                    var slice = newHeight / cores;

                    finishCount = 0;
                    if (mutex == null)
                    {
                        mutex = new Mutex(false);
                    }
                    if (cores > 1)
                    {
                        int i = 0;
                        ThreadData threadData;
                        for (i = 0; i < cores - 1; i++)
                        {
                            threadData = new ThreadData(slice * i, slice * (i + 1));
                            ParameterizedThreadStart ts = useBilinear ? new ParameterizedThreadStart(BilinearScale) : new ParameterizedThreadStart(PointScale);
                            Thread thread = new Thread(ts);
                            thread.Start(threadData);
                        }
                        threadData = new ThreadData(slice * i, newHeight);
                        if (useBilinear)
                        {
                            BilinearScale(threadData);
                        }
                        else
                        {
                            PointScale(threadData);
                        }
                        while (finishCount < cores)
                        {
                            Thread.Sleep(1);
                        }
                    }
                    else
                    {
                        ThreadData threadData = new ThreadData(0, newHeight);
                        if (useBilinear)
                        {
                            BilinearScale(threadData);
                        }
                        else
                        {
                            PointScale(threadData);
                        }
                    }

                    tex.Resize(newWidth, newHeight);
                    tex.SetPixels(newColors);
                    tex.Apply();

                    texColors = null;
                    newColors = null;
                }

                public static void BilinearScale(System.Object obj)
                {
                    ThreadData threadData = (ThreadData)obj;
                    for (var y = threadData.start; y < threadData.end; y++)
                    {
                        int yFloor = (int)Mathf.Floor(y * ratioY);
                        var y1 = yFloor * w;
                        var y2 = (yFloor + 1) * w;
                        var yw = y * w2;

                        for (var x = 0; x < w2; x++)
                        {
                            int xFloor = (int)Mathf.Floor(x * ratioX);
                            var xLerp = x * ratioX - xFloor;
                            newColors[yw + x] = ColorLerpUnclamped(ColorLerpUnclamped(texColors[y1 + xFloor], texColors[y1 + xFloor + 1], xLerp), ColorLerpUnclamped(texColors[y2 + xFloor], texColors[y2 + xFloor + 1], xLerp), y * ratioY - yFloor);
                        }
                    }

                    mutex.WaitOne();
                    finishCount++;
                    mutex.ReleaseMutex();
                }

                public static void PointScale(System.Object obj)
                {
                    ThreadData threadData = (ThreadData)obj;
                    for (var y = threadData.start; y < threadData.end; y++)
                    {
                        var thisY = (int)(ratioY * y) * w;
                        var yw = y * w2;
                        for (var x = 0; x < w2; x++)
                        {
                            newColors[yw + x] = texColors[(int)(thisY + ratioX * x)];
                        }
                    }

                    mutex.WaitOne();
                    finishCount++;
                    mutex.ReleaseMutex();
                }

                private static Color ColorLerpUnclamped(Color c1, Color c2, float value)
                {
                    return new Color(c1.r + (c2.r - c1.r) * value, c1.g + (c2.g - c1.g) * value, c1.b + (c2.b - c1.b) * value, c1.a + (c2.a - c1.a) * value);
                }
            }
        }
#endregion

    }
#endregion
}