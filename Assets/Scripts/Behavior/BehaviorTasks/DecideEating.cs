using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using System.Collections.Generic;
using System.Linq;

public class DecideEating : Conditional
{
    public SharedGameObject PossibleDestinations;
    public SharedGameObject ChairSpot;
    public SharedGameObject DeskSpot;
    public SharedGameObject DestinationSpot;

    public override TaskStatus OnUpdate()
    {
        System.Random random = new System.Random();

        //generating leaving point
        GameObject go = PossibleDestinations.Value;
        List<Transform> points = go.GetComponentsInChildren<Transform>().ToList();
        points.RemoveAt(0);
        int DestinationPosition = random.Next(0, points.Count);
        DestinationSpot.Value = points.ElementAt(DestinationPosition).gameObject;

        //generating the seat
        int seatCount = GameDataManager.Instance.AvailableChairList.Count;
        int chairIndex = random.Next(0, seatCount);
        //UnityEngine.Debug.Log("chairIndex: " + chairIndex);
        if (seatCount > 0)
        {
            Chair chair = GameDataManager.Instance.AvailableChairList.ElementAt(chairIndex);
            int chairSpotIndex = random.Next(0, chair.Spots.Length);
            ChairSpot.Value = chair.Spots[chairSpotIndex].gameObject;
            int deskSpotIndex = random.Next(0, chair.Desk.Spots.Length);
            DeskSpot.Value = chair.Desk.Spots[deskSpotIndex].gameObject;
            CitizenManager.instance.RemoveFromAvailableChairList(chair);
        }
        else
        {
            UnityEngine.Debug.LogError("No Available seat!!! Cannot do anyting!!!");
        }
        //CitizenManager.instance.UpdateAiAvailableChairList();


        return TaskStatus.Success;
    }
}

