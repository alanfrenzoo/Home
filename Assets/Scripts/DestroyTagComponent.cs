using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct DestroyTag : IComponentData
{
    public int Value;
}
public class DestroyTagComponent : ComponentDataProxy<FurniTag> { }