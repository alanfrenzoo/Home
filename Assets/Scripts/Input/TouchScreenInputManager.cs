using UnityEngine;
using UnityEngine.EventSystems;

public class TouchScreenInputManager : InputManager
{
    private enum FingerMode
    {
        none,
        one,
        two
    }

    public float groundZ = 0;
    private FingerMode finger = FingerMode.none;
    private Vector3 touchStart, direction;
    private Vector2 TouchStart;
    private Vector2Int screen;

    // EVENTS
    public static event MoveInputHandler OnMoveInput;
    public static event RotateInputHandler OnRotateInput;
    public static event ZoomInputHandler OnZoomInput;

    private void Awake()
    {
        screen = new Vector2Int(Screen.width, Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        if (ItemManager.instance.IsEditing)
            return;

        // Over UI
        if (EventSystem.current && (EventSystem.current.IsPointerOverGameObject(0) || EventSystem.current.IsPointerOverGameObject(1) || EventSystem.current.IsPointerOverGameObject()))
            return;



        if (finger == FingerMode.two)
        {
            if (Input.touchCount == 0)
                finger = FingerMode.none;
            else if (Input.touchCount == 1)
                UpdateFirstClick();
        }

        if (Input.GetMouseButtonDown(0) && finger != FingerMode.two)
            UpdateFirstClick();

        if (Input.GetMouseButton(0) && finger != FingerMode.two && Input.touchCount < 2)
            UpdateFirstClickHold();

        if (Input.touchCount == 2)
        {
            finger = FingerMode.two;

            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Zooming
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrePos = touchOne.position - touchOne.deltaPosition;

            float preMagnitude = (touchZeroPrevPos - touchOnePrePos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - preMagnitude;

            // Rotating
            Vector2 prevLine = touchZeroPrevPos - touchOnePrePos;
            Vector2 currentLine = touchZero.position - touchOne.position;

            var prevSlope = prevLine.y / prevLine.x;
            var currentSlope = currentLine.y / currentLine.x;

            float angle = currentSlope > prevSlope ? Vector2.Angle(prevLine, currentLine) : -Vector2.Angle(prevLine, currentLine);
            
            OnZoomInput?.Invoke(-difference);
            OnRotateInput?.Invoke(angle);

        }
    }

    private void UpdateFirstClick()
    {
        finger = FingerMode.one;

        Touch touch = Input.GetTouch(0);
        TouchStart = touch.position;
        /*if (Camera.main.orthographic)
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        else
            touchStart = GetWorldPosition(groundZ);*/

    }

    private void UpdateFirstClickHold()
    {
        finger = FingerMode.one;

        /*if (Camera.main.orthographic)
            direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        else
            direction = touchStart - GetWorldPosition(groundZ);*/
        direction = Vector3.zero;
        Touch touch = Input.GetTouch(0);
        if (touch.position.y > TouchStart.y)
            direction -= Vector3.forward;
        else if (touch.position.y < TouchStart.y)
            direction += Vector3.forward;

        if (touch.position.x > TouchStart.x)
            direction -= Vector3.right;
        else if (touch.position.x < TouchStart.x)
            direction += Vector3.right;

        OnMoveInput?.Invoke(direction);
    }

    /*private Vector3 GetWorldPosition(float z)
    {
        Ray mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, new Vector3(0, 0, z));
        float distance;
        ground.Raycast(mousePos, out distance);

        return mousePos.GetPoint(distance);
    }*/

}
