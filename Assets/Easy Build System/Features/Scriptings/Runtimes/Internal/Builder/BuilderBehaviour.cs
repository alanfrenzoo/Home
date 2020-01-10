using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Extensions;
using EasyBuildSystem.Runtimes.Internal.Area;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Managers.Data;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Internal.Socket;
using EasyBuildSystem.Runtimes.Internal.Socket.Data;
using EasyBuildSystem.Runtimes.Internal.Terrain;
using EasyBuildSystem.Runtimes.Internal.Terrain.Compatibilities;
using UnityEngine;

namespace EasyBuildSystem.Runtimes.Internal.Builder
{
    public enum BuildMode
    {
        None,
        Placement,
        Destruction,
        Edition
    }

    public enum DetectionType
    {
        Line,
        Overlap
    }

    public enum MovementType
    {
        Normal,
        Smooth,
        Grid,
        GridwFreeHeight
    }

    public enum RayType
    {
        FirstPerson,
        TopDown,
        ThirdPerson
    }

    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    [AddComponentMenu("Easy Build System/Features/Builders Behaviour/Default Builder Behaviour")]
    public class BuilderBehaviour : MonoBehaviour
    {
        #region Public Fields

        public static BuilderBehaviour Instance;

        #region Base Settings

        public float ActionDistance = 6f;

        public float SnapThreshold = 5f;

        public float OutOfRangeDistance = 0f;

        public float OverlapAngles = 35f;

        public bool LockRotation;

        public LayerMask FreeLayers;

        public LayerMask SocketLayers;

        public DetectionType RayDetection = DetectionType.Overlap;

        public RayType CameraType;

        public float RaycastOffset = 1f;

        public Transform RaycastOriginTransform;

        public Transform RaycastAnchorTransform;

        #endregion Base Settings

        #region Modes Settings

        public bool UsePlacementMode = true;
        public bool ResetModeAfterPlacement = false;
        public bool UseDestructionMode = true;
        public bool ResetModeAfterDestruction = false;
        public bool UseEditionMode = true;

        #endregion Modes Settings

        #region Preview Settings

        public bool UsePreviewCamera;
        public Camera PreviewCamera;
        public LayerMask PreviewLayer;

        public MovementType PreviewMovementType;

        public float PreviewGridSize = 1.0f;
        public float PreviewGridOffset;
        public float PreviewSmoothTime = 5.0f;

        #endregion Preview Settings

        #region Audio Settings

        public AudioSource Source;

        public AudioClip[] PlacementClips;

        public AudioClip[] DestructionClips;

        #endregion Audio Settings

        [HideInInspector]
        public BuildMode CurrentMode;

        [HideInInspector]
        public PartBehaviour SelectedPrefab;

        [HideInInspector]
        public int SelectedIndex;

        [HideInInspector]
        public PartBehaviour CurrentPreview;

        [HideInInspector]
        public PartBehaviour CurrentEditionPreview;

        [HideInInspector]
        public PartBehaviour CurrentRemovePreview;

        [HideInInspector]
        public Vector3 CurrentRotationOffset;

        [HideInInspector]
        public bool AllowPlacement = true;

        [HideInInspector]
        public bool AllowDestruction = true;

        [HideInInspector]
        public bool AllowEdition = true;

        [HideInInspector]
        public bool HasSocket;

        [HideInInspector]
        public SocketBehaviour CurrentSocket;

        [HideInInspector]
        public SocketBehaviour LastSocket;

        public Transform GetTransform
        {
            get
            {
                return CameraType != RayType.TopDown ? transform.parent != null ? transform.parent
                    : transform : RaycastAnchorTransform != null ? RaycastAnchorTransform : transform;
            }
        }

        #endregion Public Fields

        #region Private Fields

        private Camera BuilderCamera;

        private RaycastHit TopDownHit;

        #endregion Private Fields

        #region Private Methods

        private void OnEnable()
        {
            if (!Application.isPlaying)
            {
                if (FreeLayers == 0)
                    FreeLayers = LayerMask.GetMask(Constants.LAYER_DEFAULT);

                if (SocketLayers == 0)
                    SocketLayers = LayerMask.GetMask(Constants.LAYER_SOCKET);
            }
        }
        public virtual void Awake()
        {
            Instance = this;

            if (PreviewCamera != null)
                PreviewCamera.enabled = UsePreviewCamera;

            BuilderCamera = GetComponent<Camera>();

            if (BuilderCamera == null)
                Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : No camera for the Builder Behaviour component.");
        }

