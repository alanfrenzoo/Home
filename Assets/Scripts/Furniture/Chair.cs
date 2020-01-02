using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : Furniture
{
    public int ChairId;
    public Vector3[] Spots;
    public bool Available;

    public Chair(Vector3 position, Vector3[] spots)
    {
        Position = position;
        Spots = spots;
        Available = true;
    }
}
