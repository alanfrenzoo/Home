using EasyBuildSystem.Runtimes.Internal.Part;
using System.Collections.Generic;
using UnityEngine;

namespace EasyBuildSystem.Runtimes.Internal.Managers.Data
{
    public class PartsCollection : ScriptableObject
    {
        #region Public Fields

        public List<PartBehaviour> Parts = new List<PartBehaviour>();

        #endregion Public Fields

        #region Public Methods

        /// <summary>
        /// This method allows to check if the collection does not contains the same part Id.
        /// </summary>
        public bool CheckPartsCollection()
        {
            List<PartBehaviour> CacheParts = new List<PartBehaviour>();

            foreach (PartBehaviour Part in Parts)
                if (Part != null)
                    CacheParts.Add(Part);

            foreach (PartBehaviour CachePart in CacheParts)
            {
                if (CachePart != null)
                {
                    foreach (PartBehaviour Part in Parts)
                    {
                        if (Part != null)
                        {
                            if (CachePart.Name != Part.Name)
                            {
                                if (CachePart.Id == Part.Id)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        #endregion Public Methods
    }
}