        public virtual void Start()
        {
            if (!Application.isPlaying)
                return;

            //Instance = this;

            //if (PreviewCamera != null)
            //    PreviewCamera.enabled = UsePreviewCamera;

            //BuilderCamera = GetComponent<Camera>();

            //if (BuilderCamera == null)
            //    Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : No camera for the Builder Behaviour component.");
        }

        public virtual void Update()
        {
            if (!Application.isPlaying)
                return;

            UpdateModes();
        }

        private Ray GetRay()
        {
            if (CameraType == RayType.TopDown)
                return BuilderCamera.ScreenPointToRay(Input.mousePosition + new Vector3(0, 0, RaycastOffset));
            else if (CameraType == RayType.FirstPerson)
                return new Ray(BuilderCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, RaycastOffset)), BuilderCamera.transform.forward);
            else if (CameraType == RayType.ThirdPerson)
                if (RaycastOriginTransform != null)
                    return new Ray(RaycastOriginTransform.position, BuilderCamera.transform.forward);

            return new Ray();
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// This method allows to update all the builder (Placement, Destruction, Edition).
        /// </summary>
        public virtual void UpdateModes()
        {
            if (BuildManager.Instance == null)
                return;

            if (BuildManager.Instance.PartsCollection == null)
                return;

            if (CurrentMode == BuildMode.Placement)
            {
                if (SelectedPrefab == null)
                    return;

                if (!PreviewExists())
                {
                    CreatePreview(SelectedPrefab.gameObject);
                    return;
                }
                else
                {
                    UpdatePreview();
                }
            }
            else if (CurrentMode == BuildMode.Destruction)
                UpdateRemovePreview();
            else if (CurrentMode == BuildMode.Edition)
                UpdateEditionPreview();
            else if (CurrentMode == BuildMode.None)
                ClearPreview();
        }

        #region Placement

        /// <summary>
        /// This method allows to update the placement preview.
        /// </summary>
        public void UpdatePreview()
        {
            HasSocket = false;

            if (CameraType == RayType.TopDown)
                Physics.Raycast(GetRay(), out TopDownHit, Mathf.Infinity, LayerMask.NameToLayer(Constants.LAYER_SOCKET.ToLower()));

            if (RayDetection == DetectionType.Overlap)
            {
                SocketBehaviour[] Sockets =
                    PhysicExtension.GetNeighborsTypesBySphere<SocketBehaviour>(GetTransform.position, ActionDistance,
                    LayerMask.NameToLayer(Constants.LAYER_SOCKET.ToLower()));

                if (Sockets.Length > 0)
                    UpdateSnapsMovement(Sockets);
                else
                    UpdateFreeMovement();
            }
            else
            {
                SocketBehaviour Socket = null;

                RaycastHit Hit;

                if (Physics.Raycast(GetRay(), out Hit, ActionDistance, LayerMask.NameToLayer(Constants.LAYER_DEFAULT.ToLower())))
                    if (Hit.collider.GetComponentInChildren<SocketBehaviour>() != null)
                        Socket = Hit.collider.GetComponentInChildren<SocketBehaviour>();

                if (Socket != null)
                    UpdateSnapsMovement(new SocketBehaviour[1] { Socket });
                else
                    UpdateFreeMovement();
            }

            if (CurrentMode == BuildMode.Placement || CurrentMode == BuildMode.Edition)
            {
                foreach (SocketBehaviour Socket in BuildManager.Instance.Sockets)
                    if (Socket != null)
                        Socket.EnableColliderByPartType(SelectedPrefab.Type);
            }

            UpdatePreviewCollisions();

            CurrentPreview.gameObject.ChangeAllMaterialsColorInChildren(CurrentPreview.Renderers.ToArray(),
                AllowPlacement ? (ItemManager.instance.CurrentGameMode == ItemManager.GameModeCode.DecorateFloor ? new Color(1f, 1f, 1f) : BuildManager.Instance.PreviewAllowedColor) : BuildManager.Instance.PreviewDeniedColor, SelectedPrefab.PreviewColorLerpTime, SelectedPrefab.PreviewUseColorLerpTime);

        }

