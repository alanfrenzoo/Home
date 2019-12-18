using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanZoomManager : MonoBehaviour
{
    public float zoomOutMin = 30;
    public float zoomOutMax = 80;
    public float zoomSpeed = .5f;
    public float groundZ = 0;
    public float moveSpeed = 0.5f;

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

        if (GameManager.instance.TargetEditingItem != null)
            return;

        // Over UI
        if (EventSystem.current && (EventSystem.current.IsPointerOverGameObject(0) || EventSystem.current.IsPointerOverGameObject(1) || EventSystem.current.IsPointerOverGameObject()))
            return;


#if UNITY_EDITOR

        if (Input.GetMouseButtonDown(0))
            GetCameraWorldPos();

        if (Input.GetMouseButton(0))
            UpdateCameraMove();

        UpdateCameraZoom(Input.GetAxis("Mouse ScrollWheel") * 5f);

        if (Input.GetKey(KeyCode.R))
            UpdateCameraRotate(10f);

        return;
#endif

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
                UpdateCameraZoom(difference * zoomSpeed);
            }
            else
            {
                UpdateCameraZoom(difference * zoomSpeed);
                UpdateCameraRotate(angle);
            }
        }
        if (finger == FingerMode.none || finger == FingerMode.two)
        {
            if (Input.touchCount == 1)
                GetCameraWorldPos();
        }
        else if (finger == FingerMode.one)
        {
            if (Input.touchCount == 1)
                UpdateCameraMove();
        }
        if (Input.touchCount == 0)
            finger = FingerMode.none;

    }

    private void GetCameraWorldPos()
    {
        finger = FingerMode.one;

        if (Camera.main.orthographic)
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        else
            touchStart = GetWorldPosition(groundZ);
    }

    private void UpdateCameraMove()
    {
        finger = FingerMode.one;

        if (Camera.main.orthographic)
            direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        else
            direction = touchStart - GetWorldPosition(groundZ);

        Camera.main.transform.parent.position += moveSpeed * direction;

        var p = Camera.main.transform.parent.position;
        if (p.x > 80)
            Camera.main.transform.parent.position = new Vector3(80, p.y, p.z);
        else if (p.x < -80)
            Camera.main.transform.parent.position = new Vector3(-80, p.y, p.z);

        p = Camera.main.transform.parent.position;
        if (p.z > 45)
            Camera.main.transform.parent.position = new Vector3(p.x, p.y, 45);
        else if (p.z < -45)
            Camera.main.transform.parent.position = new Vector3(p.x, p.y, -45);
    }

    private Vector3 GetWorldPosition(float z)
    {
        Ray mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, new Vector3(0, 0, z));
        float distance;
        ground.Raycast(mousePos, out distance);

        return mousePos.GetPoint(distance);
    }

    private void UpdateCameraZoom(float increment)
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

    private void UpdateCameraRotate(float angle)
    {
        var angleFinal = Camera.main.transform.parent.localEulerAngles.y + angle;
        var rot = Quaternion.Euler(0f, angleFinal, 0f);
        Camera.main.transform.parent.rotation = rot;
    }

}
