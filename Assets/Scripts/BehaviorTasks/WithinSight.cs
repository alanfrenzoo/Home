using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class WithinSight:Conditional
{
    public float fieldOfViewAngle;
    public string targetTag;
    public SharedTransform target;

    private Transform[] possibleTargets;

    public override void OnAwake()
    {
        var targets = GameObject.FindGameObjectsWithTag(targetTag);
        possibleTargets = new Transform[targets.Length];
        //for (int)
        base.OnAwake();
    }


}
