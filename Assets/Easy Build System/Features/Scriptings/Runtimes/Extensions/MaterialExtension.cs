using System.Collections.Generic;
using UnityEngine;

namespace EasyBuildSystem.Runtimes.Extensions
{
    public static class MaterialExtension
    {
        #region Public Methods

        /// <summary>
        /// This allows to change all the materials color of childrens.
        /// </summary>
        public static void ChangeAllMaterialsColorInChildren(this GameObject go, Renderer[] renderers, Color color, float lerpTime = 15.0f, bool lerp = false)
        {
            Renderer[] Renderers = go.GetComponentsInChildren<Renderer>();

            for (int i = 0; i < Renderers.Length; i++)
            {
                if (Renderers[i] != null)
                {
                    for (int x = 0; x < Renderers[i].materials.Length; x++)
                    {
                        if (lerp)
                            Renderers[i].materials[x].color = Color.Lerp(Renderers[i].materials[x].color, color, lerpTime * Time.deltaTime);
                        else
                            Renderers[i].materials[x].color = color;
                    }
                }
            }
        }

        /// <summary>
        /// This allows to change all the materials of childrens.
        /// </summary>
        public static void ChangeAllMaterialsInChildren(this GameObject go, Renderer[] renderers, Material material)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    Material[] materials = new Material[renderers[i].sharedMaterials.Length];

                    for (int x = 0; x < renderers[i].sharedMaterials.Length; x++)
                        materials[x] = material;

                    renderers[i].sharedMaterials = materials;
                }
            }
        }

        /// <summary>
        /// This allows to change all the materials of childrens (used for the restoration of initial materials).
        /// </summary>
        public static void ChangeAllMaterialsInChildren(this GameObject go, Renderer[] renderers, Dictionary<Renderer, Material[]> materials)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] CacheMaterials = renderers[i].sharedMaterials;

                for (int c = 0; c < CacheMaterials.Length; c++)
                    CacheMaterials[c] = materials[renderers[i]][c];

                renderers[i].materials = CacheMaterials;
            }
        }

        #endregion Public Methods
    }
}