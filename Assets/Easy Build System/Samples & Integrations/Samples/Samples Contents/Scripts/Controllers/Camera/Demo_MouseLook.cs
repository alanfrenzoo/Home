using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[Serializable]
public class Demo_MouseLook
{
    #region Public Fields
    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool ClampVerticalRotation = true;
    public float MinimumX = -90F;
    public float MaximumX = 90F;
    public bool Smooth;
    public float SmoothTime = 5f;
    public bool LockCursor = true;

    #endregion Public Fields

    #region Private Fields

    private Quaternion CharacterTargetRot;
    private Quaternion CameraTargetRot;

    #endregion Private Fields

    #region Public Methods

    public void Init(Transform character, Transform camera)
    {
        CharacterTargetRot = character.localRotation;
        CameraTargetRot = camera.localRotation;
    }

    public void LookRotation(Transform character, Transform camera)
    {
        if (!LockCursor) return;

        float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
        float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;

        CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);

        CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        if (ClampVerticalRotation)
            CameraTargetRot = ClampRotationAroundXAxis(CameraTargetRot);

        if (Smooth)
        {
            character.localRotation = Quaternion.Slerp(character.localRotation, CharacterTargetRot,
                SmoothTime * Time.deltaTime);
            camera.localRotation = Quaternion.Slerp(camera.localRotation, CameraTargetRot,
                SmoothTime * Time.deltaTime);
        }
        else
        {
            character.localRotation = CharacterTargetRot;
            camera.localRotation = CameraTargetRot;
        }

        UpdateCursorLock();
    }

    public void UpdateCursorLock()
    {
            InternalLockUpdate();
    }

    #endregion Public Methods

    #region Private Methods

    private void InternalLockUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetKeyUp(KeyCode.B))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    #endregion Private Methods
}