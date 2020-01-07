using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct ReplaceTag : IComponentData
{
    public int Value;
}
public class ReplaceTagComponent : ComponentDataProxy<ReplaceTag> { }