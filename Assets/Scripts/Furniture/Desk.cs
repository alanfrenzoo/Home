﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desk : Furniture
{
    public int DeskId;
    public Vector3[] Spots;
    public Chair[] Chairs;
    public bool Available;

    public Desk(Vector3 position, Vector3[] spots)
    {
        Position = position;
        Available = true;
        Spots = spots;
    }
}
