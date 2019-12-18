using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIItemClickable : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        ControlManager.instance.leftBarController.GetComponentInChildren<ScrollRect>().enabled = true;
        ControlManager.instance.TargetUIItemIndex = transform.GetComponent<Item>().index;
    }

}
