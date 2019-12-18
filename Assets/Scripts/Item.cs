using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int index;
    public Sprite sprite;

    public Item (int i, Sprite s)
    {
        index = i;
        sprite = s;
    }

}
