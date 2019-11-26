using UnityEngine;

namespace EasyBuildSystem.Runtimes.Internal.Addons
{
    public class AddOnBehaviour : MonoBehaviour
    {
        #region Public Fields

        public virtual string Name { get; protected set; }

        public virtual string Author { get; protected set; }

        public virtual string Description { get; protected set; }

        #endregion Public Fields
    }
}