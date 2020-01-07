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

        var xStart = Mathf.RoundToInt(transform.localPosition.x) - Mathf.RoundToInt(area.HalfWidth);
        var xEnd = Mathf.RoundToInt(transform.localPosition.x) + Mathf.RoundToInt(area.HalfWidth);
        var zStart = Mathf.RoundToInt(transform.localPosition.z) - Mathf.RoundToInt(area.HalfLength);
        var zEnd = Mathf.RoundToInt(transform.localPosition.z) + Mathf.RoundToInt(area.HalfLength);

        for (int x = xStart + 1; x < xEnd; x++)
        {
            for (int z = zStart + 1; z < zEnd; z++)
            {
                string pos = string.Format("({0}, 0, {1})", x, z);
                ItemManager.instance.PlaceFloorTile(true, pos);
            }
        }

    }

}
