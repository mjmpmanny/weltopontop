using System;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [Header("Player Score Displays")]
    [SerializeField] private TMP_Text[] playerScoreTexts = new TMP_Text[4];

    public TMP_Text parkourRoundTimerText;
    
    private void Awake()
    {
        // Hide score displays until players join.
        foreach (TMP_Text scoreText in playerScoreTexts)
        {
            if (scoreText != null)
            {
                scoreText.gameObject.SetActive(false);
            }
        }
    }

    public void UpdateHillPoints(PlayerStats playerStats)
    {
        int playerIndex = playerStats.PlayerIndex;

        if (playerIndex < 0 || playerIndex >= playerScoreTexts.Length)
        {
            Debug.LogWarning("Invalid player index: " + playerIndex);
            return;
        }

        TMP_Text scoreText = playerScoreTexts[playerIndex];

        if (scoreText == null)
        {
            Debug.LogWarning("Score UI missing for Player " + (playerIndex + 1));
            return;
        }

        scoreText.gameObject.SetActive(true);

        scoreText.text =
            $"Player {playerIndex + 1}: {playerStats.currentHillPoints}";
    }

    public void UpdateParkourRoundTimer(float currentTime)
    {
        // Convert the float to a TimeSpan
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        string formattedTime = time.ToString(@"m\:ss");
        parkourRoundTimerText.text = formattedTime;
    }

    public void ShowParkourRoundUI()
    {
        foreach (TMP_Text playerScoreText in playerScoreTexts)
        {
            playerScoreText.gameObject.SetActive(true);
        }
        parkourRoundTimerText.gameObject.SetActive(true);
    }
    
    public void HideParkourRoundUI()
    {
        foreach (TMP_Text playerScoreText in playerScoreTexts)
        {
            playerScoreText.gameObject.SetActive(false);
        }
        parkourRoundTimerText.gameObject.SetActive(false);
    }
}