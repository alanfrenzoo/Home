using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Demo_UI_AndroidControls : MonoBehaviour
{
    public GameObject BuildContent;
    public Button BuildButton;
    public Button DestructionButton;
    public Button ValidateButton;
    public Button CancelButton;
    public Button RotateButton;
    public Button RightButton;
    public Button LeftButton;

    private void Start()
    {
        BuildButton.onClick.AddListener(() => 
        {
            BuilderBehaviour.Instance.ChangeMode(BuildMode.Placement);

            BuildContent.SetActive(true);
        });

        DestructionButton.onClick.AddListener(() =>
        {
            BuilderBehaviour.Instance.ChangeMode(BuildMode.Destruction);

            BuildContent.SetActive(true);
        });

        ValidateButton.onClick.AddListener(() =>
        {
            if (BuilderBehaviour.Instance.CurrentMode == BuildMode.Placement)
                BuilderBehaviour.Instance.PlacePrefab();
            else if (BuilderBehaviour.Instance.CurrentMode == BuildMode.Destruction)
                BuilderBehaviour.Instance.RemovePrefab();
            else if (BuilderBehaviour.Instance.CurrentMode == BuildMode.Edition)
                BuilderBehaviour.Instance.EditPrefab();
        });

        CancelButton.onClick.AddListener(() =>
        {
            BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
        });

        RotateButton.onClick.AddListener(() =>
        {
            if (BuilderBehaviour.Instance.SelectedPrefab != null)
                BuilderBehaviour.Instance.RotatePreview(BuilderBehaviour.Instance.SelectedPrefab.RotationAxis);
        });

        RightButton.onClick.AddListener(() =>
        {
            if (BuilderBehaviour.Instance.SelectedIndex < BuildManager.Instance.PartsCollection.Parts.Count - 1)
                BuilderBehaviour.Instance.SelectedIndex++;
            else
                BuilderBehaviour.Instance.SelectedIndex = 0;

            BuilderBehaviour.Instance.SelectPrefab(BuildManager.Instance.PartsCollection.Parts[BuilderBehaviour.Instance.SelectedIndex]);
        });

        LeftButton.onClick.AddListener(() => 
        {
            if (BuilderBehaviour.Instance.SelectedIndex > 0)
                BuilderBehaviour.Instance.SelectedIndex--;
            else
                BuilderBehaviour.Instance.SelectedIndex = BuildManager.Instance.PartsCollection.Parts.Count - 1;

            BuilderBehaviour.Instance.SelectPrefab(BuildManager.Instance.PartsCollection.Parts[BuilderBehaviour.Instance.SelectedIndex]);
        });
    }

    private void Update()
    {
        if (BuilderBehaviour.Instance != null)
        {
            RotateButton.gameObject.SetActive(BuilderBehaviour.Instance.CurrentMode == BuildMode.Placement);
            BuildContent.SetActive(BuilderBehaviour.Instance.CurrentMode != BuildMode.None);
        }
    }
}
