using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LogSlot
{
    public GameObject Object;

    [HideInInspector]
    public bool IsAvailable;

    public void Show()
    {
        Object.gameObject.SetActive(true);
    }

    public void Hide()
    {
        Object.gameObject.SetActive(false);
    }
}

public class PickableController : MonoBehaviour
{
    public static PickableController Instance;

    public List<LogSlot> Slots = new List<LogSlot>();

    public GameObject DroppableObject;

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < Slots.Count; i++)
            Slots[i].Hide();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            for (int i = Slots.Count-1; i >= 0; i--)
            {
                if (Slots[i].IsAvailable)
                {
                    Slots[i].IsAvailable = false;

                    Slots[i].Hide();

                    Rigidbody Log = Instantiate(DroppableObject, transform.position + transform.forward * 2f, transform.rotation).GetComponent<Rigidbody>();

                    Log.AddForce(transform.forward * 100f, ForceMode.Impulse);

                    break;
                }
            }
        }
    }

    public void Pick(GameObject obj)
    {
        for (int i = 0; i < Slots.Count; i++)
        {
            if (!Slots[i].IsAvailable)
            {
                Slots[i].IsAvailable = true;

                Slots[i].Show();

                Destroy(obj);

                break;
            }
        }
    }

    public int GetCurrentLogCount()
    {
        int count = 0;

        for (int i = 0; i < Slots.Count; i++)
            if (Slots[i].IsAvailable)
                count++;

        return count;
    }
}