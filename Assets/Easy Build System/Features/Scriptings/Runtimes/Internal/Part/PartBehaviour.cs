using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Extensions;
using EasyBuildSystem.Runtimes.Internal.Area;
using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Group;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part.Data;
using EasyBuildSystem.Runtimes.Internal.Socket;
using EasyBuildSystem.Runtimes.Internal.Terrain;
using EasyBuildSystem.Runtimes.Internal.Terrain.Compatibilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyBuildSystem.Runtimes.Internal.Part
{
    public enum PartType
    {
        None = 0,
        Foundation = 1,
        Pillar = 2,
        Wall = 3,
        Floor = 4,
        Stair = 5,
        Door = 6,
        Window = 7,
        Desk = 8
    }

    public enum StateType
    {
        Queue,
        Preview,
        Remove,
        Edit,
        Placed
    }

    public enum SurfaceType
    {
        SurfaceAndTerrain = 0,
        Foundation = 1,
        Pillar = 2,
        Wall = 3,
        Floor = 4,
        Stair = 5,
        Door = 6,
        Window = 7,
        Desk = 8
    }

    [AddComponentMenu("Easy Build System/Features/Buildings Behaviour/Part Behaviour")]
    public class PartBehaviour : MonoBehaviour
    {
        #region Public Fields

        #region Base Settings

        public bool AdvancedFeatures;

        public int Id;

        public string Name = "New Part";

        public PartType Type = PartType.Foundation;

        public PartBehaviour[] OccupancyParts;

        public bool FreePlacement = false;

        public bool AvoidClipping = true;

        public bool AvoidClippingOnSocket = false;

        public bool AvoidAnchoredOnSocket = false;

        public bool RequireSockets;

        #endregion Base Settings

        #region Preview Settings

        public bool UseGroundUpper;

        public float GroundUpperHeight = 1f;

        public bool RotateOnSockets = false;

        public bool RotateAccordingSlope;

        public Vector3 RotationAxis = Vector3.up * 90;

        public Vector3 PreviewOffset = new Vector3(0, 0.03f, 0);

        public GameObject[] PreviewDisableObjects;

        public MonoBehaviour[] PreviewDisableBehaviours;

        public Collider[] PreviewDisableColliders;

        public bool PreviewUseColorLerpTime = false;

        public float PreviewColorLerpTime = 15.0f;

        public Material PreviewMaterial;

        #endregion Preview Settings

        #region Appearances Settings

        public bool UseAppearances;

        public List<GameObject> Appearances = new List<GameObject>();

        public int AppearanceIndex = 0;

        #endregion Appearances Settings

        #region Meshs Settings

        public Bounds MeshBounds;

        #endregion Meshs Settings

        #region Physics Settings

        public bool UseConditionalPhysics;

        public LayerMask PhysicsLayers;

        public float PhysicsLifeTime = 5f;

        public bool PhysicsConvexOnAffected = true;

        public bool PhysicsOnlyStablePlacement;

        public string[] PhysicsIgnoreTags;

        public Detection[] CustomDetections;

        #endregion Physics Settings

        #region Terrain Settings

        public bool UseTerrainPrevention;

        public Bounds TerrainBounds;

        #endregion Terrain Settings

        [HideInInspector]
        public int EntityInstanceId;

        [HideInInspector]
        public StateType CurrentState = StateType.Placed;

        [HideInInspector]
        public StateType LastState;

        [HideInInspector]
        public bool HasGroup { get { return (GetComponentInParent<GroupBehaviour>() != null); } }

        [HideInInspector]
        public bool AffectedByPhysics;

        [HideInInspector]
        public List<PartBehaviour> LinkedParts;

        [HideInInspector]
        public Dictionary<Renderer, Material[]> InitialsRenders = new Dictionary<Renderer, Material[]>();

        [HideInInspector]
        public List<Collider> Colliders = new List<Collider>();

        [HideInInspector]
        public List<Renderer> Renderers = new List<Renderer>();

        [HideInInspector]
        public SocketBehaviour[] Sockets;

        //[HideInInspector]
        public List<string> ExtraProperties = new List<string>();
 
        #endregion Public Fields

        #region Private Fields

        private bool Quitting;

        private int InitAppearanceIndex;

        #endregion Private Fields

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
            InitAppearanceIndex = AppearanceIndex;

            if (BuildManager.Instance != null)
            {
                if (!BuildManager.Instance.UseDefaultPreviewMaterial)
                {
                    PreviewMaterial = new Material(Shader.Find(Constants.TRANSPARENT_SHADER_NAME));
                    PreviewMaterial.color = new Color(0f, 0f, 0f, 0f);
                }
                else
                    PreviewMaterial = new Material(BuildManager.Instance.CustomPreviewMaterial);
            }
            else
            {
                PreviewMaterial = new Material(Shader.Find(Constants.TRANSPARENT_SHADER_NAME));
                PreviewMaterial.color = new Color(0f, 0f, 0f, 0f);
            }

            Renderers = GetComponentsInChildren<Renderer>(true).ToList();

            for (int i = 0; i < Renderers.Count; i++)
                InitialsRenders.Add(Renderers[i], Renderers[i].sharedMaterials);

            Colliders = GetComponentsInChildren<Collider>(true).ToList();

            for (int i = 0; i < Colliders.Count; i++)
                if (Colliders[i] != Colliders[i])
                    Physics.IgnoreCollision(Colliders[i], Colliders[i]);

            Sockets = GetComponentsInChildren<SocketBehaviour>();

            if (!AdvancedFeatures)
                MeshBounds = gameObject.GetChildsBounds();
        }

        private void Start()
        {
            if (CurrentState != StateType.Preview)
            {
                BuildManager.Instance.AddPart(this);

                if (UseConditionalPhysics)
                    gameObject.AddRigibody(true, true);

                ChangeAreaState(OccupancyType.Busy);

                if (!CheckStability())
                    ApplyPhysics();
            }
        }

        private void Update()
        {
            bool IsPlaced = CurrentState == StateType.Placed;

            if (!IsPlaced)
            {
                foreach (GameObject Obj in PreviewDisableObjects)
                    if (Obj)
                        Obj.SetActive(IsPlaced);

                foreach (MonoBehaviour Behaviour in PreviewDisableBehaviours)
                    if (Behaviour)
                        Behaviour.enabled = IsPlaced;

                foreach (Collider Collider in PreviewDisableColliders)
                    if (Collider)
                        Collider.enabled = IsPlaced;

                return;
            }

            if (UseAppearances)
            {
                for (int i = 0; i < Appearances.Count; i++)
                {
                    if (Appearances[i] == null)
                        return;

                    if (i == AppearanceIndex)
                        Appearances[i].SetActive(true);
                    else
                        Appearances[i].SetActive(false);
                }
            }
        }

        private void OnDestroy()
        {
            if (Quitting)
                return;

            if (CurrentState == StateType.Preview)
                return;

            EventHandlers.DestroyedPart(GetComponent<PartBehaviour>());

            BuildManager.Instance.RemovePart(this);

            if (Sockets.Length != 0)
            {
                for (int i = 0; i < Sockets.Length; i++)
                {
                    Sockets[i].ChangeAreaState(OccupancyType.Free,
                        LayerMask.NameToLayer(Constants.LAYER_DEFAULT.ToLower()),
                        LayerMask.NameToLayer(Constants.LAYER_SOCKET.ToLower()), this);
                }
            }
            else
            {
                Collider[] Colliders = Physics.OverlapBox(GetWorldPartMeshBounds().center,
                    GetWorldPartMeshBounds().extents,
                    transform.rotation, LayerMask.NameToLayer(Constants.LAYER_SOCKET.ToLower()), QueryTriggerInteraction.Collide);

                for (int i = 0; i < Colliders.Length; i++)
                {
                    SocketBehaviour Socket = Colliders[i].GetComponent<SocketBehaviour>();

                    if (Socket != null)
                        Socket.ChangeState(OccupancyType.Free, this);
                }
            }
        }

        private void OnDestroyedPart(PartBehaviour part)
        {
            if (UseConditionalPhysics)
            {
                if (!CheckStability())
                    ApplyPhysics();

                for (int i = 0; i < LinkedParts.Count; i++)
                    if (LinkedParts[i] != null)
                        if (!LinkedParts[i].CheckStability())
                            LinkedParts[i].ApplyPhysics();
            }
        }

        private void OnApplicationQuit()
        {
            Quitting = true;
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// This method allows to change the part state (Queue, Preview, Edit, Remove, Placed).
        /// Queue = Preview Allowed Color, Disable Colliders, Enable Sockets.
        /// Preview = Preview Allowed Color, Disable Colliders, Disable Sockets.
        /// Edit = Preview Allowed Color, Disable Colliders, Enable Sockets.
        /// Remove = Preview Allowed Color, Disable Colliders, Enable Sockets.
        /// Placed = Preview Allowed Color, Disable Colliders, Enable Sockets.
        /// </summary>
        public void ChangeState(StateType state)
        {
            if (BuilderBehaviour.Instance == null)
                return;

            if (CurrentState == state)
                return;

            LastState = CurrentState;

            if (state == StateType.Queue)
            {
                gameObject.ChangeAllMaterialsInChildren(Renderers.ToArray(), PreviewMaterial);

                gameObject.ChangeAllMaterialsColorInChildren(Renderers.ToArray(), Color.clear);

                foreach (GameObject Obj in PreviewDisableObjects)
                    if (Obj)
                        Obj.SetActive(false);

                foreach (MonoBehaviour Behaviour in PreviewDisableBehaviours)
                    if (Behaviour)
                        Behaviour.enabled = false;

                EnableAllColliders();

                foreach (Collider Collider in PreviewDisableColliders)
                    if (Collider)
                        Collider.enabled = false;

                foreach (SocketBehaviour Socket in Sockets)
                {
                    Socket.EnableCollider();

                    Socket.gameObject.SetActive(true);
                }
            }
            else if (state == StateType.Preview)
            {
                gameObject.ChangeAllMaterialsInChildren(Renderers.ToArray(), PreviewMaterial);

                gameObject.ChangeAllMaterialsColorInChildren(Renderers.ToArray(),
                    BuilderBehaviour.Instance.AllowPlacement ? BuildManager.Instance.PreviewAllowedColor : BuildManager.Instance.PreviewDeniedColor);

                foreach (GameObject Obj in PreviewDisableObjects)
                    if (Obj)
                        Obj.SetActive(false);

                foreach (MonoBehaviour Behaviour in PreviewDisableBehaviours)
                    if (Behaviour)
                        Behaviour.enabled = false;

                DisableAllColliders();

                foreach (Collider Collider in PreviewDisableColliders)
                    if (Collider)
                        Collider.enabled = false;

                foreach (SocketBehaviour Socket in Sockets)
                {
                    Socket.DisableCollider();

                    Socket.gameObject.SetActive(false);
                }
            }
            else if (state == StateType.Edit)
            {
                gameObject.ChangeAllMaterialsInChildren(Renderers.ToArray(), PreviewMaterial);

                gameObject.ChangeAllMaterialsColorInChildren(Renderers.ToArray(),
                    BuilderBehaviour.Instance.AllowEdition ? BuildManager.Instance.PreviewAllowedColor : BuildManager.Instance.PreviewDeniedColor);

                foreach (GameObject Obj in PreviewDisableObjects)
                    if (Obj)
                        Obj.SetActive(false);

                foreach (MonoBehaviour Behaviour in PreviewDisableBehaviours)
                    if (Behaviour)
                        Behaviour.enabled = false;

                EnableAllColliders();

                foreach (Collider Collider in PreviewDisableColliders)
                    if (Collider)
                        Collider.enabled = false;

                foreach (SocketBehaviour Socket in Sockets)
                {
                    Socket.EnableCollider();

                    Socket.gameObject.SetActive(true);
                }
            }
            else if (state == StateType.Remove)
            {
                gameObject.ChangeAllMaterialsInChildren(Renderers.ToArray(), PreviewMaterial);

                gameObject.ChangeAllMaterialsColorInChildren(Renderers.ToArray(), BuildManager.Instance.PreviewDeniedColor);

                foreach (GameObject Obj in PreviewDisableObjects)
                    if (Obj)
                        Obj.SetActive(false);

                foreach (MonoBehaviour Behaviour in PreviewDisableBehaviours)
                    if (Behaviour)
                        Behaviour.enabled = false;

                EnableAllColliders();

                foreach (Collider Collider in PreviewDisableColliders)
                    if (Collider)
                        Collider.enabled = false;

                foreach (SocketBehaviour Socket in Sockets)
                {
                    Socket.DisableCollider();

                    Socket.gameObject.SetActive(false);
                }
            }
            else if (state == StateType.Placed)
            {
                gameObject.ChangeAllMaterialsInChildren(Renderers.ToArray(), InitialsRenders);

                foreach (GameObject Obj in PreviewDisableObjects)
                    if (Obj)
                        Obj.SetActive(true);

                foreach (MonoBehaviour Behaviour in PreviewDisableBehaviours)
                    if (Behaviour)
                        Behaviour.enabled = false;

                EnableAllColliders();

                foreach (Collider Collider in PreviewDisableColliders)
                    if (Collider)
                        Collider.enabled = true;

                foreach (SocketBehaviour Socket in Sockets)
                {
                    Socket.EnableCollider();

                    Socket.gameObject.SetActive(true);
                }
            }

            CurrentState = state;

            EventHandlers.PartStateChanged(this, state);
        }

        /// <summary>
        /// This method allows to change the state all the sockets who hit the part bounds.
        /// </summary>
        public void ChangeAreaState(OccupancyType type)
        {
            SocketBehaviour[] Sockets = PhysicExtension.GetNeighborsTypesByBox<SocketBehaviour>(GetWorldPartMeshBounds().center, GetWorldPartMeshBounds().extents,
                transform.rotation, LayerMask.NameToLayer(Constants.LAYER_SOCKET.ToLower()), QueryTriggerInteraction.Collide);

            for (int i = 0; i < Sockets.Length; i++)
            {
                if (Sockets[i] != null)
                {
                    if (Sockets[i].AttachedPart != this)
                    {
                        if (Sockets[i].AllowPart(this) && type == OccupancyType.Busy)
                        {
                            Sockets[i].ChangeAreaState(type,
                                LayerMask.NameToLayer(Constants.LAYER_DEFAULT.ToLower()),
                                LayerMask.NameToLayer(Constants.LAYER_SOCKET.ToLower()), Sockets[i].AttachedPart);

                            Sockets[i].ChangeState(type, this);
                        }
                        else if (type == OccupancyType.Free)
                        {
                            Sockets[i].ChangeAreaState(type,
                                LayerMask.NameToLayer(Constants.LAYER_DEFAULT.ToLower()),
                                LayerMask.NameToLayer(Constants.LAYER_SOCKET.ToLower()), Sockets[i].AttachedPart);

                            Sockets[i].ChangeState(type, this);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method allows to active all the trigger of collider(s).
        /// </summary>
        public void ActiveAllTriggers()
        {
            foreach (Collider Coll in Colliders)
                Coll.isTrigger = true;
        }

        /// <summary>
        /// This method allows to disable all the trigger of collider(s).
        /// </summary>
        public void DisableAllTriggers()
        {
            foreach (Collider Coll in Colliders)
                Coll.isTrigger = false;
        }

        /// <summary>
        /// This method allows to enable all the collider(s).
        /// </summary>
        public void EnableAllColliders()
        {
            foreach (Collider Coll in Colliders)
                Coll.enabled = true;
        }

        /// <summary>
        /// This method allows to disable all the collider(s).
        /// </summary>
        public void DisableAllColliders()
        {
            foreach (Collider Coll in Colliders)
                Coll.enabled = false;
        }

        /// <summary>
        /// This method allows to enable all the child socket(s).
        /// </summary>
        public void EnableAllSockets()
        {
            foreach (SocketBehaviour Socket in Sockets)
                Socket.EnableCollider();
        }

        /// <summary>
        /// This method allows to disable all the child socket(s).
        /// </summary>
        public void DisableAllSockets()
        {
            foreach (SocketBehaviour Socket in Sockets)
                Socket.DisableCollider();
        }

        /// <summary>
        /// This method allows to check if the part is stable.
        /// </summary>
        public bool CheckStability()
        {
            if (BuildManager.Instance != null)
                if (!BuildManager.Instance.UsePhysics)
                    return true;

            if (!AdvancedFeatures)
                return true;

            if (!UseConditionalPhysics)
                return true;

            if (CustomDetections.Length != 0)
            {
                bool[] Results = new bool[CustomDetections.Length];

                for (int i = 0; i < CustomDetections.Length; i++)
                {
                    if (CustomDetections[i] != null)
                    {
                        PartBehaviour[] Parts = PhysicExtension.GetNeighborsTypesByBox<PartBehaviour>(transform.TransformPoint(CustomDetections[i].Position),
                            CustomDetections[i].Size, transform.rotation, PhysicsLayers);

                        for (int p = 0; p < Parts.Length; p++)
                        {
                            PartBehaviour Part = Parts[p].GetComponent<PartBehaviour>();

                            if (Part != null)
                            {
                                if (Part != this)
                                {
                                    if (!Part.AffectedByPhysics && CustomDetections[i].CheckType((int)Part.Type))
                                    {
                                        Results[i] = true;
                                    }
                                }
                            }
                        }

                        Collider[] Colliders = PhysicExtension.GetNeighborsTypesByBox<Collider>(transform.TransformPoint(CustomDetections[i].Position),
                            CustomDetections[i].Size, transform.rotation, PhysicsLayers);

                        for (int x = 0; x < Colliders.Length; x++)
                        {
                            bool UseTerrain = Application.isPlaying == true ? BuildManager.Instance.BuildingSupport == SupportType.Terrain
                                : FindObjectOfType<BuildManager>().BuildingSupport == SupportType.Terrain;

                            if (UseTerrain)
                            {
                                if (CustomDetections[i].RequiredSupports.Contains(SurfaceType.SurfaceAndTerrain))
                                {
                                    if (CustomDetections[i].RequiredSupports.ToList().Find(entry => entry == SurfaceType.SurfaceAndTerrain) == SurfaceType.SurfaceAndTerrain)
                                    {
                                        if (Colliders[x] as TerrainCollider)
                                        {
                                            Results[i] = true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (CustomDetections[i].RequiredSupports.ToList().Contains(SurfaceType.SurfaceAndTerrain))
                                {
                                    if (Colliders[x].GetComponent<SurfaceCollider>() != null)
                                    {
                                        Results[i] = true;
                                    }
                                }
                                if (ControlManager.instance.isUsingECS)
                                {
                                    if (CustomDetections[i].RequiredSupports.ToList().Contains(SurfaceType.Desk))
                                    {
                                        if (Colliders[x].tag == "Desk")
                                        {
                                            Results[i] = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return Results.All(result => result);
            }

            return false;
        }

        /// <summary>
        /// This method allows to check the condition of close area(s).
        /// </summary>
        public bool CheckAreas()
        {
            AreaBehaviour NearestArea = BuildManager.Instance.GetNearestArea(transform.position);

            if (NearestArea != null)
            {
                if (!NearestArea.AllowPlacement)
                    return true;
                else
                {
                    if (NearestArea.AllowPartPlacement.Count != 0 && !NearestArea.CheckAllowedPart(this))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// This method allows to apply the physics on this part.
        /// </summary>
        public void ApplyPhysics()
        {
            if (!BuildManager.Instance.UsePhysics)
                return;

            if (!AdvancedFeatures)
                return;

            if (!UseConditionalPhysics)
                return;

            if (AffectedByPhysics)
                return;

            for (int i = 0; i < PhysicsIgnoreTags.Length; i++)
            {
                GameObject[] IgnoreCollider = GameObject.FindGameObjectsWithTag(PhysicsIgnoreTags[i]);

                for (int x = 0; x < IgnoreCollider.Length; x++)
                    if (IgnoreCollider[x].GetComponent<Collider>() != null)
                        for (int y = 0; y < Colliders.Count; y++)
                            Physics.IgnoreCollision(Colliders[y], IgnoreCollider[x].GetComponent<Collider>());
            }

            if (GetComponent<Rigidbody>() != null)
            {
                if (PhysicsConvexOnAffected)
                {
                    for (int i = 0; i < Colliders.Count; i++)
                        if (Colliders[i].GetComponent<MeshCollider>() != null)
                            Colliders[i].GetComponent<MeshCollider>().convex = true;
                }

                GetComponent<Rigidbody>().isKinematic = false;
            }

            ChangeAreaState(OccupancyType.Free);

            AffectedByPhysics = true;

            DisableAllSockets();

            Destroy(this);

            Destroy(gameObject, PhysicsLifeTime);
        }

        /// <summary>
        /// This method allows to check if the part enters in the terrain.
        /// </summary>
        public bool CheckTerrainClipping()
        {
            if (!AdvancedFeatures)
                return false;

            if (!UseTerrainPrevention)
                return false;

            Collider[] Colliders = PhysicExtension.GetNeighborsTypesByBox<Collider>(GetWorldPartTerrainBounds().center, 
                TerrainBounds.extents, transform.rotation, Physics.AllLayers);

            for (int i = 0; i < Colliders.Length; i++)
                if (Colliders[i] as TerrainCollider || Colliders[i].GetComponentInParent<VoxelandCollider>())
                    return true;

            return false;
        }

        /// <summary>
        /// This method allows to check if the part enters an element(s).
        /// </summary>
        public bool CheckElementsClipping(LayerMask defaultLayer)
        {
            Collider[] Colliders = PhysicExtension.GetNeighborsTypesByBox<Collider>(GetWorldPartMeshBounds().center,
                GetWorldPartMeshBounds().extents, transform.rotation, defaultLayer);

            for (int i = 0; i < Colliders.Length; i++)
                if (Colliders[i] != null)
                    if (!IsSupport(Colliders[i]))
                        return true;

            return false;
        }

        /// <summary>
        /// This method allows to check if the part enters an element(s) when the preview is placed on an socket.
        /// </summary>
        public bool CheckElementsClippingOnSocket(LayerMask defaultLayer)
        {
            Collider[] Colliders = PhysicExtension.GetNeighborsTypesByBox<Collider>(GetWorldPartMeshBounds().center,
                GetWorldPartMeshBounds().extents, transform.rotation, defaultLayer);

            // This one here if we check Colliders[i].GetComponentInParent<PartBehaviour>() == null
            // we find the collider (place on an entity) with its ComponentInParent in a gameObject logic will fail
            for (int i = 0; i < Colliders.Length; i++)
                if (Colliders[i] != null)
                    if (!IsSupport(Colliders[i]) && Colliders[i].GetComponentInParent<PartBehaviour>() == null)
                        return true;

            Colliders = PhysicExtension.GetNeighborsTypesByBox<Collider>(GetWorldPartMeshBounds().center,
                GetWorldPartMeshBounds().extents / 3.5f, transform.rotation, defaultLayer);

            for (int i = 0; i < Colliders.Length; i++)
                if (Colliders[i] != null)
                    if (!IsSupport(Colliders[i]) && Colliders[i].GetComponentInParent<PartBehaviour>() != null)
                        return true;

            return false;
        }

        /// <summary>
        /// This method allows to check if the collider is a support.
        /// </summary>
        public bool IsSupport(Collider collider)
        {
            if (BuildManager.Instance.BuildingSupport == SupportType.All)
                return true;
            else if (BuildManager.Instance.BuildingSupport == SupportType.Terrain)
                return collider.GetComponent<TerrainCollider>();
            else if (BuildManager.Instance.BuildingSupport == SupportType.Voxeland)
                return collider.GetComponentInParent<VoxelandCollider>();
            else if (BuildManager.Instance.BuildingSupport == SupportType.Surface)
                return collider.GetComponent<SurfaceCollider>();
            else
                return false;
        }

        /// <summary>
        /// This method allows to link a part at this part.
        /// </summary>
        public void LinkPart(PartBehaviour part)
        {
            if (!LinkedParts.Contains(part))
            {
                if (part.UseConditionalPhysics)
                {
                    if (part.CheckStability())
                        LinkedParts.Add(part);
                }
                else
                    LinkedParts.Add(part);
            }
        }

        /// <summary>
        /// This method allows to unlink a part at this part.
        /// </summary>
        public void UnlinkPart(PartBehaviour part)
        {
            LinkedParts.Remove(part);
        }

        /// <summary>
        /// This method allows to check if the collider is attached in this part.
        /// </summary>
        public bool HasCollider(Collider collider)
        {
            for (int i = 0; i < Colliders.Count; i++)
                if (Colliders[i] == collider)
                    return true;

            return false;
        }

        /// <summary>
        /// This method allows to change the part appearance by index.
        /// </summary>
        public void ChangeAppearance(int appearanceIndex)
        {
            if (!AdvancedFeatures)
                return;

            if (!UseAppearances)
                return;

            if (AppearanceIndex == appearanceIndex)
                return;

            if (Appearances.Count < appearanceIndex)
                return;

            AppearanceIndex = appearanceIndex;

            EventHandlers.ChangedAppearance(this, appearanceIndex);
        }

        /// <summary>
        /// This method allows to get the part bounds in world space.
        /// </summary>
        public Bounds GetWorldPartMeshBounds() { return transform.BoundsToWorld(MeshBounds); }

        /// <summary>
        /// This method allows to get the terrain bounds in world space.
        /// </summary>
        public Bounds GetWorldPartTerrainBounds() { return transform.BoundsToWorld(TerrainBounds); }

        #endregion Public Methods
    }
}