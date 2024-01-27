using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingState : PlayerState
{
    private bool IsTouching = false;
    private float FlyingSpeed;

    public override void SetUpEnviroment()
    {
        // Disable UnityEngine gravity in fly mode
        _playerBehaviour.rb.gravityScale = 0;

        // Set Active for rocket in fly mode
        var rocket = _playerBehaviour.transform.GetChild(1);
        rocket.gameObject.SetActive(true);
        FlyingSpeed = _playerBehaviour.speed * Time.deltaTime * GameConst.SPEED_SCALE;
    }

    public override void HandleUserSingleTouch()
    {
        IsTouching = true;
        _playerBehaviour.transform.Translate(new Vector3(0, FlyingSpeed, 0));
    }

    public override void StateByFrame()
    {
        if (!IsTouching)
        {
            _playerBehaviour.transform.Translate(new Vector3(0, FlyingSpeed * -1, 0));
        }
        IsTouching = false;
    }

    public override void GoThroughPortal()
    {
        _playerBehaviour.TransitionTo(new NormalState());
    }
}
