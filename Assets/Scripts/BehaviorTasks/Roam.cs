using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

public class Roam : BehaviorDesigner.Runtime.Tasks.Movement.Seek
{
    public SharedGameObject Self;

    public override void OnStart()
    {
        if (Self != null)
        {
            GPUSkinningPlayerMono mono = Self.Value.GetComponent<GPUSkinningPlayerMono>();
            if (mono != null)
            {
                GPUSkinningPlayer player = mono.Player;
                if (player!=null)
                {
                    player.CrossFade("Walk",0.2f);
                }
            }
        }
        
        base.OnStart();
    }

}
