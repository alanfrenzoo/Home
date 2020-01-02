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
    private List<Transform> points;
    private ExternalBehavior[] WanderingAI;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;

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
            System.Random random = new System.Random();
            CreateWanderingPeople(random);
        }
        bool haveDesk = GameDataManager.Instance.DeskDict.Count > 0;
        bool producing = false;
        if (haveDesk)
        {
            UnityEngine.Debug.Log("haveDesk");
            int deskCount = GameDataManager.Instance.DeskDict.Count;

            if (producing)
            {
            }
            else
            {
            };

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
            int CharacterRandom = random.Next(0, 3);
            int StartingPosition = random.Next(0, points.Count);

            switch (CharacterRandom)
            {
                case 0:
                    WanderingPeopleArr[i] = Instantiate(Remy, points.ElementAt(StartingPosition).position, Quaternion.identity);
                    break;
                case 1:
                    WanderingPeopleArr[i] = Instantiate(Malcolm, points.ElementAt(StartingPosition).position, Quaternion.identity);
                    break;
                case 2:
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
