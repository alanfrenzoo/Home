using UnityEditor;
using UnityEngine;

namespace EasyBuildSystem.Editor
{
    public class SceneHelper
    {
        #region Public Methods

        public static void Focus(Object target, DrawCameraMode mode = DrawCameraMode.Wireframe, bool autoSelect = true)
        {
            EditorWindow.GetWindow<SceneView>("", typeof(SceneView));

            if (autoSelect)
                Selection.activeObject = target;

            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.FrameSelected();
            }
        }

        public static void UnFocus()
        {
            if (SceneView.lastActiveSceneView != null)
            {

            }
        }

        #endregion Public Methods
    }
}