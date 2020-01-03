using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct FloorTag : IComponentData
{
    public int Value;
}
public class FloorTagComponent : ComponentDataProxy<FloorTag> { }