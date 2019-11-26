using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct FurniTag : IComponentData
{
    public int Value;
}
public class FurniTagComponent : ComponentDataProxy<FurniTag> { }