using EasyBuildSystem.Runtimes.Internal.Storage.Data;
using UnityEngine;

namespace EasyBuildSystem.Runtimes.Internal.Blueprint.Data
{
    public class BlueprintData : ScriptableObject
    {
        #region Public Fields

        public PartModel Model = new PartModel();

        public string Data;

        #endregion Public Fields
    }
}