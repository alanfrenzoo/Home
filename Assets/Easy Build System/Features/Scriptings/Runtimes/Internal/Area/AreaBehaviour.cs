using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using System.Collections.Generic;
using UnityEngine;

namespace EasyBuildSystem.Runtimes.Internal.Area
{
    [AddComponentMenu("Easy Build System/Features/Buildings Behaviour/Area Behaviour")]
    public class AreaBehaviour : MonoBehaviour
    {
        #region Public Fields

        public float Radius = 5f;

        public bool AllowPlacement;

        public List<PartBehaviour> AllowPartPlacement = new List<PartBehaviour>();

        public bool AllowDestruction;

        #endregion Public Fields

        #region Private Methods

        private void Start()
        {
            BuildManager.Instance.AddArea(this);
        }

        private void OnDestroy()
        {
            BuildManager.Instance.RemoveArea(this);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = AllowPlacement ? Color.green : Color.red;

            Gizmos.DrawWireSphere(transform.position, Radius);
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// This method allows to check if the part (by id) exists in the AllowPartPlacement list.
        /// </summary>
        public bool CheckAllowedPart(PartBehaviour part)
        {
            return AllowPartPlacement.Find(entry => entry.Id == part.Id);
        }

        #endregion Public Methods
    }
}