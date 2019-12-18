using EasyBuildSystem.Runtimes.Internal.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemListCreator : MonoBehaviour
{
    public GameObject prefab;
    public Transform content;

    public List<Sprite> spriteList;

    void Start()
    {
        for(int i = 0; i < BuildManager.Instance.PartsCollection.Parts.Count; i++)
        {
            var obj = Instantiate(prefab);
            obj.transform.SetParent(content);

            obj.GetComponent<Item>().index = i;
            obj.GetComponent<Item>().sprite = spriteList[i];
            obj.GetComponent<Image>().sprite = spriteList[i];
            
        }
    }

}
