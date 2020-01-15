using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

public class DriveRandomly : Conditional
{
    //public SharedGameObject carHead;
    //public SharedGameObject carTail;
    public SharedGameObject target;

    public override TaskStatus OnUpdate()
    {
        NextPoint np = target.Value.GetComponent<NextPoint>();
        if (np != null)
        {
            int length = np.Points.Length;
            if (length > 0)
            {
                List<NextPoint> points = new List<NextPoint>();
                for (int i = 0; i < np.Points.Length; i++)
                {
                    points.Add(np.Points[i]);
                }

                System.Random random = new System.Random();
                int index = random.Next(0, points.Count);
                /*float headDistance = Vector3.Distance(carHead.Value.transform.position, points.ElementAt(index).transform.position);
                float tailDistance = Vector3.Distance(carTail.Value.transform.position, points.ElementAt(index).transform.position);
                if (headDistance > tailDistance)
                {
                    points.RemoveAt(index);
                    index = random.Next(0, points.Count);
                }
                UnityEngine.Debug.Log(np.name+" "+index);*/
                target.SetValue(points.ElementAt(index).gameObject);
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Failure;
            }
        }
        return TaskStatus.Failure;
    }

}
