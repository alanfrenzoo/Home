using UnityEngine;

namespace EasyBuildSystem.Runtimes.Extensions
{
    public static class GameObjectExtension
    {
        #region Public Methods

        /// <summary>
        /// This allows to add a rigidbody component in runtime.
        /// </summary>
        public static void AddRigibody(this GameObject target, bool useGravity, bool isKinematic, float maxDepenetrationVelocity = 15f, HideFlags flag = HideFlags.HideAndDontSave)
        {
            if (target == null)
                return;

            if (target.GetComponent<Rigidbody>() != null)
                return;

            Rigidbody Component = target.AddComponent<Rigidbody>();
            Component.maxDepenetrationVelocity = maxDepenetrationVelocity;
            Component.useGravity = useGravity;
            Component.isKinematic = isKinematic;
            Component.hideFlags = flag;
        }

        /// <summary>
        /// This allows to add a sphere collider component in runtime.
        /// </summary>
        public static void AddSphereCollider(this GameObject target, float radius, bool isTrigger = true, HideFlags flag = HideFlags.HideAndDontSave)
        {
            if (target == null)
                return;

            if (target.GetComponent<Rigidbody>() != null)
                return;

            SphereCollider Component = target.AddComponent<SphereCollider>();
            Component.radius = radius;
            Component.isTrigger = isTrigger;
            Component.hideFlags = flag;
        }

        /// <summary>
        /// This allows to add a box collider component in runtime.
        /// </summary>
        public static void AddBoxCollider(this GameObject target, Vector3 size, Vector3 center, bool isTrigger = true, HideFlags flag = HideFlags.HideAndDontSave)
        {
            if (target == null)
                return;

            if (target.GetComponent<Rigidbody>() != null)
                return;

            BoxCollider Component = target.AddComponent<BoxCollider>();
            Component.size = size;
            Component.center = center;
            Component.isTrigger = isTrigger;
            Component.hideFlags = flag;
        }

        #endregion Public Methods
    }
}