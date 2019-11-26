using EasyBuildSystem.Runtimes.Extensions;
using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Managers;
using UnityEngine;

[AddComponentMenu("Easy Build System/Features/Builders Behaviour/Desktop Builder Behaviour")]
public class DesktopBuilderBehaviour : BuilderBehaviour
{
    #region Public Fields

    public bool CreativeMode = true;

    public KeyCode PlacementModeKey = KeyCode.E;
    public KeyCode DestructionModeKey = KeyCode.R;
    public KeyCode EditionModeKey = KeyCode.T;

    public KeyCode ValidateModeKey = KeyCode.Mouse0;
    public KeyCode CancelModeKey = KeyCode.Mouse1;

    #endregion Public Fields

    #region Public Methods

    public override void UpdateModes()
    {
        base.UpdateModes();

        if (CreativeMode)
        {
            if (Input.GetKeyDown(PlacementModeKey))
                ChangeMode(BuildMode.Placement);

            if (Input.GetKeyDown(DestructionModeKey))
                ChangeMode(BuildMode.Destruction);

            if (Input.GetKeyDown(EditionModeKey))
                ChangeMode(BuildMode.Edition);

            if (CurrentMode != BuildMode.Placement)
                UpdatePrefabSelection();

            if (Input.GetKeyDown(CancelModeKey))
                ChangeMode(BuildMode.None);
        }

        if (CurrentMode == BuildMode.Placement)
        {
            if (UIExtension.IsCursorOverUserInterface())
                return;

            if (Input.GetKeyDown(ValidateModeKey))
                PlacePrefab();

            float WheelAxis = Input.GetAxis("Mouse ScrollWheel");

            if (WheelAxis > 0)
                RotatePreview(SelectedPrefab.RotationAxis);
            else if (WheelAxis < 0)
                RotatePreview(-SelectedPrefab.RotationAxis);

            if (Input.GetKeyDown(CancelModeKey))
                ChangeMode(BuildMode.None);
        }
        else if (CurrentMode == BuildMode.Edition)
        {
            if (UIExtension.IsCursorOverUserInterface())
                return;

            if (Input.GetKeyDown(ValidateModeKey))
                EditPrefab();

            if (Input.GetKeyDown(CancelModeKey))
                ChangeMode(BuildMode.None);
        }
        else if (CurrentMode == BuildMode.Destruction)
        {
            if (UIExtension.IsCursorOverUserInterface())
                return;

            if (Input.GetKeyDown(ValidateModeKey))
                RemovePrefab();

            if (Input.GetKeyDown(CancelModeKey))
                ChangeMode(BuildMode.None);
        }
    }

    #endregion Public Methods

    #region Private Methods

    private void UpdatePrefabSelection()
    {
        float WheelAxis = Input.GetAxis("Mouse ScrollWheel");

        if (WheelAxis > 0)
        {
            if (SelectedIndex < BuildManager.Instance.PartsCollection.Parts.Count - 1)
                SelectedIndex++;
            else
                SelectedIndex = 0;
        }
        else if (WheelAxis < 0)
        {
            if (SelectedIndex > 0)
                SelectedIndex--;
            else
                SelectedIndex = BuildManager.Instance.PartsCollection.Parts.Count - 1;
        }

        if (SelectedIndex == -1)
            return;

        SelectPrefab(BuildManager.Instance.PartsCollection.Parts[SelectedIndex]);
    }

    #endregion Private Methods
}