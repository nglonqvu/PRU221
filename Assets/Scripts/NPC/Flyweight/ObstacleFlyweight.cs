using UnityEngine;
using System.Collections;

public class ObstacleFlyweight : IBlockFlyweight
{
    public void HandleCollision(Collision2D collision)
    {
        if(collision.transform.CompareTag(GameTag.Player))
        {
            // TODO: Invoke game over event
        }
    }
}

