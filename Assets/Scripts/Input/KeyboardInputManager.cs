using UnityEngine;

public class KeyboardInputManager : InputManager
{
    // EVENTS
    public static event MoveInputHandler OnMoveInput;
    public static event RotateInputHandler OnRotateInput;
    public static event ZoomInputHandler OnZoomInput;

    // Update is called once per frame
    void Update()
    {
        // movement
        if (Input.GetKey(KeyCode.W))
        {
            OnMoveInput?.Invoke(Vector3.forward);
        }
        if (Input.GetKey(KeyCode.S))
        {
            OnMoveInput?.Invoke(-Vector3.forward);
        }
        if (Input.GetKey(KeyCode.A))
        {
            OnMoveInput?.Invoke(-Vector3.right);
        }
        if (Input.GetKey(KeyCode.D))
        {
            OnMoveInput?.Invoke(Vector3.right);
        }

        // roration
        if (Input.GetKey(KeyCode.Q))
        {
            OnRotateInput?.Invoke(1f);
        }
        if (Input.GetKey(KeyCode.E))
        {
            OnRotateInput?.Invoke(-1f);
        }

        //zoom
        if (Input.GetKey(KeyCode.X))
        {
            OnZoomInput?.Invoke(1f);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            OnZoomInput?.Invoke(-1f);
        }
    }
}
