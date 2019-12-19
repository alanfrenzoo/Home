using EasyBuildSystem.Runtimes.Internal.Area;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AreaReferenceGenerator : MonoBehaviour
{
    public GameObject prefab;
    public NavMeshSurface nav;

    void Start()
    {
        var area = transform.GetComponent<AreaBehaviour>();

        var plane = Instantiate(prefab, transform);
        plane.transform.localScale = new Vector3(area.HalfWidth * 2, 0.05f, area.HalfLength * 2);
        plane.layer = LayerMask.NameToLayer("House");
        plane.SetActive(false);
        

    }

}
