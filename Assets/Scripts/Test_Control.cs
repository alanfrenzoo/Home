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

public class Test_Control : MonoBehaviour
{
    public static Test_Control instance;
    public bool isUsingECS = false;

    public GameObject BuildContent;
    public Button ValidateButton;
    public Button CancelButton;
    public Button RotateButton;
    public Button DestructionButton;

    public Transform playerTransform;

    private EntityManager entityManager;
    private Dictionary<int, Entity> indexEntityPair;

    public static int currentFurniIndex;
    private GameObject currentCollider;

    private bool editing = false;
    public bool Editing
    {
        get { return editing; }
        set { editing = value; }
    }

    public void Start()
    {
        instance = this;

        entityManager = World.Active.EntityManager;
        indexEntityPair = new Dictionary<int, Entity>();

        ValidateButton.onClick.AddListener(() =>
        {
            // To confirm Placement or Edition (moving)
            if (BuilderBehaviour.Instance.CurrentMode == BuildMode.Placement)
            {
                if (!isUsingECS)
                    BuilderBehaviour.Instance.PlacePrefab();
                else
                    ECS_PlacePrefab();
            }
            //else if (BuilderBehaviour.Instance.CurrentMode == BuildMode.Edition)
            //    BuilderBehaviour.Instance.EditPrefab();
            //else if (BuilderBehaviour.Instance.CurrentMode == BuildMode.Destruction)
            //    BuilderBehaviour.Instance.RemovePrefab();

            Editing = false;

            BuildContent.SetActive(false);
            BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
            BuilderBehaviour.Instance.ChangeMode(BuildMode.Edition);
        });

        CancelButton.onClick.AddListener(() =>
        {
            // if we are in Edition mode, there are 2 parts we are working on
            // 1: CurrentEditionPreview that stays on the original position
            // 2: CurrentPreview that moves

            // Edition
            if (BuilderBehaviour.Instance.CurrentEditionPreview != null)
            {
                Destroy(BuilderBehaviour.Instance.CurrentPreview.gameObject);
                BuilderBehaviour.Instance.CurrentEditionPreview.gameObject.SetActive(true);
            }

            if (isUsingECS && currentCollider != null)
            {
                Destroy(BuilderBehaviour.Instance.CurrentPreview.gameObject);

                //

                var workingObject = BuildManager.Instance.PartsCollection.Parts[currentFurniIndex].gameObject;

                Entity prefab;
                if (indexEntityPair.ContainsKey(currentFurniIndex))
                {
                    indexEntityPair.TryGetValue(currentFurniIndex, out prefab);
                }
                else
                {
                    // Create entity prefab from the game object hierarchy once
                    prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(workingObject, World.Active);
                    indexEntityPair[currentFurniIndex] = prefab;
                }

                // Efficiently instantiate a bunch of entities from the already converted entity prefab
                var instance = entityManager.Instantiate(prefab);

                var position = currentCollider.transform.position + workingObject.transform.localPosition;
                var rotation = Quaternion.Euler(currentCollider.transform.eulerAngles);

                entityManager.SetComponentData(instance, new Translation { Value = position });
                entityManager.SetComponentData(instance, new Rotation { Value = rotation });
                entityManager.AddComponentData(instance, new FurniTag { Value = currentFurniIndex });

                //

                currentCollider.SetActive(true);

            }

            // Clear referencing
            currentCollider = null;

            Editing = false;

            BuildContent.SetActive(false);
            BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
            BuilderBehaviour.Instance.ChangeMode(BuildMode.Edition);
        });

        RotateButton.onClick.AddListener(() =>
        {
            if (BuilderBehaviour.Instance.SelectedPrefab != null)
            {
                BuilderBehaviour.Instance.RotatePreview(BuilderBehaviour.Instance.SelectedPrefab.RotationAxis);
                BuilderBehaviour.Instance.UpdatePreview();
            }

        });

        DestructionButton.onClick.AddListener(() =>
        {
            // Destroy Both the hidden and moving object if any
            if (BuilderBehaviour.Instance.CurrentEditionPreview != null)
                Destroy(BuilderBehaviour.Instance.CurrentEditionPreview.gameObject);
            if (BuilderBehaviour.Instance.CurrentPreview != null)
                Destroy(BuilderBehaviour.Instance.CurrentPreview.gameObject);

            if (isUsingECS && currentCollider != null)
            {
                Destroy(currentCollider);
                EventHandlers.PlacedPart(null, null);
            }

            Editing = false;

            BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
            BuilderBehaviour.Instance.ChangeMode(BuildMode.Edition);
            BuildContent.SetActive(false);
        });
    }

