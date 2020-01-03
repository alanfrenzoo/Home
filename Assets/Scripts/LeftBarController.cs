using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftBarController : MonoBehaviour
{
    public GameObject DecorateMark;

    public Button decorateBtn;
    public Button exitBtn;

    public GameObject FloorViewport;
    public GameObject FurnitureViewport;

    public Button FurnitureTab;
    public Button FloorTab;
    public Button ShowHideItemButton;

    private Animator animator;

    void Start()
    {
        animator = transform.GetComponent<Animator>();

        decorateBtn.onClick.AddListener(() =>
        {
            EnterDecorateMode();
        });

        exitBtn.onClick.AddListener(() =>
        {
            EnterViewMode();
        });

        FurnitureTab.onClick.AddListener(() =>
        {
            toggleFurniture();
        });

        FloorTab.onClick.AddListener(() =>
        {
            toggleFloor();
        });

        ShowHideItemButton.onClick.AddListener(() =>
        {
            ItemManager.instance.ShowHideAllFurniture();
        });

    }

    private void toggleFurniture()
    {
        ItemManager.instance.CancelPlacement();

        FloorViewport.SetActive(false);
        FurnitureViewport.SetActive(true);

        ShowHideItemButton.gameObject.SetActive(false);

        ItemManager.instance.ShowHideAllFurniture(true);

    }

    private void toggleFloor()
    {
        ItemManager.instance.CancelPlacement();

        FloorViewport.SetActive(true);
        FurnitureViewport.SetActive(false);

        ShowHideItemButton.gameObject.SetActive(true);

        ItemManager.instance.PlacementContainer.gameObject.SetActive(false);
    }

    public void EnterDecorateMode()
    {
        DecorateMark.SetActive(true);
        animator.Play("showLeftBar");
        ItemManager.instance.CurrentGameMode = ItemManager.GameModeCode.DecorateFurniture;

        toggleFurniture();
    }

    public void EnterViewMode()
    {
        DecorateMark.SetActive(false);
        animator.Play("hideLeftBar");
        ItemManager.instance.CurrentGameMode = ItemManager.GameModeCode.View;

        ItemManager.instance.ShowHideAllFurniture(true);
    }

}
