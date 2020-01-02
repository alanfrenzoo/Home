using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Register : Furniture
{
    public int RegisterId;
    public Vector3[] Spots;
    public bool Available;

    public Register(Vector3 position, Vector3[] spots)
    {
        Position = position;
        Available = true;
        Spots = spots;
    }
}
