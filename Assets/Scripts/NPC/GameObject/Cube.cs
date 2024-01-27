using UnityEngine;
using System.Collections;
using System;

public class Cube : BuildingBlock
{
    private void Start()
	{
		_flyweight = GetFlyweight(BlockCategory.BuildingBlock);
    }
}

