using System.Linq;
using UnityEngine;

namespace EasyBuildSystem.Runtimes.Internal.Part.Data
{
    [System.Serializable]
    public class Detection
    {
        #region Public Fields

        public Vector3 Position;

        public Vector3 Size = Vector3.one;

        public SurfaceType[] RequiredSupports;

        #endregion Public Fields

        #region Public Methods

        public bool CheckType(int type)
        {
            return RequiredSupports.Contains((SurfaceType)type);
        }

        #endregion Public Methods
    }
}