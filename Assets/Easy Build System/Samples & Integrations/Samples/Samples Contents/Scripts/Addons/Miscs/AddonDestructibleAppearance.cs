using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Extensions;
using EasyBuildSystem.Runtimes.Internal.Addons;
using EasyBuildSystem.Runtimes.Internal.Part;
using UnityEngine;

[System.Serializable]
public class DestructibleAppearance
{
    public string Name;
    public int AppearanceIndex = 0;
    public GameObject FracturedAppearance;
    public float FracturedLifeTime;
}

[AddOn(ADDON_NAME, ADDON_AUTHOR, ADDON_DESCRIPTION, AddOnTarget.PartBehaviour)]
public class AddonDestructibleAppearance : AddOnBehaviour
{
    #region AddOn Fields

    public const string ADDON_NAME = "(AdsStudio12) Destructible Appearance(s)";
    public const string ADDON_AUTHOR = "AdsCryptoz22";
    public const string ADDON_DESCRIPTION = "This add-on allow to instantiate after the destruction, some gameObject(s) according to the appearance of the part.\n" +
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

    [Header("Destructible Appearance(s) Settings")]

    [Tooltip("Define here the gameObject(s) at instantiate when the destruction of gameObject.")]
    public DestructibleAppearance[] Destructibles;

    [Tooltip("This allows to define the max depenetration velocity before the destruction of gameObject.")]
    public float MaxDepenetrationVelocity = .5f;

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
            InstantiateObjects();
    }

    private void OnDestroyedPart(PartBehaviour part)
    {
        if (part != Part)
            return;

        if (!Part.CheckStability())
            InstantiateObjects();
    }

    private void InstantiateObjects()
    {
        for (int i = 0; i < Destructibles.Length; i++)
        {
            if (Part.AppearanceIndex == Destructibles[i].AppearanceIndex)
            {
                if (Destructibles[i] == null)
                    return;

                Destructibles[i].FracturedAppearance.SetActive(true);

                GameObject Temp = Instantiate(Destructibles[i].FracturedAppearance.gameObject,
                    Destructibles[i].FracturedAppearance.transform.position, 
                    Destructibles[i].FracturedAppearance.transform.rotation);

                for (int x = 0; x < Temp.transform.childCount; x++)
                    Temp.transform.GetChild(x).GetComponent<Rigidbody>().maxDepenetrationVelocity = MaxDepenetrationVelocity;

                Destroy(Temp, Destructibles[i].FracturedLifeTime);

                Destroy(gameObject);

                break;
            }
        }
    }

    #endregion
}