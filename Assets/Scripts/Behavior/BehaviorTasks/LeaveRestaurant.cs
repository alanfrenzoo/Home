using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

public class LeaveRestaurant : BehaviorDesigner.Runtime.Tasks.Movement.Seek
{
    public SharedBool StartFromBegining;
    public SharedGameObject Self;
    public SharedGameObject ChairPosition;
    public SharedGameObject PasserbyPoints;

    public override void OnStart()
    {
        StartFromBegining.Value = true;

        if (Self != null)
        {
            GPUSkinningPlayerMono mono = Self.Value.GetComponent<GPUSkinningPlayerMono>();
            if (mono != null)
            {
                GPUSkinningPlayer player = mono.Player;
                if (player != null)
                {
                    player.CrossFade("Walk", 0.2f);
                }
            }
        }

        base.OnStart();

        for (int i = 0; i < GameDataManager.Instance.UnavailableChairList.Count; i++)
        {
            for (int j=0; j< GameDataManager.Instance.UnavailableChairList.ElementAt(i).Spots.Length;j++)
            {
                if (GameDataManager.Instance.UnavailableChairList.ElementAt(i).Spots.ElementAt(j).position.Equals(ChairPosition.Value.transform.position))
                {
                    
                    Chair chair = GameDataManager.Instance.UnavailableChairList.ElementAt(i);

                    CitizenManager.instance.AddToAvailableChairList(chair);
                    UnityEngine.Debug.Log("Add Back To AvailableChairList: "+GameDataManager.Instance.AvailableChairList.Count);
                    break;
                }
            }
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (HasArrived())
        {
            return TaskStatus.Success;
        }

        if (HasPath())
        {
            if (navMeshAgent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathPartial || navMeshAgent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
            {
                if (navMeshAgent.nextPosition == navMeshAgent.pathEndPosition)
                {
                    System.Random random = new System.Random();
                    GameObject go = PasserbyPoints.Value;
                    List<Transform> points = go.GetComponentsInChildren<Transform>().ToList();
                    points.RemoveAt(0);
                    int DestinationPosition = random.Next(0, points.Count);
                    targetPosition.Value = points.ElementAt(DestinationPosition).position;
                }
            }
        }
        SetDestination(Target());

        return TaskStatus.Running;
    }

    private Vector3 Target()
    {
        if (target.Value != null)
        {
            return target.Value.transform.position;
        }
        return targetPosition.Value;
    }
}