        /// <summary>
        /// This method allows to move the preview in free movement.
        /// </summary>
        public void UpdateFreeMovement()
        {
            if (CurrentPreview == null)
                return;

            if (!ScreenTouchManager.instance.CheckOnEditingItemPress() && ItemManager.instance.CurrentGameMode == ItemManager.GameModeCode.DecorateFurniture)
                return;

            if (!ControlManager.instance.IsJustInstantiated)
                if (!ScreenTouchManager.instance.CheckOnButtonUpWithoutMove() && ItemManager.instance.CurrentGameMode == ItemManager.GameModeCode.DecorateFloor)
                    return;

            if (ScreenTouchManager.instance.CheckInputOnStationary() && ItemManager.instance.CurrentGameMode == ItemManager.GameModeCode.DecorateFurniture)
                return;

            if (ItemManager.instance.CurrentGameMode == ItemManager.GameModeCode.DecorateFloor)
                ItemManager.instance.ClearTempFloorTileReferencing();

            RaycastHit Hit;

            float Distance = OutOfRangeDistance == 0 ? ActionDistance : OutOfRangeDistance;

            if (Physics.Raycast(GetRay(), out Hit, Distance, FreeLayers))
            {
                if (PreviewMovementType == MovementType.Normal)
                    CurrentPreview.transform.position = Hit.point + CurrentPreview.PreviewOffset;
                else if (PreviewMovementType == MovementType.Grid)
                    CurrentPreview.transform.position = MathExtension.PositionToGridPosition(PreviewGridSize, PreviewGridOffset, Hit.point + CurrentPreview.PreviewOffset);
                else if (PreviewMovementType == MovementType.Smooth)
                    CurrentPreview.transform.position = Vector3.Lerp(CurrentPreview.transform.position, Hit.point + CurrentPreview.PreviewOffset, PreviewSmoothTime * Time.deltaTime);
                else if (PreviewMovementType == MovementType.GridwFreeHeight)
                {
                    CurrentPreview.transform.position = MathExtension.PositionToGridPosition(PreviewGridSize, PreviewGridOffset, Hit.point + CurrentPreview.PreviewOffset);
                    CurrentPreview.transform.position = new Vector3(CurrentPreview.transform.position.x, Hit.point.y + CurrentPreview.PreviewOffset.y, CurrentPreview.transform.position.z);
                }

                if (!CurrentPreview.RotateAccordingSlope)
                {
                    if (LockRotation)
                        CurrentPreview.transform.rotation = GetTransform.rotation * SelectedPrefab.transform.localRotation * Quaternion.Euler(CurrentPreview.RotationAxis + CurrentRotationOffset);
                    else
                        CurrentPreview.transform.rotation = Quaternion.Euler(CurrentPreview.RotationAxis + CurrentRotationOffset);
                }
                else
                {
                    Quaternion SlopeRotation = Quaternion.identity;

                    if (Hit.collider is TerrainCollider)
                    {
                        float SampleHeight = Hit.collider.GetComponent<UnityEngine.Terrain>().SampleHeight(Hit.point);

                        if (Hit.point.y - .1f < SampleHeight)
                        {
                            CurrentPreview.transform.rotation = Quaternion.FromToRotation(GetTransform.up, Hit.normal) * Quaternion.Euler(CurrentRotationOffset)
                                * GetTransform.rotation * SelectedPrefab.transform.localRotation * Quaternion.Euler(CurrentPreview.RotationAxis + CurrentRotationOffset);
                        }
                        else
                            CurrentPreview.transform.rotation = GetTransform.rotation * SelectedPrefab.transform.localRotation * Quaternion.Euler(CurrentPreview.RotationAxis + CurrentRotationOffset);
                    }
                    else
                        CurrentPreview.transform.rotation = GetTransform.rotation * SelectedPrefab.transform.localRotation * Quaternion.Euler(CurrentPreview.RotationAxis + CurrentRotationOffset);
                }

                return;
            }

            if (LockRotation)
                CurrentPreview.transform.rotation = GetTransform.rotation * SelectedPrefab.transform.localRotation * Quaternion.Euler(CurrentPreview.RotationAxis + CurrentRotationOffset);
            else
                CurrentPreview.transform.rotation = Quaternion.Euler(CurrentPreview.RotationAxis + CurrentRotationOffset);

            Transform StartTransform = (CurrentPreview.GroundUpperHeight == 0) ? GetTransform : BuilderCamera.transform;

            Vector3 LookDistance = StartTransform.position + StartTransform.forward * Distance;

            if (CurrentPreview.UseGroundUpper)
            {
                LookDistance.y = Mathf.Clamp(LookDistance.y, GetTransform.position.y - CurrentPreview.GroundUpperHeight,
                    GetTransform.position.y + CurrentPreview.GroundUpperHeight);
            }
            else
            {
                if (!CurrentPreview.FreePlacement)
                {
                    if (Physics.Raycast(CurrentPreview.transform.position + new Vector3(0, 0.3f, 0),
                        Vector3.down, out Hit, Mathf.Infinity, FreeLayers, QueryTriggerInteraction.Ignore))
                        LookDistance.y = Hit.point.y;
                }
                else
                    LookDistance.y = Mathf.Clamp(LookDistance.y, GetTransform.position.y,
                        GetTransform.position.y);
            }

            if (PreviewMovementType == MovementType.Normal)
                CurrentPreview.transform.position = LookDistance;
            else if (PreviewMovementType == MovementType.Grid)
                CurrentPreview.transform.position = MathExtension.PositionToGridPosition(PreviewGridSize, PreviewGridOffset, LookDistance + CurrentPreview.PreviewOffset);
            else if (PreviewMovementType == MovementType.Smooth)
                CurrentPreview.transform.position = Vector3.Lerp(CurrentPreview.transform.position, LookDistance, PreviewSmoothTime * Time.deltaTime);
            else if (PreviewMovementType == MovementType.GridwFreeHeight)
            {
                CurrentPreview.transform.position = MathExtension.PositionToGridPosition(PreviewGridSize, PreviewGridOffset, LookDistance + CurrentPreview.PreviewOffset);
                CurrentPreview.transform.position = new Vector3(CurrentPreview.transform.position.x, LookDistance.y, CurrentPreview.transform.position.z);
            }

            CurrentSocket = null;

            LastSocket = null;

            HasSocket = false;
        }