    private void ECS_PlacePrefab()
    {

        if (currentCollider != null)
            Destroy(currentCollider);

        if (!BuilderBehaviour.Instance.AllowPlacement)
            return;

        if (BuilderBehaviour.Instance.CurrentPreview == null)
            return;

        if (BuilderBehaviour.Instance.CurrentEditionPreview != null)
            Destroy(BuilderBehaviour.Instance.CurrentEditionPreview.gameObject);

        // Replace the gameObject logic to entity

        var objectEntity = BuilderBehaviour.Instance.SelectedPrefab.transform.GetChild(0).gameObject;

        Entity prefab;
        if (indexEntityPair.ContainsKey(currentFurniIndex))
        {
            indexEntityPair.TryGetValue(currentFurniIndex, out prefab);
        }
        else
        {
            // Create entity prefab from the game object hierarchy once
            prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(objectEntity, World.Active);
            indexEntityPair[currentFurniIndex] = prefab;
        }

        // Efficiently instantiate a bunch of entities from the already converted entity prefab
        var instance = entityManager.Instantiate(prefab);

        var position = BuilderBehaviour.Instance.CurrentPreview.transform.position + objectEntity.transform.localPosition;
        var rotation = Quaternion.Euler(BuilderBehaviour.Instance.CurrentPreview.transform.eulerAngles);

        entityManager.SetComponentData(instance, new Translation { Value = position });
        entityManager.SetComponentData(instance, new Rotation { Value = rotation });
        entityManager.AddComponentData(instance, new FurniTag { Value = currentFurniIndex });

        var colliderToSpawn = new GameObject("c");
        colliderToSpawn.AddComponent<MeshCollider>();
        colliderToSpawn.GetComponent<MeshCollider>().sharedMesh = objectEntity.GetComponentInChildren<MeshCollider>().sharedMesh;
        colliderToSpawn.transform.position = position;
        colliderToSpawn.transform.rotation = rotation;
        colliderToSpawn.layer = LayerMask.NameToLayer("Furniture");

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

        if (BuilderBehaviour.Instance.CurrentPreview != null)
            Destroy(BuilderBehaviour.Instance.CurrentPreview.gameObject);

        Editing = false;

    }

    public void onBuild(int index)
    {
        currentFurniIndex = index;

        BuilderBehaviour.Instance.ChangeMode(BuildMode.None);

        // Select and start Placement
        BuilderBehaviour.Instance.SelectPrefab(BuildManager.Instance.PartsCollection.Parts[index]);
        BuilderBehaviour.Instance.ChangeMode(BuildMode.Placement);

        // Move to Centre
        BuilderBehaviour.Instance.CreatePreview(BuilderBehaviour.Instance.SelectedPrefab.gameObject);
        BuilderBehaviour.Instance.CurrentPreview.transform.position = MathExtension.PositionToGridPosition(BuilderBehaviour.Instance.PreviewGridSize, BuilderBehaviour.Instance.PreviewGridOffset, BuilderBehaviour.Instance.CurrentPreview.PreviewOffset);

        DestructionButton.gameObject.SetActive(false);

        Editing = true;
    }

    private void Update()
    {

        // Update the postion of BuildContent
        if (BuilderBehaviour.Instance.CurrentPreview != null)
        {
            if (!BuildContent.activeSelf)
                BuildContent.SetActive(true);

            Vector2 myPositionOnScreen = Camera.main.WorldToScreenPoint(BuilderBehaviour.Instance.CurrentPreview.transform.position);

            Canvas copyOfMainCanvas = GameObject.Find("TempUI").GetComponent<Canvas>();
            float scaleFactor = copyOfMainCanvas.scaleFactor;

            BuildContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(myPositionOnScreen.x / scaleFactor, myPositionOnScreen.y / scaleFactor);
        }

        if (Input.GetMouseButtonDown(0) == false || Input.touchCount > 1)
            return;

        // Bypass its workflow
        // Once we pick an object for edition, start edit (move) immediately 
        if (BuilderBehaviour.Instance.CurrentEditionPreview != null)
        {
            BuilderBehaviour.Instance.EditPrefab();

            // Hide the object that stays at its position
            if (BuilderBehaviour.Instance.CurrentPreview != null)
                BuilderBehaviour.Instance.CurrentEditionPreview.gameObject.SetActive(false);

            if (!DestructionButton.gameObject.activeSelf)
                DestructionButton.gameObject.SetActive(true);
        }

        // ECS Edition
        if (isUsingECS && !Editing)
        {
            RaycastHit Hit;

            float Distance = BuilderBehaviour.Instance.OutOfRangeDistance == 0 ? BuilderBehaviour.Instance.ActionDistance : BuilderBehaviour.Instance.OutOfRangeDistance;

            if (Physics.Raycast(GetRay(), out Hit, Distance, LayerMask.GetMask("Furniture")))
            {
                if (Hit.collider != null)
                {
                    Editing = true;

                    Entity entities = entityManager.CreateEntity();
                    entityManager.AddComponentData(entities, new Translation { Value = Hit.collider.transform.position });
                    entityManager.AddComponentData(entities, new DestroyTag { });

                    currentCollider = Hit.collider.gameObject;
                    currentCollider.SetActive(false);

                    // ready for destroy
                    DestructionButton.gameObject.SetActive(true);
                }
            }
        }

    }

    private Ray GetRay()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition + new Vector3(0, 0, BuilderBehaviour.Instance.RaycastOffset));
    }


}
