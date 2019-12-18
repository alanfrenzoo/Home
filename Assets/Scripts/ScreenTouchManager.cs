using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Part;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenTouchManager : MonoBehaviour
{
    public static ScreenTouchManager instance;

    public float onItemLongPressDuration = 1f; //ViewMode Lift Up
    public float onItemShortPressDuration = 0.1f; //DecorateMode Lift Up
    public float onItemPressDuration = 0.1f; //DecoretaMode Canel

    private enum FingerMode
    {
        none,
        one,
        two
    }

    private FingerMode finger;
    private float startTimer;
    private float pressDuration;

    private void Start()
    {
        instance = this;
        finger = FingerMode.none;
        startTimer = float.PositiveInfinity;
        pressDuration = 0f;
    }

    private Vector3 startPos;

    private void Update()
    {

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            startTimer = Time.time;
            startPos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            // Not Stationary
            if (startPos != Input.mousePosition)
                ResetTimer();
            else
                pressDuration = Time.time - startTimer;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ResetTimer();
            CancelInteract();
        }
        return;
#endif

        if (finger == FingerMode.none)
        {
            if (Input.touchCount == 1)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    startTimer = Time.time;
                }
            }
            else if (Input.touchCount == 2)
            {

            }
        }
        else if (finger == FingerMode.one)
        {
            if (Input.touchCount == 1)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Stationary)
                {
                    pressDuration = Time.time - startTimer;
                }
                else
                {
                    ResetTimer();
                }
            }
            else if (Input.touchCount == 2)
            {
                ResetTimer();
            }
        }
        else if (finger == FingerMode.two)
        {
            if (Input.touchCount == 1)
            {

            }
            else if (Input.touchCount == 2)
            {

            }
        }

        // final update
        if (Input.touchCount == 0)
        {
            finger = FingerMode.none;
            ResetTimer();
            CancelInteract();
        }
        else if (Input.touchCount == 1)
        {
            finger = FingerMode.one;
        }
        else if (Input.touchCount == 2)
        {
            finger = FingerMode.two;
            ResetTimer();
        }

    }

    public bool CheckInputOnStationary()
    {
#if UNITY_EDITOR
        if (startPos == Input.mousePosition)
            return true;
        else
            return false;
#endif

        if (Input.GetTouch(0).phase == TouchPhase.Stationary)
            return true;
        else
            return false;

    }

    private void ResetTimer()
    {
        startTimer = float.PositiveInfinity;
        pressDuration = float.NegativeInfinity;

        ControlManager.instance.DisableLoadingBar();
    }

    private void CancelInteract()
    {
        GameManager.instance.NewCollider = null;
        GameManager.instance.TargetEditingItem = null;
    }

    public float GetLongPressTimePortion()
    {
        return (pressDuration > onItemPressDuration ? pressDuration - onItemPressDuration : pressDuration) / onItemLongPressDuration;
    }

    public float GetShortPressTimePortion()
    {
        return (pressDuration > onItemPressDuration ? pressDuration - onItemPressDuration : pressDuration) / onItemShortPressDuration;
    }

    public float GetPressTimePortion()
    {
        return pressDuration / onItemPressDuration;
    }

    public bool CheckOnItemPress()
    {
        if ((CheckOnFurnitureTouch() || GameManager.instance.NewCollider) && pressDuration >= 0f)
            return true;
        else
            return false;
    }

    public bool CheckOnEditingItemPress()
    {
        if (CheckOnEditingItemTouch() || GameManager.instance.TargetEditingItem)
            return true;
        else
            return false;
    }

    private bool CheckOnFurnitureTouch()
    {
        RaycastHit Hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition + new Vector3(0, 0, BuilderBehaviour.Instance.RaycastOffset));
        float Distance = BuilderBehaviour.Instance.OutOfRangeDistance == 0 ? BuilderBehaviour.Instance.ActionDistance : BuilderBehaviour.Instance.OutOfRangeDistance;

        if (Physics.Raycast(ray, out Hit, Distance, LayerMask.GetMask("Furniture")))
        {
            if (Hit.collider != null)
            {
                if (Input.GetMouseButtonDown(0))
                    GameManager.instance.NewCollider = Hit.collider.gameObject;
                return true;
            }

        }
        return false;
    }

    private bool CheckOnEditingItemTouch()
    {
        RaycastHit Hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition + new Vector3(0, 0, BuilderBehaviour.Instance.RaycastOffset));
        float Distance = BuilderBehaviour.Instance.OutOfRangeDistance == 0 ? BuilderBehaviour.Instance.ActionDistance : BuilderBehaviour.Instance.OutOfRangeDistance;

        if (Physics.Raycast(ray, out Hit, Distance, LayerMask.GetMask("PartBehaviour")))
        {
            if (Hit.collider != null)
            {
                if (Input.GetMouseButtonDown(0))
                    GameManager.instance.TargetEditingItem = Hit.transform.parent.gameObject;
                return true;
            }

        }
        return false;
    }

    public void UpdateCamera()
    {

    }

}