        /// <summary>
        /// This method allows to move the preview only on available socket(s).
        /// </summary>
        public void UpdateSnapsMovement(SocketBehaviour[] sockets)
        {
            if (CurrentPreview == null)
                return;

            float ClosestAngle = Mathf.Infinity;

            CurrentSocket = null;

            for (int i = 0; i < sockets.Length; i++)
            {
                PartBehaviour Part = sockets[i].AttachedPart;

                if (Part == null || Part.Sockets.Length == 0)
                    continue;

                for (int x = 0; x < Part.Sockets.Length; x++)
                {
                    SocketBehaviour Socket = Part.Sockets[x];

                    if (Socket != null)
                    {
                        if (Socket.gameObject.activeSelf && !Socket.IsDisabled)
                        {
                            if (Socket.AllowPart(SelectedPrefab) && !Part.AvoidAnchoredOnSocket)
                            {
                                if (RayDetection == DetectionType.Overlap)
                                {
                                    if ((Socket.transform.position - (CameraType != RayType.TopDown ? GetTransform.position : TopDownHit.point)).sqrMagnitude <
                                        Mathf.Pow(CameraType != RayType.TopDown ? ActionDistance : SnapThreshold, 2))
                                    {
                                        float Angle = Vector3.Angle(GetRay().direction, Socket.transform.position - GetRay().origin);

                                        if (Angle < ClosestAngle && Angle < OverlapAngles)
                                        {
                                            ClosestAngle = Angle;

                                            if (CameraType != RayType.TopDown && CurrentSocket == null)
                                                CurrentSocket = Socket;
                                            else
                                                CurrentSocket = Socket;
                                        }
                                    }
                                }
                                else
                                {
                                    CurrentSocket = Socket;
                                }
                            }
                        }
                    }
                }
            }

            if (CurrentSocket != null)
            {
                PartOffset Offset = CurrentSocket.GetOffsetPart(SelectedPrefab.Id);

                if (CurrentSocket.CheckOccupancy(SelectedPrefab))
                    return;

                if (Offset != null)
                {
                    CurrentPreview.transform.position = CurrentSocket.transform.position + CurrentSocket.transform.TransformVector(Offset.Position);

                    CurrentPreview.transform.rotation = CurrentSocket.transform.rotation * (CurrentPreview.RotateOnSockets ? Quaternion.Euler(Offset.Rotation + CurrentRotationOffset) : Quaternion.Euler(Offset.Rotation));

                    if (Offset.UseCustomScale)
                        CurrentPreview.transform.localScale = Offset.Scale;

                    LastSocket = CurrentSocket;

                    HasSocket = true;

                    return;
                }
            }

            UpdateFreeMovement();
        }

