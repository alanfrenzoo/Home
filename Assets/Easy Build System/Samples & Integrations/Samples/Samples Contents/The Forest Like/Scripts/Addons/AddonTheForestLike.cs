using EasyBuildSystem.Runtimes.Internal.Addons;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EasyBuildSystem.Runtimes;

[AddOn(ADDON_NAME, ADDON_AUTHOR, ADDON_DESCRIPTION, AddOnTarget.PartBehaviour)]
public class AddonTheForestLike : AddOnBehaviour
{
    #region AddOn Fields

    public const string ADDON_NAME = "(AdsStudio12) The Forest Like";
    public const string ADDON_AUTHOR = "AdsCryptoz22";
    public const string ADDON_DESCRIPTION = "This add-on allow to create an preview, who is upgraded when an interaction is done.\n" +
        "If you use the Add-on Detach Children's make you sure that you have defined the children's manually.\n" +
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

    [Header("The Forest Like Settings")]

    [Tooltip("Define here all the transforms to active at each upgrade interaction (works by order).")]
    public Transform[] Elements;

    #endregion

    #region Private Fields

    private PartBehaviour Part;

    private GameObject Preview;

    private List<Renderer> CacheRenderers;

    #endregion

    #region Private Methods

    private void Awake()
    {
        Part = GetComponent<PartBehaviour>();

        BuildManager.Instance.DefaultState = StateType.Queue;
    }

    private void Start()
    {
        if (Part.CurrentState != StateType.Queue)
            return;

        if (Preview == null)
        {
            Preview = Instantiate(Part.gameObject, transform.position, transform.rotation);

            Preview.GetComponent<PartBehaviour>().EnableAllColliders();

            Preview.GetComponent<PartBehaviour>().ActiveAllTriggers();

            CacheRenderers = Preview.GetComponentsInChildren<Renderer>(true).ToList();

            gameObject.ChangeAllMaterialsInChildren(CacheRenderers.ToArray(), Part.PreviewMaterial);
            Preview.ChangeAllMaterialsColorInChildren(CacheRenderers.ToArray(), BuildManager.Instance.PreviewAllowedColor);

            GameObject OnlyPreview = new GameObject("Preview");

            for (int i = 0; i < CacheRenderers.Count; i++)
                CacheRenderers[i].transform.SetParent(OnlyPreview.transform);

            OnlyPreview.transform.localScale += OnlyPreview.transform.localScale * 0.001f;

            Destroy(Preview.gameObject);

            OnlyPreview.transform.SetParent(transform);

            Preview = OnlyPreview;

            GameObject OnlyInteractionPreview = Instantiate(OnlyPreview);

            OnlyInteractionPreview.name = "Interaction Preview";

            OnlyInteractionPreview.transform.SetParent(transform, false);

            for (int i = 0; i < OnlyInteractionPreview.GetComponentsInChildren<Renderer>().Length; i++)
                Destroy(OnlyInteractionPreview.GetComponentsInChildren<Renderer>()[i]);

            OnlyInteractionPreview.SetLayerRecursively(LayerMask.GetMask(Constants.LAYER_INTERACTION));

            EventHandlers.OnChangePartState += OnChangePartState;

            EventHandlers.OnDestroyedPart += OnDestroyedPart;
        }
    }

    private void Update()
    {
        if (Part.CurrentState != StateType.Queue)
            return;

        for (int i = 0; i < Elements.Length; i++)
            if (Part != null)
                Elements[i].gameObject.SetActive(i <= Part.AppearanceIndex);
    }

    private void OnChangePartState(PartBehaviour part, StateType state)
    {
        if (part == Part)
        {
            if (state == StateType.Queue)
            {
                if (Preview == null)
                    return;

                gameObject.ChangeAllMaterialsInChildren(Part.Renderers.ToArray(), Part.InitialsRenders);

                Preview.ChangeAllMaterialsColorInChildren(CacheRenderers.ToArray(), BuildManager.Instance.PreviewAllowedColor);
            }
        }
    }

    private void OnDestroyedPart(PartBehaviour part)
    {
        if (Part != part)
            return;

        Destroy(gameObject);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// This method allows to upgrade the base part.
    /// </summary>
    public void Upgrade()
    {
        Part.ChangeAppearance(Part.AppearanceIndex++);

        gameObject.ChangeAllMaterialsInChildren(Part.Renderers.ToArray(), Part.InitialsRenders);

        if (IsCompleted())
        {
            if (!Part.CheckStability())
                Part.ApplyPhysics();

            Part.DisableAllTriggers();

            Destroy(Preview);

            for (int i = 0; i < Elements.Length; i++)
                Elements[i].gameObject.SetActive(true);

            Part.ChangeState(StateType.Placed);
        }
    }

    /// <summary>
    /// This method allows to get the current upgrade progression.
    /// </summary>
    public int GetCurrentProgression()
    {
        int Count = 0;

        for (int i = 0; i < Elements.Length; i++)
            if (Elements[i].gameObject.activeSelf)
                Count++;

        return Count;
    }

    /// <summary>
    /// This method allows to check if the part progression is complete.
    /// </summary>
    public bool IsCompleted()
    {
        if (Part == null)
            return false;

        return Elements.Length - 1 <= Part.AppearanceIndex;
    }

    #endregion
}