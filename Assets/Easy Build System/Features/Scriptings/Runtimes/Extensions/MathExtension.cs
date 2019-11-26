using UnityEngine;

namespace EasyBuildSystem.Runtimes.Extensions
{
    public static class MathExtension
    {
        #region Public Methods

        /// <summary>
        /// This method allows to encapsuled all the childs and return the result bounds.
        /// </summary>
        public static Bounds GetChildsBounds(this GameObject target)
        {
            MeshRenderer[] Renders = target.GetComponentsInChildren<MeshRenderer>();

            Quaternion CurrentRotation = target.transform.rotation;

            Vector3 CurrentScale = target.transform.localScale;

            target.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            target.transform.localScale = Vector3.one;

            Bounds ResultBounds = new Bounds(target.transform.position, Vector3.zero);

            foreach (Renderer Render in Renders)
                ResultBounds.Encapsulate(Render.bounds);

            Vector3 RelativeCenter = ResultBounds.center - target.transform.position;

            ResultBounds.center = PositionToGridPosition(0.1f, 0f, RelativeCenter);

            ResultBounds.size = PositionToGridPosition(0.1f, 0f, ResultBounds.size);

            target.transform.rotation = CurrentRotation;

            target.transform.localScale = CurrentScale;

            return ResultBounds;
        }

        /// <summary>
        /// This method allows to encapsuled the parent and return the result bounds.
        /// </summary>
        public static Bounds GetParentBounds(this GameObject target)
        {
            MeshRenderer[] Renders = target.GetComponents<MeshRenderer>();

            Quaternion CurrentRotation = target.transform.rotation;

            Vector3 CurrentScale = target.transform.localScale;

            target.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            target.transform.localScale = Vector3.one;

            Bounds ResultBounds = new Bounds(target.transform.position, Vector3.zero);

            foreach (Renderer Render in Renders)
                ResultBounds.Encapsulate(Render.bounds);

            Vector3 RelativeCenter = ResultBounds.center - target.transform.position;

            ResultBounds.center = PositionToGridPosition(0.1f, 0f, RelativeCenter);

            ResultBounds.size = PositionToGridPosition(0.1f, 0f, ResultBounds.size);

            target.transform.rotation = CurrentRotation;

            target.transform.localScale = CurrentScale;

            return ResultBounds;
        }

        /// <summary>
        /// This allows to change the local space of bounds in world space.
        /// </summary>
        public static Bounds BoundsToWorld(this Transform transform, Bounds localBounds)
        {
            if (transform != null)
                return new Bounds(transform.TransformPoint(localBounds.center), localBounds.size);
            else
                return new Bounds(localBounds.center, localBounds.size);
        }

        /// <summary>
        /// This allows to get a axis in a grid.
        /// </summary>
        public static float ConvertToGrid(float gridSize, float gridOffset, float axis)
        {
            return Mathf.Round(axis) * gridSize + gridOffset;
        }

        /// <summary>
        /// This allows to get a vector in a grid.
        /// </summary>
        public static Vector3 PositionToGridPosition(float gridSize, float gridOffset, Vector3 position)
        {
            position -= Vector3.one * gridOffset;
            position /= gridSize;
            position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z));
            position *= gridSize;
            position += Vector3.one * gridOffset;
            return position;
        }

        #endregion Public Methods
    }
}