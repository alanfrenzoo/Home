using EasyBuildSystem.Runtimes.Internal.Addons;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EasyBuildSystem.Runtimes;
using EasyBuildSystem.Runtimes.Internal.Socket;
using System;
using UnityEngine.AI;
using EasyBuildSystem.Runtimes.Internal.Storage;

[AddOn(ADDON_NAME, ADDON_AUTHOR, ADDON_DESCRIPTION, AddOnTarget.BuildManager)]
public class AddonNavMesh : AddOnBehaviour
{
    #region AddOn Fields

    public const string ADDON_NAME = "(AdsStudio12) Nav Mesh";
    public const string ADDON_AUTHOR = "AdsCryptoz22";
    public const string ADDON_DESCRIPTION = "This add-on allow to update the NavMesh Surface component to each action of system.\n" +
        "It is important of select the Use Geometry in (Physics Colliders) to use correctly this add-on.\n" +
        "Require : This add-on require the component NavMesh Surface in your scene.";

    [HideInInspector]
    public string _Name = ADDON_NAME;

    public override string Name
    {
        get
        {
            return _Name;
        }

        protected set
        {
            _Name = value;
        }
    }

    [HideInInspector]
    public string _Author = ADDON_AUTHOR;

    public override string Author
    {
        get
        {
            return _Author;
        }

        protected set
        {
            _Author = value;
        }
    }

    [HideInInspector]
    public string _Description = ADDON_DESCRIPTION;
     
    public override string Description
    {
        get
        {
            return _Description;
        }

        protected set
        {
            _Description = value;
        }
    }

    #endregion AddOn Fields

    #region Public Fields

    public static AddonNavMesh Instance;

    #endregion

    #region Private Fields

    public NavMeshSurface Surface;

    #endregion

    #region Private Methods

    private void OnEnable()
    {
        if (FindObjectOfType<BuildStorage>() != null && FindObjectOfType<BuildStorage>().ExistsStorageFile())
            EventHandlers.OnStorageLoadingDone += OnStorageLoadingDone;
        else
        {
            EventHandlers.OnPlacedPart += OnPlacedPart;
            EventHandlers.OnDestroyedPart += OnDestroyedPart;
        }
    }

    private void OnDisable()
    {
        EventHandlers.OnPlacedPart -= OnPlacedPart;
        EventHandlers.OnDestroyedPart -= OnDestroyedPart;
    }

    private void Awake()
    {
        UpdateMeshData();

        Instance = this;
    }

    private void OnStorageLoadingDone(PartBehaviour[] Parts)
    {
        UpdateMeshData();

        EventHandlers.OnPlacedPart += OnPlacedPart;
        EventHandlers.OnDestroyedPart += OnDestroyedPart;
    }

    private void OnPlacedPart(PartBehaviour part, SocketBehaviour socket)
    {
        UpdateMeshData();
    }

    private void OnDestroyedPart(PartBehaviour part)
    {
        UpdateMeshData();
    }

    #endregion

    #region Public Methods

    public void UpdateMeshData(NavMeshData data = null)
    {
        if (data == null)
            Surface.UpdateNavMesh(Surface.navMeshData);
        else
            Surface.UpdateNavMesh(data);
    }

    #endregion
}