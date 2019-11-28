using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if MTAssets_Anima2D_Available
    using Anima2D;
#endif
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace MTAssets
{
    /*
     * Class responsible for rendering the texture under the combined mesh (Anima2D) of sprite meshes.
     */

    [ExecuteInEditMode]
    public class RendererOfCombinedAnima2D : MonoBehaviour
    {
#if UNITY_EDITOR
        //The UI of this component
        [UnityEditor.CustomEditor(typeof(RendererOfCombinedAnima2D)), CanEditMultipleObjects]
        public class ConfiguracaoUI : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                RendererOfCombinedAnima2D script = (RendererOfCombinedAnima2D)target;

                GUILayout.Space(10);
                EditorGUILayout.HelpBox("This component is part of the Anima2D merge made with the Skinned Mesh Combiner. It is responsible for rendering the texture over the combined meshes. Please do not modify this component. If you want to undo the merge, access the Skinned Mesh Combiner component in the GameObject root.", MessageType.None);
                if (GUILayout.Button("Select Root GameObject", GUILayout.Height(25)))
                {
#if MTAssets_Anima2D_Available
                    Selection.objects = new Object[] { script.rootGameObject };
#endif
                }
                GUILayout.Space(10);

                DrawDefaultInspector();
            }
        }
#endif

#if MTAssets_Anima2D_Available
        public Texture2D atlasForRenderInChar;
        [HideInInspector]
        public SkinnedMeshRenderer cachedSkinnedRenderer;
        [HideInInspector]
        public MaterialPropertyBlock materialPropertyBlock;
        [HideInInspector]
        public GameObject rootGameObject;

        void OnWillRenderObject()
        {
            if (materialPropertyBlock != null)
            {
                if(atlasForRenderInChar != null)
                {
                    materialPropertyBlock.SetTexture("_MainTex", atlasForRenderInChar);
                }
                cachedSkinnedRenderer.SetPropertyBlock(materialPropertyBlock);
            }
            if (materialPropertyBlock == null)
            {
                materialPropertyBlock = new MaterialPropertyBlock();
            }
        }
#endif
    }
}
