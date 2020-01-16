using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using UnityEngine;
using UnityEngine.AI;

public class CitizenManager : MonoBehaviour
{
    public static CitizenManager instance;

    public GameObject PasserbyPoints;

    [Header("Controls")]
    public int PeopleWanderingSize = 20;
    public int StaffSize = 1;

    [Header("Characters")]
    public GameObject Josh;
    public GameObject Malcolm;
    public GameObject Megan;
    public GameObject Remy;

    [Header("AI")]
    public ExternalBehavior WanderAI;
    public ExternalBehavior StaffAI;
    public ExternalBehavior CustomerAI;

    private GameObject[] WanderingPeopleArr;
    private GameObject[] CustomerArr;
    private GameObject[] StaffArr;
    private List<Transform> points;
    private ExternalBehavior[] WanderingBehavior;
    private ExternalBehavior[] CustomerBehavior;
    private ExternalBehavior[] StaffBehavior;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        System.Random random = new System.Random();

        points = PasserbyPoints.GetComponentsInChildren<Transform>().ToList();
        points.RemoveAt(0);
        if (points.Count > 0)
        {
            CreateWanderingPeople(random);
        }

        CreateCustomer(random);

        bool haveRegister = GameDataManager.Instance.RegisterDict.Count > 0;
        if (haveRegister)
        {
            CreateStaff(random);
        }

    }

    public void CreateStaff(System.Random random)
    {
        Register reg = GameDataManager.Instance.RegisterDict.ElementAt(0).Value;
        StaffArr = new GameObject[StaffSize];
        StaffBehavior = new ExternalBehavior[StaffSize];
        for (int i = 0; i < StaffSize; i++)
        {
            StaffArr[i] = Instantiate(Josh, reg.Spots.ElementAt(0), Quaternion.identity);
            StaffBehavior[i] = UnityEngine.Object.Instantiate(StaffAI);
            StaffBehavior[i].Init();

            BehaviorTree bt = StaffArr[i].GetComponent<BehaviorTree>();
            if (bt != null)
            {
                bt.ExternalBehavior = StaffBehavior[i];
                bt.SetVariable("Self", (SharedGameObject)bt.gameObject);
                bt.SetVariable("RegisterLocation", (SharedVector3)reg.Spots.ElementAt(0));
                bt.SetVariable("Speed", (SharedFloat)(random.Next(10, 31) / 10f));
            }
        }
    }

    public void CreateCustomer(System.Random random)
    {
        int DeskCount = GameDataManager.Instance.DeskDict.Count;
        bool haveDesk = DeskCount > 0;
        bool producing = false;
        if (haveDesk)
        {
            //UnityEngine.Debug.Log("haveDesk");
            Dictionary<Vector3, Desk> deskDict = GameDataManager.Instance.DeskDict;

            for (int i = 0; i < DeskCount; i++)
            {
                Desk desk = deskDict.ElementAt(i).Value;
                if (desk.Chairs != null)
                {
                    //UnityEngine.Debug.Log("desk.Chairs.Length: " + desk.Chairs.Length);
                    for (int j = 0; j < desk.Chairs.Length; j++)
                    {
                        GameDataManager.Instance.AvailableChairList.Add(desk.Chairs[j]);
                    }
                }
            }

            CustomerArr = new GameObject[GameDataManager.Instance.AvailableChairList.Count * 7 / 10];
            CustomerBehavior = new ExternalBehavior[CustomerArr.Length];
            for (int i = 0; i < CustomerBehavior.Length; i++)
            {
                CustomerBehavior[i] = UnityEngine.Object.Instantiate(CustomerAI);
                CustomerBehavior[i].Init();
            }

            int waitForServe = random.Next(1, CustomerArr.Length);
            //UnityEngine.Debug.Log("CustomerArr.Length: " + CustomerArr.Length);
            //UnityEngine.Debug.Log("waitForServe: " + waitForServe);
            for (int i = 0; i < waitForServe; i++)
            {
                int chairIndex = random.Next(0, GameDataManager.Instance.AvailableChairList.Count);
                int destinationIndex = random.Next(0, points.Count);
                //UnityEngine.Debug.Log("i: " + i);
                //UnityEngine.Debug.Log("GameDataManager.Instance.AvailableChairList.Count: " + GameDataManager.Instance.AvailableChairList.Count);
                //UnityEngine.Debug.Log("chairIndex: " + chairIndex);
                CustomerArr[i] = Instantiate(Malcolm, GameDataManager.Instance.AvailableChairList.ElementAt(chairIndex).Spots.ElementAt(0).position, Quaternion.identity);

                BehaviorTree bt = CustomerArr[i].GetComponent<BehaviorTree>();
                if (bt != null)
                {
                    bt.ExternalBehavior = CustomerBehavior[i];
                    bt.SetVariable("Self", (SharedGameObject)bt.gameObject);
                    bt.SetVariable("IsInProduction", (SharedBool)true);
                    bt.SetVariable("Speed", (SharedFloat)2f);
                    bt.SetVariable("StartFromBegining", (SharedBool)false);
                    bt.SetVariable("WaitForServe", (SharedBool)producing);
                    bt.SetVariable("ChairSpot", (SharedGameObject)GameDataManager.Instance.AvailableChairList.ElementAt(chairIndex).Spots.ElementAt(0).gameObject);
                    bt.SetVariable("DeskSpot", (SharedGameObject)GameDataManager.Instance.AvailableChairList.ElementAt(chairIndex).Desk.Spots.ElementAt(0).gameObject);
                    bt.SetVariable("LeavingDestination", (SharedGameObject)points.ElementAt(destinationIndex).gameObject);
                    bt.SetVariable("PasserbyPoints", (SharedGameObject)PasserbyPoints);
                    bt.SetVariable("True", (SharedBool)true);
                    bt.SetVariable("False", (SharedBool)false);
                }

                Chair chair = GameDataManager.Instance.AvailableChairList.ElementAt(chairIndex);
                RemoveFromAvailableChairList(chair);
            }


            int CustomerWalkingToRestaurant = CustomerArr.Length - waitForServe;
            for (int i = 0; i < CustomerWalkingToRestaurant; i++)
            {
                int chairIndex = random.Next(0, GameDataManager.Instance.AvailableChairList.Count);

                int spawnIndex = random.Next(0, points.Count);
                int destinationIndex = random.Next(0, points.Count);

                CustomerArr[waitForServe + i] = Instantiate(Malcolm, points.ElementAt(spawnIndex).position, Quaternion.identity);
                BehaviorTree bt = CustomerArr[waitForServe + i].GetComponent<BehaviorTree>();
                if (bt != null)
                {
                    bt.ExternalBehavior = CustomerBehavior[waitForServe + i];
                    bt.SetVariable("Self", (SharedGameObject)bt.gameObject);
                    bt.SetVariable("IsInProduction", (SharedBool)true);
                    bt.SetVariable("Speed", (SharedFloat)2f);
                    bt.SetVariable("ChairSpot", (SharedGameObject)GameDataManager.Instance.AvailableChairList.ElementAt(chairIndex).Spots.ElementAt(0).gameObject);
                    bt.SetVariable("DeskSpot", (SharedGameObject)GameDataManager.Instance.AvailableChairList.ElementAt(chairIndex).Desk.Spots.ElementAt(0).gameObject);
                    bt.SetVariable("LeavingDestination", (SharedGameObject)points.ElementAt(destinationIndex).gameObject);
                    bt.SetVariable("StartFromBegining", (SharedBool)true);
                    bt.SetVariable("WaitForServe", (SharedBool)producing);
                    bt.SetVariable("PasserbyPoints", (SharedGameObject)PasserbyPoints);
                    bt.SetVariable("True", (SharedBool)true);
                    bt.SetVariable("False", (SharedBool)false);
                }
                Chair chair = GameDataManager.Instance.AvailableChairList.ElementAt(chairIndex);
                RemoveFromAvailableChairList(chair);
            }
        }
        else
        {
            UnityEngine.Debug.Log("noDesk");
        }
    }

    public void CreateWanderingPeople(System.Random random)
    {
        WanderingPeopleArr = new GameObject[PeopleWanderingSize];
        WanderingBehavior = new ExternalBehavior[PeopleWanderingSize];
        for (int i = 0; i < WanderingBehavior.Length; i++)
        {
            WanderingBehavior[i] = UnityEngine.Object.Instantiate(WanderAI);
            WanderingBehavior[i].Init();
        }

        for (int i = 0; i < PeopleWanderingSize; i++)
        {
            int CharacterRandom = random.Next(0, 2);
            int StartingPosition = random.Next(0, points.Count);

            switch (CharacterRandom)
            {
                case 0:
                    WanderingPeopleArr[i] = Instantiate(Remy, points.ElementAt(StartingPosition).position, Quaternion.identity);
                    break;
                case 1:
                    WanderingPeopleArr[i] = Instantiate(Megan, points.ElementAt(StartingPosition).position, Quaternion.identity);
                    break;
                default:
                    break;
            }

            BehaviorTree bt = WanderingPeopleArr[i].GetComponent<BehaviorTree>();
            if (bt != null)
            {
                bt.ExternalBehavior = WanderingBehavior[i];
                bt.SetVariable("Self", (SharedGameObject)bt.gameObject);
                bt.SetVariable("PossibleTargets", (SharedGameObject)PasserbyPoints);
                bt.SetVariable("Speed", (SharedFloat)(random.Next(10, 31) / 10f));
            }
        }
    }

    public void RemoveFromAvailableChairList(Chair chair)
    {
        GameDataManager.Instance.AvailableChairList.Remove(chair);
        GameDataManager.Instance.UnavailableChairList.Add(chair);
    }

    public void AddToAvailableChairList(Chair chair)
    {
        GameDataManager.Instance.AvailableChairList.Add(chair);
        GameDataManager.Instance.UnavailableChairList.Remove(chair);
    }
}
