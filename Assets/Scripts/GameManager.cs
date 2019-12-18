using Deform;
using EasyBuildSystem.Runtimes;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Extensions;
using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    #region ECS
    public bool isUsingECS = true;

    private GameObjectConversionSettings settings;
    private EntityManager entityManager;
    private Dictionary<int, Entity> indexEntityPair;
    #endregion

    public enum GameModeCode
    {
        View,
        DecorateFurniture,
        DecorateFloor
    }

    public void Start()
    {
        instance = this;

        settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        indexEntityPair = new Dictionary<int, Entity>();
    }

    private void Update()
    {
        if (!Input.GetMouseButton(0))
            return;

        if (CurrentGameMode == GameModeCode.View)
        {
            if (ScreenTouchManager.instance.CheckOnItemPress())
            {
                if (ScreenTouchManager.instance.GetPressTimePortion() > 1f)
                {
                    // Start UI Animation
                    ControlManager.instance.PositionUILoadingBar();
                    ControlManager.instance.LoadingLayer.sizeDelta = new Vector2(145f * (1f - ScreenTouchManager.instance.GetLongPressTimePortion()), ControlManager.instance.LoadingLayer.rect.height);

                    if (ScreenTouchManager.instance.GetLongPressTimePortion() > 1f)
                    {
                        TargetCollider = NewCollider;
                        TargetCollider.SetActive(false);

                        ControlManager.instance.DisableLoadingBar();

                        LiftUpItem();

                    }

                }
            }
            else
            {
                UpdateCameraMovement();
            }
        }
        else if (CurrentGameMode == GameModeCode.DecorateFurniture)
        {
            if (ScreenTouchManager.instance.CheckOnItemPress())
            {
                if (!isEditing || TargetCollider != NewCollider)
                {
                    if (ScreenTouchManager.instance.GetPressTimePortion() > 1f)
                    {
                        // No UI Animation
                        if (ScreenTouchManager.instance.GetShortPressTimePortion() > 1f)
                        {
                            if (isEditing)
                            {
                                CancelPlacement();
                            }

                            TargetCollider = NewCollider;
                            TargetCollider.SetActive(false);

                            LiftUpItem();

                        }
                    }
                }
            }
            else
            {
                UpdateCameraMovement();
            }
        }
        //else if (CurrentGameMode == GameModeCode.DecorateFloor)
        //{ }
    }

    public void InstantiateItem(int index)
    {
        CancelPlacement();

        TargetItemIndex = index;
        BuilderBehaviour.Instance.ChangeMode(BuildMode.None);

        // Select and start Placement
        BuilderBehaviour.Instance.SelectPrefab(BuildManager.Instance.PartsCollection.Parts[index]);
        BuilderBehaviour.Instance.ChangeMode(BuildMode.Placement);
        BuilderBehaviour.Instance.CreatePreview(BuilderBehaviour.Instance.SelectedPrefab.gameObject);

        TargetEditingItem = BuilderBehaviour.Instance.CurrentPreview.gameObject;

        ControlManager.instance.EnableBuildContent(false);

        IsEditing = true;
    }

    private void LiftUpItem()
    {
        Entity entities = entityManager.CreateEntity();
        entityManager.AddComponentData(entities, new Translation { Value = TargetCollider.transform.position });
        entityManager.AddComponentData(entities, new DestroyTag { Value = int.Parse(TargetCollider.name) });

        // with destroy
        ControlManager.instance.EnableBuildContent(true);
        ControlManager.instance.leftBarController.EnterDecorateMode();

        isEditing = true;
    }

    public void PlaceItem()
    {
        if (!BuilderBehaviour.Instance.AllowPlacement)
            return;

        if (BuilderBehaviour.Instance.CurrentPreview == null)
            return;

        if (TargetCollider != null)
            Destroy(TargetCollider);

        // Replace the gameObject logic to entity

        var workingObject = BuilderBehaviour.Instance.SelectedPrefab.transform.GetChild(0).gameObject;

        Entity prefab;
        if (indexEntityPair.ContainsKey(TargetItemIndex))
        {
            indexEntityPair.TryGetValue(TargetItemIndex, out prefab);
        }
        else
        {
            // Create entity prefab from the game object hierarchy once
            prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(workingObject, settings);
            indexEntityPair[TargetItemIndex] = prefab;
        }

        // Efficiently instantiate a bunch of entities from the already converted entity prefab
        var instance = entityManager.Instantiate(prefab);

        var position = BuilderBehaviour.Instance.CurrentPreview.transform.position + workingObject.transform.localPosition;
        var rotation = Quaternion.Euler(BuilderBehaviour.Instance.CurrentPreview.transform.eulerAngles);

        entityManager.SetComponentData(instance, new Translation { Value = position });
        entityManager.SetComponentData(instance, new Rotation { Value = rotation });
        entityManager.AddComponentData(instance, new FurniTag { Value = TargetItemIndex });

        var colliderToSpawn = new GameObject(TargetItemIndex.ToString());
        colliderToSpawn.AddComponent<MeshCollider>();
        colliderToSpawn.GetComponent<MeshCollider>().sharedMesh = workingObject.GetComponentInChildren<MeshCollider>().sharedMesh;
        colliderToSpawn.transform.position = position;
        colliderToSpawn.transform.rotation = rotation;
        colliderToSpawn.layer = LayerMask.NameToLayer("Furniture");

        if (BuilderBehaviour.Instance.CurrentPreview.Type == EasyBuildSystem.Runtimes.Internal.Part.PartType.Desk)
        {
            // Hard code for stable support
            colliderToSpawn.tag = "Desk";

            // Hard code for Desk Spot
            var spotdl = new GameObject("spot_desk_l");
            spotdl.transform.SetParent(colliderToSpawn.transform);
            spotdl.transform.localPosition = new Vector3(-2f, 0f, 0f);
            spotdl.transform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
            var spotdr = new GameObject("spot_desk_r");
            spotdr.transform.SetParent(colliderToSpawn.transform);
            spotdr.transform.localPosition = new Vector3(2f, 0f, 0f);
            spotdr.transform.localRotation = Quaternion.Euler(new Vector3(0f, -90f, 0f));
        }

        var colliderList = workingObject.GetComponentsInChildren<MeshCollider>();
        if (colliderList.Length > 1)
        {
            for (int i = 1; i < colliderList.Length; i++)
            {
                var child = new GameObject("");
                child.AddComponent<MeshCollider>();
                child.GetComponent<MeshCollider>().sharedMesh = colliderList[i].sharedMesh;
                child.transform.SetParent(colliderToSpawn.transform);
                child.transform.localPosition = colliderList[i].transform.localPosition;
                child.transform.localRotation = colliderList[i].transform.localRotation;

                // Hard code for Chair Spot
                // Hard code for Desk Spot
                var spotcl = new GameObject("spot_chair_l");
                spotcl.transform.SetParent(child.transform);
                spotcl.transform.localPosition = new Vector3(-1f, 0f, 0f);
                spotcl.transform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
                var spotcr = new GameObject("spot_chair_r");
                spotcr.transform.SetParent(child.transform);
                spotcr.transform.localPosition = new Vector3(1f, 0f, 0f);
                spotcr.transform.localRotation = Quaternion.Euler(new Vector3(0f, -90f, 0f));
            }
        }

        // Update the NavMesh
        EventHandlers.PlacedPart(null, null);

        // Finish Up the remaining flow

        if (BuilderBehaviour.Instance.Source != null)
            if (BuilderBehaviour.Instance.PlacementClips.Length != 0)
                BuilderBehaviour.Instance.Source.PlayOneShot(BuilderBehaviour.Instance.PlacementClips[UnityEngine.Random.Range(0, BuilderBehaviour.Instance.PlacementClips.Length)]);

        BuilderBehaviour.Instance.CurrentRotationOffset = Vector3.zero;

        BuilderBehaviour.Instance.CurrentSocket = null;

        BuilderBehaviour.Instance.LastSocket = null;

        BuilderBehaviour.Instance.AllowPlacement = false;

        BuilderBehaviour.Instance.HasSocket = false;

        ClearReferencingAndResetMode();
    }

    public void CancelPlacement()
    {
        if (TargetCollider != null)
        {
            var workingObject = BuildManager.Instance.PartsCollection.Parts[TargetItemIndex].gameObject;

            Entity prefab;
            if (indexEntityPair.ContainsKey(TargetItemIndex))
            {
                indexEntityPair.TryGetValue(TargetItemIndex, out prefab);
            }
            else
            {
                // Create entity prefab from the game object hierarchy once
                prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(workingObject, settings);
                indexEntityPair[TargetItemIndex] = prefab;
            }

            // Efficiently instantiate a bunch of entities from the already converted entity prefab
            var instance = entityManager.Instantiate(prefab);

            var position = GameManager.instance.TargetCollider.transform.position + workingObject.transform.localPosition;
            var rotation = Quaternion.Euler(GameManager.instance.TargetCollider.transform.eulerAngles);

            entityManager.SetComponentData(instance, new Translation { Value = position });
            entityManager.SetComponentData(instance, new Rotation { Value = rotation });
            entityManager.AddComponentData(instance, new FurniTag { Value = TargetItemIndex });

            TargetCollider.SetActive(true);
        }

        ClearReferencingAndResetMode();

    }

    public void RemoveItem()
    {
        if (TargetCollider != null)
        {
            Destroy(TargetCollider);
            EventHandlers.PlacedPart(null, null);
        }

        ClearReferencingAndResetMode();
    }

    public void RotateItem()
    {
        if (BuilderBehaviour.Instance.SelectedPrefab != null)
        {
            BuilderBehaviour.Instance.RotatePreview(BuilderBehaviour.Instance.SelectedPrefab.RotationAxis);
            //BuilderBehaviour.Instance.UpdatePreview();
        }
    }

    private void ClearReferencingAndResetMode()
    {
        if (BuilderBehaviour.Instance.CurrentPreview != null)
            Destroy(BuilderBehaviour.Instance.CurrentPreview.gameObject);

        TargetCollider = null;
        IsEditing = false;

        ControlManager.instance.DisableBuildContent();

        BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
        BuilderBehaviour.Instance.ChangeMode(BuildMode.Edition);
    }

    private void UpdateCameraMovement()
    {
        //Handle in PanZoomManager
    }

    private bool isEditing = false;
    public bool IsEditing
    {
        get => isEditing;
        set => isEditing = value;
    }

    private int targetItemIndex = -1;
    public int TargetItemIndex
    {
        get => targetItemIndex;
        set => targetItemIndex = value;
    }

    private GameObject targetCollider = null;
    public GameObject TargetCollider
    {
        get => targetCollider;
        set => targetCollider = value;
    }

    private GameObject newCollider = null;
    public GameObject NewCollider
    {
        get => newCollider;
        set => newCollider = value;
    }

    private GameObject targetEditingItem = null;
    public GameObject TargetEditingItem
    {
        get => targetEditingItem;
        set => targetEditingItem = value;
    }

    private GameModeCode currentGameMode = GameModeCode.View;
    public GameModeCode CurrentGameMode
    {
        get => currentGameMode;
        set
        {
            currentGameMode = value;
            if (currentGameMode == GameModeCode.View)
            {
                // Clear up editing item status
                // i.e. validate or cancel placement?
                // Todo: Pop up warning

                CancelPlacement();

            }
            else if (currentGameMode == GameModeCode.DecorateFurniture)
            {
                //Show Grid

            }
            //else if (currentGameMode == GameModeCode.DecorateFloor)
            //{
            //Prepare for Show/Hide Items & Character
            //}
        }
    }

}
