using UnityEditor;
using UnityEngine;

namespace EasyBuildSystem.Editor
{
    [InitializeOnLoad]
    public class Initializer
    {
        #region Public Methods

        static Initializer()
        {
            if (!EditorPrefs.GetBool("Ebs_IsInit"))
            {
                Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : Loading <b>Easy Build System</b> on this project ...");

                MainEditor.CheckLayers();

                Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : The system has been correctly initialized !");

                EditorPrefs.SetBool("Ebs_IsInit", true);
            }
        }

        #endregion Public Methods
    }
}