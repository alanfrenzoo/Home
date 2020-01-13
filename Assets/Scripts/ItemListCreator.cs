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
    private List<int> furniToPartCollectionIndex;
    private List<int> floorToPartCollectionIndex;

    void Start()
    {
        furniToPartCollectionIndex = new List<int>();
        floorToPartCollectionIndex = new List<int>();

        for(int i = 0; i < BuildManager.Instance.PartsCollection.Parts.Count; i++)
        {
            var pt = BuildManager.Instance.PartsCollection.Parts[i];
            if (pt.Type == EasyBuildSystem.Runtimes.Internal.Part.PartType.Floor)
                floorToPartCollectionIndex.Add(i);
            else
                furniToPartCollectionIndex.Add(i);
        }

        for (int i = 0; i < furniSpriteList.Count; i++)
        {
            var obj = Instantiate(prefab);
            obj.transform.SetParent(furniContent);

            obj.GetComponent<Item>().index = furniToPartCollectionIndex[i];
            obj.GetComponent<Item>().sprite = furniSpriteList[i];
            obj.GetComponent<Image>().sprite = furniSpriteList[i];

        }

        for (int i = 0; i < floorSpriteList.Count; i++)
        {
            var obj = Instantiate(prefab);
            obj.transform.SetParent(floorContent);

            obj.GetComponent<Item>().index = floorToPartCollectionIndex[i];
            obj.GetComponent<Item>().sprite = floorSpriteList[i];
            obj.GetComponent<Image>().sprite = floorSpriteList[i];

        }

        furniToPartCollectionIndex.Clear();
        floorToPartCollectionIndex.Clear();

    }

}
