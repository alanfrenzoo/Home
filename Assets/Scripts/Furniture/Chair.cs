using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : Furniture
{
    public int ChairId;
    public Transform[] Spots;
    public bool Available;
    public Desk Desk;

    public Chair(Vector3 position, Transform[] spots, Desk desk)
    {
        Position = position;
        Spots = spots;
        Available = true;
        Desk = desk;
    }
}
