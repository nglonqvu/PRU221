using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public float speed;
    public float jumpSpeed;
    public float rotateSpeed;
    public float gravity;

    public Rigidbody2D rb;
    public Transform Sprite;

    private PlayerState _state;

    public Observable<bool> GameOverEvent = new Observable<bool>();
    public float totalSurviveTime = 0f;

    public void TransitionTo(PlayerState state)
    {
        _state = state;
        _state.SetContext(this);
    }

    public void HandleUserSingleTouch()
    {
        _state.HandleUserSingleTouch();
    }

    public void StateByFrame()
    {
        _state.StateByFrame();
    }

    public void GoThroughPortal()
    {
        _state.GoThroughPortal();
    }

    // Start is called before the first frame update
    private void Start()
    {
        TransitionTo(new NormalState());
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        totalSurviveTime += Time.deltaTime;

        if (Input.touchCount > 0 || Input.GetKey(KeyCode.Space))
        {
            HandleUserSingleTouch();
        }

        StateByFrame();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag(GameTag.Portal))
        {
            GoThroughPortal();
        }
        else
        {
            _state.OnCollisionEnter(collision);
        }
    }

    public void Destroy()
    {
        GameOverEvent.Notify(true);
    }
}
