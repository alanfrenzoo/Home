using EasyBuildSystem.Editor;
using EasyBuildSystem.Runtimes.Internal.Builder;
using System;
using UnityEditor;
using UnityEngine;

public class QuickStart : MonoBehaviour
{
    #region Public Fields

    public const string MANAGER_PREFAB_NAME = "Easy Build System - Build Manager";

    public static bool IsForIntegration;

    #endregion Public Fields

    #region Public Methods

    [MenuItem("Tools/Easy Build System/Utilities.../Quick Start...", priority = 99)]
    public static void Init()
    {
        try
        {
            if (!IsForIntegration)
            {
                if (!EditorUtility.DisplayDialog("Easy Build System - Quick Start",
                    "The Quick Start function allows you to use the default placement system with the default settings.\n\n" +
                    "You will find more information on this function in the documentation included in the system.\n\n" +
                    "If you Check that your scene contains a Main Camera.\n\n" +
                    "Do you want run the Quick Start?", "Yes", "No"))
                    return;
            }

            if (Camera.main != null)
            {
                if (FindObjectOfType<DesktopBuilderBehaviour>() == null)
                    Camera.main.gameObject.AddComponent<DesktopBuilderBehaviour>();
                else
                {
                    Debug.LogError("<b><color=red>[Easy Build System]</color></b> : The component <b>Desktop Builder Behaviour</b> already exists.");

                    return;
                }
            }

            MainEditor.CheckLayers();

            GameObject ResourceManager = (GameObject)Resources.Load("Prefabs/" + MANAGER_PREFAB_NAME);

            if (ResourceManager == null)
            {
                Debug.LogError("<b><color=red>[Easy Build System]</color></b> : The gameObject with the name (Easy Build System - Build Manager) does not exists !");

                return;
            }

            GameObject Manager = Instantiate(ResourceManager);

            Manager.name = MANAGER_PREFAB_NAME;

            if (!IsForIntegration)
                Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : You can now use the system with the default settings !");

            IsForIntegration = false;
        }
        catch (Exception ex)
        {
            Debug.LogError("<b><color=red>[Easy Build System]</color></b> : " + ex.Message);
        }
    }

    #endregion Public Methods
}