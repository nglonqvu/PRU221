using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField]
    private TMP_Text scoreText;
    [SerializeField]
    private Button replayButton;

    public Observable<bool> onReplayButtonClicked = new Observable<bool>();

    private void Start()
    {
        replayButton.onClick.AddListener(() =>
        {
           onReplayButtonClicked.Notify(true);
        });
    }

    public void ShowGameOverScreen(float score)
    {
        scoreText.text = "Score: " + score.ToString();
        gameObject.SetActive(true);
    }

    public void HideGameOverScreen()
    {
        gameObject.SetActive(false);
    }
}

