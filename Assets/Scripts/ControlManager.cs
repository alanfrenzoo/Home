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

public class ControlManager : MonoBehaviour
{
    public static ControlManager instance;

    public Canvas MainCanvas;
    public GameObject BuildContent;
    public Button ValidateButton;
    public Button CancelButton;
    public Button RotateButton;
    public Button DestructionButton;
    public RectTransform LoadingLayer;
    public LeftBarController leftBarController;

    private float scaleFactor;

    public void Start()
    {
        instance = this;
        scaleFactor = MainCanvas.scaleFactor;

        ValidateButton.onClick.AddListener(() =>
        {
            //if (BuilderBehaviour.Instance.CurrentMode == BuildMode.Placement)
            {
                if (GameManager.instance.isUsingECS)
                {
                    GameManager.instance.PlaceItem();
                }
                else
                {
                    BuilderBehaviour.Instance.PlacePrefab();

                    GameManager.instance.IsEditing = false;

                    BuildContent.SetActive(false);
                    BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
                    BuilderBehaviour.Instance.ChangeMode(BuildMode.Edition);
                }
            }

        });

        CancelButton.onClick.AddListener(() =>
        {
            if (GameManager.instance.isUsingECS)
            {
                GameManager.instance.CancelPlacement();
            }
            else
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

                // Clear referencing
                GameManager.instance.TargetCollider = null;

                GameManager.instance.IsEditing = false;

                BuildContent.SetActive(false);
                BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
                BuilderBehaviour.Instance.ChangeMode(BuildMode.Edition);
            }

        });

        RotateButton.onClick.AddListener(() =>
        {
            GameManager.instance.RotateItem();
        });

        DestructionButton.onClick.AddListener(() =>
        {
            if (GameManager.instance.isUsingECS)
            {
                GameManager.instance.RemoveItem();
            }
            else
            {
                // Destroy Both the hidden and moving object if any
                if (BuilderBehaviour.Instance.CurrentEditionPreview != null)
                    Destroy(BuilderBehaviour.Instance.CurrentEditionPreview.gameObject);
                if (BuilderBehaviour.Instance.CurrentPreview != null)
                    Destroy(BuilderBehaviour.Instance.CurrentPreview.gameObject);

                GameManager.instance.IsEditing = false;

                BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
                BuilderBehaviour.Instance.ChangeMode(BuildMode.Edition);
                BuildContent.SetActive(false);
            }

        });
    }

    private void Update()
    {
        // Update the postion of BuildContent
        if (BuilderBehaviour.Instance.CurrentPreview != null)
        {
            PositionUIBuildContent();
        }

        // Handle UI Item To GameObject Instancing
        if (TargetUIItemIndex >= 0)
        {
            var leftBarWidth = leftBarController.GetComponent<RectTransform>().rect.width;
            if (Input.mousePosition.x > leftBarWidth)
            {
                if (Input.GetMouseButton(0))
                {
                    GameManager.instance.InstantiateItem(TargetUIItemIndex);
                    leftBarController.GetComponentInChildren<ScrollRect>().enabled = false;
                }
                TargetUIItemIndex = -1;
            }

        }
    }

    public void MoveUINextTo(GameObject targetUI, Transform worldObject)
    {
        targetUI.SetActive(true);
        Vector2 myPositionOnScreen = Camera.main.WorldToScreenPoint(worldObject.position);
        targetUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(myPositionOnScreen.x / scaleFactor, myPositionOnScreen.y / scaleFactor);
    }

    public void PositionUIBuildContent()
    {
        MoveUINextTo(BuildContent, BuilderBehaviour.Instance.CurrentPreview.transform);
    }

    public void PositionUILoadingBar()
    {
        MoveUINextTo(LoadingLayer.parent.gameObject, GameManager.instance.NewCollider.transform);
    }

    public void EnableLoadingBar()
    {
        LoadingLayer.transform.parent.gameObject.SetActive(true);
    }
    public void DisableLoadingBar()
    {
        LoadingLayer.transform.parent.gameObject.SetActive(false);
    }

    public void EnableBuildContent(bool withDestruction)
    {
        BuildContent.SetActive(true);
        if (withDestruction)
            DestructionButton.gameObject.SetActive(true);
        else
            DestructionButton.gameObject.SetActive(false);
    }
    public void DisableBuildContent()
    {
        BuildContent.SetActive(false);
    }

    private int targetUIItemIndex = -1;
    public int TargetUIItemIndex
    {
        get => targetUIItemIndex;
        set => targetUIItemIndex = value;
    }

}