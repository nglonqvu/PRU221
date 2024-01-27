using UnityEngine;
using System.Collections.Generic;
using System;

public class BlockFlyweightFactory
{
    private static BlockFlyweightFactory _instance = null;

    private readonly Dictionary<BlockCategory, IBlockFlyweight> _flyweightMap;

    private BlockFlyweightFactory()
    {
        _flyweightMap = new();
    }

    public IBlockFlyweight GetFlyweight(BlockCategory key)
    {
        if (_flyweightMap.TryGetValue(key, out var flyweight))
        {
            return flyweight;
        }

        if (key == BlockCategory.BuildingBlock)
        {
            flyweight = new BuildingFlyweight();
            _flyweightMap.Add(key, flyweight);
            return flyweight;
        }

        if (key == BlockCategory.Obstacle)
        {
            flyweight = new ObstacleFlyweight();
            _flyweightMap.Add(key, flyweight);
            return flyweight;
        }

        return null;
    }

    // Factory as Singleton object
    public static BlockFlyweightFactory GetInstance()
    {
        if (_instance == null)
        {
            return new BlockFlyweightFactory();
        }

        return _instance;
    }
}

