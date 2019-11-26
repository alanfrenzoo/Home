using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    public GameObject[] Prefab;
    public int CountX = 0;
    public int CountY = 0;

    public Button Add10btn;
    public Button Add100btn;
    public Text Number;
    public Button Colliderbtn;
    public Button Resetbtn;

    private EntityManager entityManager;
    private static int Total;
    private static bool isInstantiatingCollider = true;
    private List<GameObject> colliderList;
    private Dictionary<int, Entity> indexEntityPair;

    void Start()
    {
        entityManager = World.Active.EntityManager;
        colliderList = new List<GameObject>();
        indexEntityPair = new Dictionary<int, Entity>();

        Add10btn.onClick.AddListener(() =>
        {
            Total += 100;
            OnRun(10, 10);
            Number.text = String.Format("Total Entities: {0}", Total);
        });

        Add100btn.onClick.AddListener(() =>
        {
            Total += 1000;
            OnRun(100, 10);
            Number.text = String.Format("Total Entities: {0}", Total);
        });

        Colliderbtn.onClick.AddListener(() =>
        {
            isInstantiatingCollider = !isInstantiatingCollider;
            Colliderbtn.GetComponentInChildren<Text>().text = isInstantiatingCollider ? "ON" : "OFF";
        });

        Resetbtn.onClick.AddListener(() =>
        {
            foreach(var e in entityManager.GetAllEntities())
            {
                entityManager.DestroyEntity(e);
            }
            foreach (var c in colliderList)
            {
                Destroy(c);
            }
            
            colliderList.Clear();
            indexEntityPair.Clear();

            Total = 0;
            Number.text = String.Format("Total Entities: {0}", Total);

        });

        Total = CountX * CountY;
        OnRun(CountX, CountY);
        Number.text = String.Format("Total Entities: {0}", Total);
        Colliderbtn.GetComponentInChildren<Text>().text = isInstantiatingCollider ? "ON" : "OFF";
    }

    private void Update()
    {
        foreach(var c in colliderList)
        {
            var deltaTime = Time.deltaTime;
            c.transform.position = new Vector3(c.transform.position.x, c.transform.position.y, c.transform.position.z - deltaTime);
            if(c.transform.position.z < -50f)
                c.transform.position = new Vector3(c.transform.position.x, c.transform.position.y, 50f);
        }
    }

    private void OnRun(int countX, int countY)
    {

        for (int x = 0; x < countX; x++)
        {
            for (int y = 0; y < countY; y++)
            {
                var ran = UnityEngine.Random.Range(0, Prefab.Length - 1);

                Entity prefab;
                if (indexEntityPair.ContainsKey(ran))
                {
                    indexEntityPair.TryGetValue(ran, out prefab);
                }
                else
                {
                    // Create entity prefab from the game object hierarchy once
                    prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(Prefab[ran], World.Active);

                    indexEntityPair[ran] = prefab;
                }
                

                // Efficiently instantiate a bunch of entities from the already converted entity prefab
                var instance = entityManager.Instantiate(prefab);

                // Place the instantiated entity in a grid with some noise
                var position = transform.TransformPoint(new float3(x - CountX / 2, noise.cnoise(new float2(x, y) * 0.21F) * 10, y - CountY / 2));
                entityManager.SetComponentData(instance, new Translation() { Value = position });
                entityManager.AddComponentData(instance, new MoveDownTag());
                entityManager.AddComponentData(instance, new MovingTag());

                if (isInstantiatingCollider)
                {
                    var colliderToSpawn = new GameObject("c");
                    colliderToSpawn.AddComponent<MeshCollider>();
                    colliderToSpawn.GetComponent<MeshCollider>().sharedMesh = Prefab[ran].GetComponent<MeshCollider>().sharedMesh;
                    colliderToSpawn.transform.position = position;

                    colliderList.Add(colliderToSpawn);
                }
                
            }
        }
    }
}