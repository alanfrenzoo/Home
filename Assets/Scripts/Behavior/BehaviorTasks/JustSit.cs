using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

public class JustSit : BehaviorDesigner.Runtime.Tasks.Wait
{
    public SharedGameObject Self;
    public SharedBool WaitForServe;

    public override void OnStart()
    {
        if (Self != null)
        {
            GPUSkinningPlayerMono mono = Self.Value.GetComponent<GPUSkinningPlayerMono>();
            if (mono != null)
            {
                GPUSkinningPlayer player = mono.Player;
                if (player != null)
                {
                    player.CrossFade("Stand", 0.2f);
                }
            }
        }

        base.OnStart();
    }

}
