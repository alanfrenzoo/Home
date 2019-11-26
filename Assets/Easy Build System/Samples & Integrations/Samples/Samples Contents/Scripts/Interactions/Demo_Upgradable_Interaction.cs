using EasyBuildSystem.Runtimes.Internal.Part;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_Upgradable_Interaction : Demo_Interactable
{
    public override string InteractionMessage
    {
        get
        {
            return string.Format("Press <b>F</b> To Place.\nProgression : {0}/{1}", GetComponent<AddonTheForestLike>().GetCurrentProgression(), GetComponent<AddonTheForestLike>().Elements.Length);
        }
    }

    public override void Show(Vector3 point)
    {
        if (GetComponent<AddonTheForestLike>() == null)
            return;

        if (GetComponent<AddonTheForestLike>().IsCompleted())
            return;

        Demo_UI_Tooltip.Instance.Show(point, InteractionMessage);
    }

    public override void Hide()
    {
        Demo_UI_Tooltip.Instance.Hide();
    }

    public override void Interaction()
    {
        for (int i = PickableController.Instance.Slots.Count-1; i >= 0; i--)
        {
            if (PickableController.Instance.Slots[i].IsAvailable)
            {
                if (!GetComponent<AddonTheForestLike>().IsCompleted())
                {
                    if (GetComponent<PartBehaviour>().CurrentState != StateType.Queue)
                        return;

                    PickableController.Instance.Slots[i].IsAvailable = false;

                    PickableController.Instance.Slots[i].Hide();

                    GetComponent<AddonTheForestLike>().Upgrade();

                    break;
                }
            }
        }
    }
}
