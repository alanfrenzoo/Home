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
    public int PeopleWandering = 20;

    [Header("Characters")]
    public GameObject Josh;
    public GameObject Malcolm;
    public GameObject Megan;
    public GameObject Remy;

    [Header("AI")]
    public ExternalBehavior WanderAI;

    private GameObject[] WanderingPeopleArr;
    private GameObject[] CustomerArr;
    private List<Transform> points;
    private ExternalBehavior[] WanderingAI;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        System.Random random = new System.Random();

        points = PasserbyPoints.GetComponentsInChildren<Transform>().ToList();
        points.RemoveAt(0);
        if (points.Count > 0)
        {
            WanderingPeopleArr = new GameObject[PeopleWandering];
            WanderingAI = new ExternalBehavior[PeopleWandering];
            for (int i = 0; i < WanderingAI.Length; i++)
            {
                WanderingAI[i] = UnityEngine.Object.Instantiate(WanderAI);
                WanderingAI[i].Init();
            }
            
            CreateWanderingPeople(random);
        }

        bool haveRegister = GameDataManager.Instance.RegisterDict.Count > 0;
        if (haveRegister)
        {
            Register reg = GameDataManager.Instance.RegisterDict.ElementAt(0).Value;
            GameObject staff = Instantiate(Josh, reg.Spots.ElementAt(0), Quaternion.identity);
            BehaviorTree bt = staff.GetComponent<BehaviorTree>();
            if (bt != null)
            {
            }
        }

        int DeskCount = GameDataManager.Instance.DeskDict.Count;
        bool haveDesk = DeskCount > 0;
        bool producing = false;
        if (haveDesk)
        {
            UnityEngine.Debug.Log("haveDesk");
            Dictionary<Vector3, Desk> deskDict = GameDataManager.Instance.DeskDict;

            for (int i = 0; i < DeskCount; i++)
            {
                Desk desk = deskDict.ElementAt(i).Value;
                if (desk.Chairs != null)
                {
                    for (int j = 0; j < desk.Chairs.Length; j++)
                    {
                        GameDataManager.Instance.AvailableChairList.Add(desk.Chairs[j]);
                    }
                }
            }

            CustomerArr = new GameObject[GameDataManager.Instance.AvailableChairList.Count*7/10];

            int waitForServe = random.Next(1, GameDataManager.Instance.AvailableChairList.Count);
            for (int i = 0; i < waitForServe; i++)
            {
                int chariIndex = random.Next(0, GameDataManager.Instance.AvailableChairList.Count);
                CustomerArr[i] = Instantiate(Malcolm, GameDataManager.Instance.AvailableChairList.ElementAt(chariIndex).Spots.ElementAt(0), Quaternion.identity);
                Chair chair = GameDataManager.Instance.AvailableChairList.ElementAt(chariIndex);
                GameDataManager.Instance.AvailableChairList.RemoveAt(chariIndex);
                GameDataManager.Instance.UnavailableChairList.Add(chair);
                if (producing)
                {
                }
                else
                {

                };
            }

        }
        else
        {
            UnityEngine.Debug.Log("noDesk");
        }
    }

    public void CreateWanderingPeople(System.Random random)
    {
        for (int i = 0; i < PeopleWandering; i++)
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
                bt.ExternalBehavior = WanderingAI[i];
                bt.SetVariable("Self", (SharedGameObject)bt.gameObject);
                bt.SetVariable("PossibleTargets", (SharedGameObject)PasserbyPoints);
                bt.SetVariable("Speed", (SharedFloat)(random.Next(10, 31) / 10f));
            }
        }
    }
}
