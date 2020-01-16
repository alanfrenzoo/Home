using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class ReturnSuccess : Conditional
{
    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}
