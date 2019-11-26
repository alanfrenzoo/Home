using EasyBuildSystem.Runtimes.Internal.Part;
using UnityEngine;

namespace EasyBuildSystem.Runtimes.Internal.Socket.Data
{
    [System.Serializable]
    public class PartOffset
    {
        #region Public Fields

        public PartBehaviour Part;

        public Vector3 Position;

        public Vector3 Rotation;

        public bool UseCustomScale;

        public Vector3 Scale = Vector3.one;

        #endregion Public Fields

        #region Public Methods

        public PartOffset(PartBehaviour part)
        {
            Part = part;
        }

        #endregion Public Methods
    }
}