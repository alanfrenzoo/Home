using System.Collections.Generic;
using UnityEngine;

namespace EasyBuildSystem.Runtimes.Extensions
{
    public static class PhysicExtension
    {
        #region Public Methods

        /// <summary>
        /// This method allows of change recursively all layers of each transform child.
        /// </summary>
        public static void SetLayerRecursively(this GameObject go, LayerMask layer)
        {
            if (go == null)
            {
                return;
            }

            go.layer = ToLayer(layer.value);

            foreach (Transform child in go.transform)
            {
                if (child == null)
                {
                    continue;
                }

                SetLayerRecursively(child.gameObject, layer);
            }
        }

        /// <summary>
        /// This method allows to get index from a bitmask (LayerMask).
        /// </summary>
        public static int ToLayer(int bitmask)
        {
            int Result = bitmask > 0 ? 0 : 31;

            while (bitmask > 1)
            {
                bitmask = bitmask >> 1;

                Result++;
            }

            return Result;
        }

        /// <summary>
        /// This method allows to get all the types at proximity by OverlapSphere.
        /// </summary>
        public static T[] GetNeighborsTypesBySphere<T>(Vector3 position, float size, LayerMask layer, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            Collider[] Colliders = Physics.OverlapSphere(position, size, layer, query);

            List<T> Types = new List<T>();

            for (int i = 0; i < Colliders.Length; i++)
            {
                T Type = Colliders[i].GetComponentInParent<T>();

                if (Type != null)
                    if (Type is T)
                        if (!Types.Contains(Type))
                            Types.Add(Type);
            }

            return Types.ToArray();
        }

        /// <summary>
        /// This method allows to get all the types by GetComponentInParent at proximity by OverlapBox.
        /// </summary>
        public static T[] GetNeighborsTypesByBox<T>(Vector3 position, Vector3 size, Quaternion rotation, LayerMask layer, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            bool InitQueries = Physics.queriesHitTriggers;

            Physics.queriesHitTriggers = true;

            Collider[] Colliders = Physics.OverlapBox(position, size, rotation, layer, query);

            Physics.queriesHitTriggers = InitQueries;

            List<T> Types = new List<T>();

            for (int i = 0; i < Colliders.Length; i++)
            {
                T Type = Colliders[i].GetComponentInParent<T>();

                if (Type != null)
                {
                    if (Type is T)
                        if (!Types.Contains(Type))
                            Types.Add(Type);
                }
            }

            return Types.ToArray();
        }

        #endregion Public Methods
    }
}