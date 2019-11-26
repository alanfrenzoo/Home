using UnityEngine;
using UnityEngine.EventSystems;

namespace EasyBuildSystem.Runtimes.Extensions
{
    public static class UIExtension
    {
        #region Public Methods

        /// <summary>
        /// This method allows to check if the cursor is over on UI/GUI.
        /// </summary>
        public static bool IsCursorOverUserInterface()
        {
#if UNITY_ANDROID
            return false;
#else
            if (EventSystem.current != null)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return true;

                for (int i = 0; i < Input.touchCount; ++i)
                    if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                        return true;
            }

            return GUIUtility.hotControl != 0;
#endif
        }

        #endregion Public Methods
    }
}