using System;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Events;
using UnityEngine;

public class Demo_Basic_Interaction : MonoBehaviour
{
    #region Public Events

    public delegate void EventHandler(GameObject obj);

    public static event EventHandler OnInteracted;

    public static void Interacted(GameObject obj)
    {
        if (OnInteracted != null)
            OnInteracted.Invoke(obj);
    }

    #endregion

    #region Public Fields

    [Header("Interaction Settings")]
    public float InteractionDistance = 3.0f;

    public KeyCode InteractionKey = KeyCode.E;
    public GUIStyle Font;
    public LayerMask Layers;

    #endregion

    #region Private Fields

    private bool IsOver;

    #endregion

    #region Private Methods

    private Demo_Interactable LastInteractable;

    private void Start()
    {
        EventHandlers.OnDestroyedPart += OnDestroyedPart;
    }

    private void Update()
    {
        Ray Ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        RaycastHit Hit;

        if (Physics.Raycast(Ray, out Hit, InteractionDistance, Layers))
        {
            if (Hit.collider.GetComponentInParent<Demo_Interactable>() && Hit.collider.GetComponentInParent<Demo_Interactable>().enabled)
            {
                LastInteractable = Hit.collider.GetComponentInParent<Demo_Interactable>();

                Hit.collider.GetComponentInParent<Demo_Interactable>().Show(Hit.point);

                if (Input.GetKeyUp(InteractionKey))
                {
                    Interacted(Hit.collider.gameObject);

                    Hit.collider.GetComponentInParent<Demo_Interactable>().Interaction();

                    LastInteractable.Hide();
                }
            }
            else
                if (LastInteractable != null)
                    LastInteractable.Hide();
        }
        else
            if (LastInteractable != null)
                LastInteractable.Hide();
    }

    private void OnDestroyedPart(PartBehaviour part)
    {
        if (LastInteractable != null)
            LastInteractable.Hide();
    }

    #endregion
}