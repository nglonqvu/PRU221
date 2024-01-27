using System;
using UnityEngine;

public class BuildingBlock: MonoBehaviour
{
    protected float _speed = GameConst.PLATFORM_SPEED * GameConst.SPEED_SCALE;

    protected Rigidbody2D _rigidbody;

    protected IBlockFlyweight _flyweight;

    protected Vector3 _originalPosition;

    protected virtual void Awake()
    {
        _originalPosition = transform.localPosition;
    }

    protected virtual void OnEnable()
    {
        transform.localPosition = _originalPosition;
    }

    protected virtual void Update()
    {
        _flyweight.Move(transform, _speed);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        _flyweight.HandleCollision(collision);
    }

    protected IBlockFlyweight GetFlyweight(BlockCategory type)
    {
        BlockFlyweightFactory flyweightFactory = BlockFlyweightFactory.GetInstance();
        var flyweight = flyweightFactory.GetFlyweight(type);

        if (flyweight == null)
        {
            throw new Exception($"Cannot resolve flyweight type for {type.ToString()}");
        }

        return flyweight;
    }
}

