using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

// Serializable attribute is for editor support.
[Serializable]
public struct MovingTag : IComponentData
{
    // MovingTag is a "tag" component and contains no data. Tag components can be used to mark entities that a system should process.
}

