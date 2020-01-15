using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desk : Furniture
{
    public int DeskId;
    public Transform[] Spots;
    public Chair[] Chairs;
    public bool Available;

    public Desk(Vector3 position, Transform[] spots)
    {
        Position = position;
        Available = true;
        Spots = spots;
    }
}
