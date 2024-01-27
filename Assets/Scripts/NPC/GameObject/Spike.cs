using UnityEngine;
using System.Collections;
using System;

public class Spike : BuildingBlock
{
    private void Start()
    {
        _flyweight = GetFlyweight(BlockCategory.Obstacle);
    }
}

