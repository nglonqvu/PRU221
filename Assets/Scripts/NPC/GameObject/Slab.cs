using UnityEngine;
using System;
using System.Collections;

public class Slab : BuildingBlock
{
    private void Start()
    {
        _flyweight = GetFlyweight(BlockCategory.BuildingBlock);
    }
}

