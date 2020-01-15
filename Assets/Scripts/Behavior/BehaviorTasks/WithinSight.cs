using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class WithinSight:Conditional
{
    //public float fieldOfViewAngle;
    public string targetTag;
    public SharedGameObject target;
    public SharedGameObject ServingPosition;
    public SharedGameObject Self;

    private GameObject[] possibleTargets;

    public override void OnStart()
    {
        GPUSkinningPlayerMono mono = Self.Value.GetComponent<GPUSkinningPlayerMono>();
        if (mono != null)
        {
            GPUSkinningPlayer player = mono.Player;
            if (player != null)
            {
                player.CrossFade("Idle", 0.2f);
            }
        }

        base.OnStart();
    }
    public override void OnAwake()
    {
        possibleTargets = GameObject.FindGameObjectsWithTag(targetTag);
        //possibleTargets = new GameObject[targets.Length];
        //for (int)
        base.OnAwake();
    }

    public override TaskStatus OnUpdate()
    {
        for (int i=0; i < possibleTargets.Length; i++)
        {
            BehaviorTree bt = possibleTargets[i].GetComponent<BehaviorTree>();
            if (bt!=null)
            {
                if ((bool)bt.GetVariable("WaitForServe").GetValue())
                {
                    target.Value = possibleTargets[i];
                    ServingPosition.Value = (GameObject)bt.GetVariable("DeskSpot").GetValue();
                    return TaskStatus.Success;
                }
            }
        }
        return TaskStatus.Running;
    }
}
