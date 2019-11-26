using EasyBuildSystem.Runtimes;
using EasyBuildSystem.Runtimes.Internal.Addons;
using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

[AddOn(ADDON_NAME, ADDON_AUTHOR, ADDON_DESCRIPTION, AddOnTarget.PartBehaviour)]
public class AddonDetachChildren : AddOnBehaviour
{
    #region AddOn Fields

    public const string ADDON_NAME = "(AdsStudio12) Add-On Detach Childrens";
    public const string ADDON_AUTHOR = "R.Andrew";
    public const string ADDON_DESCRIPTION = "This add-on allow to detach the defined children's after the destruction is done.\n" +
        "This add-on is specific at the demo it is important of understand how it works if you want use it correctly in your own part(s).";

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

    [Header("Detach Children Settings")]

    [Tooltip("This allows to find automatically the childrens at detach before the destruction of gameObject.")]
    public bool AutomaticallyDefineChildrens;

    [Tooltip("Define here all the transforms at detach before the destruction of gameObject.")]
    public Transform[] DefinedChildrens;

    [Tooltip("This allows to define the life time of detached childrens after the destruction of gameObject.")]
    public float ChildrensLifeTime = 10f;

    [Tooltip("This allows to add a dynamic rigidobdy component before the destruction of gameObject.")]
    public bool AddDynamicRigidbody;

    [Tooltip("This allows to define the max depenetration velocity before the destruction of gameObject.")]
    public float MaxDepenetrationVelocity = 2f;

    [Tooltip("This allows to add a dynamic box collider component before the destruction of gameObject.")]
    public bool AddDynamicBoxCollider;

    #endregion

    #region Private Fields

    private PartBehaviour Part;

    private bool IsExiting;

    #endregion

    #region Private Methods

    private void OnEnable()
    {
        EventHandlers.OnDestroyedPart += OnDestroyedPart;
    }

    private void OnDisable()
    {
        EventHandlers.OnDestroyedPart -= OnDestroyedPart;
    }

    private void Awake()
    {
        Part = GetComponent<PartBehaviour>();

        if (AutomaticallyDefineChildrens)
        {
            Renderer[] Renderers = GetComponentsInChildren<Renderer>();

            DefinedChildrens = new Transform[Renderers.Length];

            for (int i = 0; i < Renderers.Length; i++)
                DefinedChildrens[i] = Renderers[i].transform;
        }
    }

    private void OnApplicationQuit()
    {
        IsExiting = true;
    }

    private void OnDestroy()
    {
        if (IsExiting)
            return;

        if (Part.CurrentState == StateType.Remove || Part.CurrentState == StateType.Placed)
            DetachChildren();
    }

    private void OnDestroyedPart(PartBehaviour part)
    {
        if (part != Part)
            return;

        if (!Part.CheckStability())
            DetachChildren();
    }

    #endregion

    #region Public Methods

    public void DetachChildren()
    {
        gameObject.ChangeAllMaterialsInChildren(Part.Renderers.ToArray(), Part.InitialsRenders);

        for (int i = 0; i < DefinedChildrens.Length; i++)
        {
            if (DefinedChildrens[i] == null)
                return;

            if (!DefinedChildrens[i].gameObject.activeSelf)
                return;

            GameObject Temp = Instantiate(DefinedChildrens[i].gameObject, DefinedChildrens[i].transform.position, DefinedChildrens[i].transform.rotation);

            if (AddDynamicRigidbody)
                Temp.AddRigibody(true, false, MaxDepenetrationVelocity);

            if (AddDynamicBoxCollider)
            {
                BoxCollider Collider = Temp.AddComponent<BoxCollider>();
                Bounds B = Temp.GetChildsBounds();

                Collider.size = B.size;
                Collider.center = B.center;
            }

            Destroy(Temp, ChildrensLifeTime);
        }
    }

    #endregion
}