using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Demo_ThirdPersonController))]
public class Demo_ThirdPersonControls : MonoBehaviour
{
    #region Private Fields

    private Demo_ThirdPersonController Character;
    private Transform Camera;
    private Vector3 CameraForward;
    private Vector3 Direction;
    private bool Jump;

    #endregion Private Fields

    #region Private Methods

    private void Start()
    {
        if (UnityEngine.Camera.main != null)
        {
            Camera = UnityEngine.Camera.main.transform;
        }
        else
        {
            Debug.LogWarning("Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
        }

        Character = GetComponent<Demo_ThirdPersonController>();
    }

    private void Update()
    {
        if (!Jump)
        {
            Jump = CrossPlatformInputManager.GetButtonDown("Jump");
        }
    }

    private void FixedUpdate()
    {
        float h = CrossPlatformInputManager.GetAxis("Horizontal");

        float v = CrossPlatformInputManager.GetAxis("Vertical");

        bool crouch = Input.GetKey(KeyCode.C);

        if (Camera != null)
        {
            CameraForward = Vector3.Scale(Camera.forward, new Vector3(1, 0, 1)).normalized;
            Direction = v * CameraForward + h * Camera.right;
        }
        else
        {
            Direction = v * Vector3.forward + h * Vector3.right;
        }

        if (Input.GetKey(KeyCode.LeftShift)) Direction *= 0.5f;

        Character.Move(Direction, crouch, Jump);
        Jump = false;
    }

    #endregion Private Methods
}