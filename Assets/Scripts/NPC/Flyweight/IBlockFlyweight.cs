using UnityEngine;
using System.Collections;

public interface IBlockFlyweight
{
	// Default implementation for moving building blocks
	public void Move(Transform transform, float speed)
	{
		transform.Translate(speed * Time.deltaTime * Vector3.left);
	}

	public void HandleCollision(Collision2D collision);
}

