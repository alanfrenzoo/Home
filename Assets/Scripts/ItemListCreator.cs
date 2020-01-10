using EasyBuildSystem.Runtimes.Internal.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemListCreator : MonoBehaviour
{
    public GameObject prefab;
    public Transform furniContent;
    public Transform floorContent;

    public List<Sprite> furniSpriteList;
    public List<Sprite> floorSpriteList;

    void Start()
    {
        for (int i = 0; i < furniSpriteList.Count; i++)
        {
            var obj = Instantiate(prefab);
            obj.transform.SetParent(furniContent);

            obj.GetComponent<Item>().index = i;
            obj.GetComponent<Item>().sprite = furniSpriteList[i];
            obj.GetComponent<Image>().sprite = furniSpriteList[i];

        }

        for (int i = 0; i < floorSpriteList.Count; i++)
        {
            var obj = Instantiate(prefab);
            obj.transform.SetParent(floorContent);

            var index = i + furniSpriteList.Count;
            obj.GetComponent<Item>().index = index;
            obj.GetComponent<Item>().sprite = floorSpriteList[i];
            obj.GetComponent<Image>().sprite = floorSpriteList[i];

        }
    }

}
