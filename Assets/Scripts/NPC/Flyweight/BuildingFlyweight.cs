using UnityEngine;
using System.Collections;

public class BuildingFlyweight : IBlockFlyweight
{
    public void HandleCollision(Collision2D collision)
    {
        if(collision.transform.CompareTag(GameTag.Player)) {
            ContactPoint2D contactPoint = collision.contacts[0];

            Vector2 normalized = contactPoint.normal;
            if(normalized.x != 0 && Mathf.Abs(normalized.y) < 0.1f)
            {
                // TODO: Invoke game over event
            }
        }
    }
}

