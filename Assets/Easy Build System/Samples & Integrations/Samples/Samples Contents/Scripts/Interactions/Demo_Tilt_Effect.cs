using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_Tilt_Effect : MonoBehaviour
{
    #region Public Fields

    public bool EnableTiltEffect;

    public float Amount = 0.02f;
    public float MaxAmount = 0.04f;

    public bool EnableTiltSideEffect;

    public float Smooth = 5;
    public float TiltAngle = 5;

    #endregion Public Fields

    #region Private Fields

    private Vector3 LocalPosition;

    #endregion Private Fields

    #region Private Methods

    private void Awake()
    {
        LocalPosition = transform.localPosition;
    }

    private void FixedUpdate()
    {
        if (Cursor.lockState == CursorLockMode.None)
            return;

        if (EnableTiltEffect)
        {
            float Horizontal = -Input.GetAxis("Mouse X") * Amount;
            float Vertical = -Input.GetAxis("Mouse Y") * Amount;

            if (Horizontal > MaxAmount)
                Horizontal = MaxAmount;

            if (Horizontal < -MaxAmount)
                Horizontal = -MaxAmount;

            if (Vertical > MaxAmount)
                Vertical = MaxAmount;

            if (Vertical < -MaxAmount)
                Vertical = -MaxAmount;

            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(LocalPosition.x + Horizontal, LocalPosition.y + Vertical, LocalPosition.z), Time.deltaTime * Smooth);
        }

        if (EnableTiltSideEffect)
        {
            float Horizontal = -Input.GetAxis("Horizontal") * TiltAngle;

            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, 0, Horizontal), Time.deltaTime * Smooth);
        }
    }

    #endregion Private Methods
}
