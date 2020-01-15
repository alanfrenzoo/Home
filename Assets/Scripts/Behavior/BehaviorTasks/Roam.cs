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

    // Seek the destination. Return success once the agent has reached the destination.
    // Return running if the agent hasn't reached the destination yet
    public override TaskStatus OnUpdate()
    {
        if (HasArrived())
        {
            return TaskStatus.Success;
        }

        SetDestination(Target());
        return TaskStatus.Running;
    }

    // Return targetPosition if target is null
    private Vector3 Target()
    {
        if (target.Value != null)
        {
            return target.Value.transform.position;
        }
        return targetPosition.Value;
    }
}
