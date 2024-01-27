using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameOverScreen gameOverScreen;
    [SerializeField]
    private PlayerBehaviour playerBehaviour;
    private int score;

    private bool isPlaying = false;

    private void Start()
    {
        playerBehaviour.GameOverEvent.Subscribe(OnObstacleHit);
        gameOverScreen.onReplayButtonClicked.Subscribe(OnReplayButtonClicked);
        isPlaying = true;
    }

    private void OnReplayButtonClicked(bool repeat)
    {
        if (repeat)
        {
             Debug.Log("onReplayButtonClicked called with value " + repeat);
        //reset score
        score = 0;
        //change isPlaying state
        isPlaying = true;
        //hide game over screen
        gameOverScreen.HideGameOverScreen();
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;

        }
    }

    private void Update()
    {
        if (isPlaying)
        {
            float distanceTraveled = playerBehaviour.totalSurviveTime;
            score = Mathf.RoundToInt(distanceTraveled);
            UpdateScore(score);
        }
    }

    private void OnObstacleHit(bool hit)
    {
        if (hit)
        {
            //pause game
            Time.timeScale = 0;
            isPlaying = false;
            // Show the game over screen and pass the player's final score
            gameOverScreen.ShowGameOverScreen(score);
        }
    }

    public void UpdateScore(int score)
    {
        this.score = score;
    }

    //gets the score
    public float GetScore()
    {
        return score;
    }
}

