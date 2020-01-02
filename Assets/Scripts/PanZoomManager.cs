using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanZoomManager : MonoBehaviour
{
    public float zoomOutMin = 30;
    public float zoomOutMax = 80;
    public float orthoZoomMin = 1f;
    public float orthoZoomMax = 10f;
    public float zoomSpeed = 0.005f;
    public float groundZ = 0;
    public float moveSpeed = 0.5f;

    private Vector3 touchStart = Vector3.up, direction;

    private Vector2 touch1Pos;
    private Vector2 touch2Pos;
    private Vector3 touchStart1 = Vector3.up;
    private Vector3 touchStart2 = Vector3.up;

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

        if (ItemManager.instance.TargetEditingItem != null)
            return;

        // Over UI
        if (EventSystem.current && (EventSystem.current.IsPointerOverGameObject(0) || EventSystem.current.IsPointerOverGameObject(1) || EventSystem.current.IsPointerOverGameObject()))
            return;


#if UNITY_EDITOR

        if (Input.GetMouseButtonDown(0))
            GetCameraWorldPos();

        if (Input.GetMouseButton(0))
            UpdateCameraMove();

        if (Input.GetMouseButtonUp(0))
            touchStart = Vector3.up;

        UpdateCameraZoom(Input.GetAxis("Mouse ScrollWheel") * 5f);

        //if (Input.GetKey(KeyCode.R))
        //    UpdateCameraRotate(10f);

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
                UpdateCameraZoom(difference * 0.2f);// zoomSpeed);
                UpdateCameraRotate(angle);
            }

            if (touchStart1 == Vector3.zero && touchStart2 == Vector3.zero)
            {
                touch1Pos = touchZero.position;
                touch2Pos = touchOne.position;

                touchStart1 = GetWorldPosition(groundZ, true);
                touchStart2 = GetWorldPosition(groundZ, false, true);
            }
            if (touchStart1 != Vector3.zero && touchStart2 != Vector3.zero)
            {
                var direction1 = touchStart1 - GetWorldPosition(groundZ, true);
                var direction2 = touchStart2 - GetWorldPosition(groundZ, false, true);

                Camera.main.transform.parent.position += moveSpeed * (direction1 + direction2) / 2;

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

        //if (Camera.main.orthographic)
        //    touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //else
        touchStart = GetWorldPosition(groundZ);

        touchStart1 = Vector3.zero;
        touchStart2 = Vector3.zero;
    }

    private void UpdateCameraMove()
    {
        finger = FingerMode.one;

        //if (Camera.main.orthographic)
        //    direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //else

        if (touchStart == Vector3.up)
            direction = Vector3.zero;
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

    private Vector3 GetWorldPosition(float z, bool touch1 = false, bool touch2 = false)
    {
        var pos = Input.mousePosition;
        if (touch1)
            pos = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
        if (touch2)
            pos = new Vector3(Input.GetTouch(1).position.x, Input.GetTouch(1).position.y);

        Ray mousePos = Camera.main.ScreenPointToRay(pos);
        //Ray mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
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
            Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize, orthoZoomMin);
            Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize, orthoZoomMax);
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
