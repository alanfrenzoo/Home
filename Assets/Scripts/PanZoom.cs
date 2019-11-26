using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanZoom : MonoBehaviour
{
    public float zoomOutMin = 1;
    public float zoomOutMax = 8;
    public float perspectiveZoomSpeed = .5f;
    public float orthoZoomSpeed = .5f;
    public float groundZ = 0;

    private Vector3 touchStart, direction;

    private enum FingerMode
    {
        none,
        one,
        two
    }

    private FingerMode finger = FingerMode.none;

    // Update is called once per frame
    void Update()
    {
        if (Test_Control.instance.Editing)
            return;

        // Over UI
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(0) || UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(1) || UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

#if UNITY_EDITOR

        zoom(Input.GetAxis("Mouse ScrollWheel") * 5f);

        if (Input.GetKey(KeyCode.R))
            rotate(25f);
#endif

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


            if (Camera.main.orthographic)
            {
                zoom(difference * orthoZoomSpeed);
            }
            else
            {
                zoom(difference * perspectiveZoomSpeed);
                rotate(angle);
            }
        }

    }

    private void UpdateFirstClick()
    {
        finger = FingerMode.one;

        if (Camera.main.orthographic)
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        else
            touchStart = GetWorldPosition(groundZ);
    }

    private void UpdateFirstClickHold()
    {
        finger = FingerMode.one;

        if (Camera.main.orthographic)
            direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        else
            direction = touchStart - GetWorldPosition(groundZ);

        Camera.main.transform.parent.position += direction;
    }

    private Vector3 GetWorldPosition(float z)
    {
        Ray mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, new Vector3(0, 0, z));
        float distance;
        ground.Raycast(mousePos, out distance);

        return mousePos.GetPoint(distance);
    }

    private void zoom(float increment)
    {
        if (Camera.main.orthographic)
        {
            Camera.main.orthographicSize -= increment;
            Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize, .1f);
        }
        else
        {
            Camera.main.fieldOfView -= increment;
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, zoomOutMin, zoomOutMax);
        }
    }

    private void rotate(float angle)
    {
        var angleFinal = Camera.main.transform.parent.localEulerAngles.y + angle;
        var rot = Quaternion.Euler(0f, angleFinal, 0f);
        Camera.main.transform.parent.rotation = rot;
    }
}
