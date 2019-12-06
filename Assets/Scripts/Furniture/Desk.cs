using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desk : Furniture
{
    public Vector3[] Spots;
    public Chair[] Chairs;
    public bool Available;

    public Desk(Vector3 position, Chair[] chairList)
    {
        Position = position;
        Available = true;
        Chairs = chairList;
    }
}