        /// <summary>
        /// This method allows to update the preview collisions.
        /// </summary>
        protected void UpdatePreviewCollisions()
        {
            bool HasCollision = CurrentPreview.CheckTerrainClipping();

            if (CurrentPreview.AvoidClipping && !HasCollision && !HasSocket)
                HasCollision = CurrentPreview.CheckElementsClipping(FreeLayers);
            else if (CurrentPreview.AvoidClippingOnSocket && !HasCollision && HasSocket)
                HasCollision = CurrentPreview.CheckElementsClippingOnSocket(FreeLayers);

            if (!HasCollision)
                HasCollision = CurrentPreview.CheckAreas();

            if (HasSocket)
                AllowPlacement = !HasCollision;
            else
                AllowPlacement = !CurrentPreview.RequireSockets && !HasCollision;

            if (!HasCollision && AllowPlacement)
                if (OutOfRangeDistance != 0)
                    AllowPlacement = Vector3.Distance(GetTransform.position, CurrentPreview.transform.position) < ActionDistance;

            if (!HasCollision && CurrentPreview.UseConditionalPhysics && CurrentPreview.PhysicsOnlyStablePlacement)
                AllowPlacement = AllowPlacement && CurrentPreview.CheckStability();

            RaycastHit Hit;

            if (!CurrentPreview.FreePlacement)
            {
                if (CurrentPreview.Type == PartType.Foundation)
                {
                    if (!HasSocket)
                    {
                        if (Physics.Raycast(CurrentPreview.transform.position + new Vector3(0, 0.1f, 0), Vector3.down, out Hit,
                            CurrentPreview.UseGroundUpper ? CurrentPreview.GroundUpperHeight : 0.3f, FreeLayers))
                        {
                            if (TerrainManager.Instance != null)
                            {
                                if (TerrainManager.Instance.ActiveTerrain != null)
                                {
                                    if (BuildManager.Instance.BuildingSupport == SupportType.Terrain)
                                    {
                                        if (Hit.collider as TerrainCollider == null)
                                        {
                                            if (AllowPlacement)
                                                AllowPlacement = false;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (BuildManager.Instance.BuildingSupport == SupportType.Voxeland)
                                {
                                    if (Hit.collider.GetComponentInParent<VoxelandCollider>() == null)
                                    {
                                        if (AllowPlacement)
                                            AllowPlacement = false;
                                    }
                                }
                                else if (BuildManager.Instance.BuildingSupport == SupportType.Surface)
                                {
                                    if (Hit.collider.GetComponent<SurfaceCollider>() == null)
                                    {
                                        if (AllowPlacement)
                                            AllowPlacement = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (AllowPlacement)
                                AllowPlacement = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method allows to place the current preview.
        /// </summary>
        public virtual void PlacePrefab()
        {
            if (!AllowPlacement)
                return;

            if (CurrentPreview == null)
                return;

            if (CurrentEditionPreview != null)
                Destroy(CurrentEditionPreview.gameObject);

            BuildManager.Instance.PlacePrefab(SelectedPrefab,
                CurrentPreview.transform.position,
                CurrentPreview.transform.eulerAngles,
                CurrentPreview.transform.localScale,
                null,
                CurrentSocket);

            if (Source != null)
                if (PlacementClips.Length != 0)
                    Source.PlayOneShot(PlacementClips[Random.Range(0, PlacementClips.Length)]);

            CurrentRotationOffset = Vector3.zero;

            CurrentSocket = null;

            LastSocket = null;

            AllowPlacement = false;

            HasSocket = false;

            if (CurrentMode == BuildMode.Placement && ResetModeAfterPlacement)
                ChangeMode(BuildMode.None);

            if (CurrentPreview != null)
                Destroy(CurrentPreview.gameObject);
        }

        /// <summary>
        /// This method allows to create a preview.
        /// </summary>
        public virtual PartBehaviour CreatePreview(GameObject prefab)
        {
            if (prefab == null)
                return null;

            CurrentPreview = Instantiate(prefab).GetComponent<PartBehaviour>();

            RaycastHit Hit;

            if (Physics.Raycast(GetRay(), out Hit, Mathf.Infinity, FreeLayers, QueryTriggerInteraction.Ignore))
                CurrentPreview.transform.position = Hit.point;

            CurrentPreview.ChangeState(StateType.Preview);

            SelectedPrefab = prefab.GetComponent<PartBehaviour>();

            if (UsePreviewCamera == true)
                CurrentPreview.gameObject.SetLayerRecursively(PreviewLayer);

            EventHandlers.PreviewCreated(CurrentPreview);

            CurrentSocket = null;

            LastSocket = null;

            AllowPlacement = false;

            HasSocket = false;

            return CurrentPreview;
        }

        /// <summary>
        /// This method allows to clear the current preview.
        /// </summary>
        public virtual void ClearPreview()
        {
            if (CurrentPreview == null)
                return;

            EventHandlers.PreviewCanceled(CurrentPreview);

            Destroy(CurrentPreview.gameObject);

            AllowPlacement = false;

            CurrentPreview = null;
        }

        /// <summary>
        /// This method allows to check if the current preview exists.
        /// </summary>
        public bool PreviewExists()
        {
            return CurrentPreview;
        }

        #endregion Placement

        #region Destruction

        /// <summary>
        /// This method allows to update the destruction preview.
        /// </summary>
        public void UpdateRemovePreview()
        {
            RaycastHit Hit;

            foreach (SocketBehaviour Socket in BuildManager.Instance.Sockets)
                if (Socket != null)
                    Socket.DisableCollider();

            float Distance = OutOfRangeDistance == 0 ? ActionDistance : OutOfRangeDistance;

            if (CurrentRemovePreview != null)
            {
                AreaBehaviour NearestArea = BuildManager.Instance.GetNearestArea(CurrentRemovePreview.transform.position, CurrentPreview.MeshBounds, CurrentPreview.transform.eulerAngles.y);

                if (NearestArea != null)
                    AllowDestruction = NearestArea.AllowDestruction;
                else
                    AllowDestruction = true;

                CurrentRemovePreview.ChangeState(StateType.Remove);

                AllowPlacement = false;
            }

            if (Physics.Raycast(GetRay(), out Hit, Distance, FreeLayers))
            {
                PartBehaviour Part = Hit.collider.GetComponentInParent<PartBehaviour>();

                if (Part != null)
                {
                    if (CurrentRemovePreview != null)
                    {
                        if (CurrentRemovePreview.GetInstanceID() != Part.GetInstanceID())
                        {
                            ClearRemovePreview();

                            CurrentRemovePreview = Part;
                        }
                    }
                    else
                        CurrentRemovePreview = Part;
                }
                else
                    ClearRemovePreview();
            }
            else
                ClearRemovePreview();
        }

        /// <summary>
        /// This method allows to remove the current preview.
        /// </summary>
        public virtual void RemovePrefab()
        {
            if (CurrentRemovePreview == null)
                return;

            if (!AllowDestruction)
                return;

            Destroy(CurrentRemovePreview.gameObject);

            if (Source != null)
                if (DestructionClips.Length != 0)
                    Source.PlayOneShot(DestructionClips[Random.Range(0, DestructionClips.Length)]);

            CurrentSocket = null;

            LastSocket = null;

            AllowDestruction = false;

            HasSocket = false;

            if (ResetModeAfterDestruction)
                ChangeMode(BuildMode.None);
        }

        /// <summary>
        /// This method allows to clear the current remove preview.
        /// </summary>
        public virtual void ClearRemovePreview()
        {
            if (CurrentRemovePreview == null)
                return;

            CurrentRemovePreview.ChangeState(CurrentRemovePreview.LastState);

            AllowDestruction = false;

            CurrentRemovePreview = null;
        }

        #endregion Destruction

        #region Edition

        /// <summary>
        /// This method allows to update the edition mode.
        /// </summary>
        public void UpdateEditionPreview()
        {
            RaycastHit Hit;

            AllowEdition = CurrentEditionPreview;

            if (CurrentEditionPreview != null && AllowEdition)
                CurrentEditionPreview.ChangeState(StateType.Edit);

            float Distance = OutOfRangeDistance == 0 ? ActionDistance : OutOfRangeDistance;

            if (Physics.Raycast(GetRay(), out Hit, Distance, FreeLayers))
            {
                PartBehaviour Part = Hit.collider.GetComponentInParent<PartBehaviour>();

                if (Part != null)
                {
                    if (CurrentEditionPreview != null)
                    {
                        if (CurrentEditionPreview.GetInstanceID() != Part.GetInstanceID())
                        {
                            ClearEditionPreview();

                            CurrentEditionPreview = Part;
                        }
                    }
                    else
                        CurrentEditionPreview = Part;
                }
                else
                {
                    ClearEditionPreview();
                }
            }
            else
                ClearEditionPreview();
        }

        /// <summary>
        /// This method allows to edit the current preview.
        /// </summary>
        public virtual void EditPrefab()
        {
            if (!AllowEdition)
                return;

            PartBehaviour Part = CurrentEditionPreview;

            Part.ChangeState(StateType.Edit);

            EventHandlers.EditedPart(CurrentEditionPreview, CurrentSocket);

            SelectPrefab(Part);

            SelectedPrefab.AppearanceIndex = Part.AppearanceIndex;

            ChangeMode(BuildMode.Placement);
        }

        /// <summary>
        /// This method allows to clear the current edition preview.
        /// </summary>
        public void ClearEditionPreview()
        {
            if (CurrentEditionPreview == null)
                return;

            CurrentEditionPreview.ChangeState(CurrentEditionPreview.LastState);

            AllowEdition = false;

            CurrentEditionPreview = null;
        }

        #endregion Edition

        #region Miscs

        /// <summary>
        /// This method allows to change mode.
        /// </summary>
        public void ChangeMode(BuildMode mode)
        {
            if (CurrentMode == mode)
                return;

            if (mode == BuildMode.Placement && !UsePlacementMode)
                return;

            if (mode == BuildMode.Destruction && !UseDestructionMode)
                return;

            if (mode == BuildMode.Edition && !UseEditionMode)
                return;

            if (CurrentMode == BuildMode.Placement)
                ClearPreview();

            if (CurrentMode == BuildMode.Destruction)
                ClearRemovePreview();

            if (mode == BuildMode.None)
            {
                ClearPreview();
                ClearRemovePreview();
                ClearEditionPreview();
            }

            CurrentMode = mode;

            EventHandlers.BuildModeChanged(CurrentMode);
        }

        /// <summary>
        /// This method allows to select a prefab.
        /// </summary>
        public void SelectPrefab(PartBehaviour prefab)
        {
            if (prefab == null)
                return;

            SelectedPrefab = BuildManager.Instance.GetPart(prefab.Id);
        }

        /// <summary>
        /// This method allows to rotate the current preview.
        /// </summary>
        public void RotatePreview(Vector3 rotateAxis)
        {
            if (CurrentPreview == null)
                return;

            CurrentRotationOffset += rotateAxis;

            // Alan
            CurrentPreview.transform.rotation = Quaternion.Euler(CurrentPreview.RotationAxis + CurrentRotationOffset);

            UpdatePreviewCollisions();

            CurrentPreview.gameObject.ChangeAllMaterialsColorInChildren(CurrentPreview.Renderers.ToArray(),
                AllowPlacement ? (ItemManager.instance.CurrentGameMode == ItemManager.GameModeCode.DecorateFloor ? new Color(1f, 1f, 1f) : BuildManager.Instance.PreviewAllowedColor) : BuildManager.Instance.PreviewDeniedColor, SelectedPrefab.PreviewColorLerpTime, SelectedPrefab.PreviewUseColorLerpTime);
        }

        /// <summary>
        /// This method allows to get the object that the camera is currently looking at.
        /// </summary>
        public PartBehaviour GetTargetedPart()
        {
            RaycastHit Hit;

            if (Physics.SphereCast(CameraType == RayType.FirstPerson ? new Ray(BuilderCamera.transform.position, BuilderCamera.transform.forward) : GetRay(),
                .1f, out Hit, ActionDistance, LayerMask.NameToLayer(Constants.LAYER_DEFAULT.ToLower())))
            {
                PartBehaviour Part = Hit.collider.GetComponentInParent<PartBehaviour>();

                if (Part != null)
                    return Part;
            }

            return null;
        }

        #endregion Miscs

        #endregion Public Methods
    }
}