using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CitizenManager : MonoBehaviour
{
    public GameObject PasserbyPoints;

    [Header("Controls")]
    public int PeopleWandering = 20;

    [Header("Characters")]
    public GameObject Josh;
    public GameObject Malcolm;
    public GameObject Megan;
    public GameObject Remy;

    private GameObject[] WanderingPeopleArr;
    private List<Transform> points;
    // Start is called before the first frame update
    void Start()
    {
        points = PasserbyPoints.GetComponentsInChildren<Transform>().ToList();
        points.RemoveAt(0);
        if (points.Count > 0)
        {
            WanderingPeopleArr = new GameObject[PeopleWandering];

            System.Random random = new System.Random();

            CreateWanderingPeople(random);
            StartCoroutine(WaitAndAssignDestinations(random));
        }
    }

    public void CreateWanderingPeople(System.Random random)
    {
        for (int i = 0; i < PeopleWandering; i++)
        {
            int CharacterRandom = random.Next(0, 4);
            int StartingPosition = random.Next(0, points.Count);

            switch (CharacterRandom)
            {
                case 0:
                    WanderingPeopleArr[i] = Instantiate(Josh, points.ElementAt(StartingPosition).position, Quaternion.identity);
                    break;
                case 1:
                    WanderingPeopleArr[i] = Instantiate(Malcolm, points.ElementAt(StartingPosition).position, Quaternion.identity);
                    break;
                case 2:
                    WanderingPeopleArr[i] = Instantiate(Megan, points.ElementAt(StartingPosition).position, Quaternion.identity);
                    break;
                case 3:
                    WanderingPeopleArr[i] = Instantiate(Remy, points.ElementAt(StartingPosition).position, Quaternion.identity);
                    break;
                default:
                    break;
            }
        }
    }

    IEnumerator WaitAndAssignDestinations(System.Random random)
    {
        yield return new WaitForSeconds(1f); //Count is the amount of time in seconds that you want to wait.

        for (int i = 0; i < WanderingPeopleArr.Length; i++)
        {
            GPUSkinningPlayerMono mono = WanderingPeopleArr[i].GetComponent<GPUSkinningPlayerMono>();
            if (mono != null)
            {
                int EndPosition = 0;
                do
                {
                    EndPosition = random.Next(0, points.Count);
                } while (points[EndPosition].position == WanderingPeopleArr[i].transform.position);

                StartCoroutine(AssignDestinations(random, mono, EndPosition));               
            }
        }

        yield return null;
    }
    IEnumerator AssignDestinations(System.Random random, GPUSkinningPlayerMono mono, int endPosition)
    {
        GPUSkinningPlayer player = mono.Player;

        if (player != null)
        {
            player.Play("Idle");
            yield return new WaitForSeconds(random.Next(0, 3));
            player.Play("Walk");
            NavMeshAgent nma = mono.gameObject.GetComponent<NavMeshAgent>();
            if (nma != null)
            {
                nma.speed = random.Next(35, 65) / 100f;
                nma.SetDestination(points[endPosition].position);
            }
        }

        yield return null;
    }
}
