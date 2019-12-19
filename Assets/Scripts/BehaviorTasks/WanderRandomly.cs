using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

public class WanderRandomly : Conditional
{
    //public SharedGameObject carHead;
    //public SharedGameObject carTail;
    public SharedGameObject PossibleTargets;
    public SharedGameObject Target;

    public override TaskStatus OnUpdate()
    {
        GameObject targetsVar = PossibleTargets.Value;
        if (targetsVar != null)
        {
            List<Transform> targetList = targetsVar.GetComponentsInChildren<Transform>().ToList();
            if (targetList.Count > 1)
            {
                targetList.RemoveAt(0);

                System.Random random = new System.Random();
                int index = random.Next(0, targetList.Count);
                /*float headDistance = Vector3.Distance(carHead.Value.transform.position, points.ElementAt(index).transform.position);
                float tailDistance = Vector3.Distance(carTail.Value.transform.position, points.ElementAt(index).transform.position);
                if (headDistance > tailDistance)
                {
                    points.RemoveAt(index);
                    index = random.Next(0, points.Count);
                }
                UnityEngine.Debug.Log(np.name+" "+index);*/
                Target.SetValue(targetList.ElementAt(index).gameObject);
                return TaskStatus.Success;

            }
            else
            {
                UnityEngine.Debug.Log("targetList.Count <= 1");
                return TaskStatus.Failure;
            }
        }
        UnityEngine.Debug.Log("targetsVar == null");
        return TaskStatus.Failure;
    }

}
