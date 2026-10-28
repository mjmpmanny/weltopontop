using System;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [Header("Player Score Displays")]
    [SerializeField] private TMP_Text[] playerScoreTexts = new TMP_Text[4];

    public TMP_Text parkourRoundTimerText;
    public TMP_Text buildRoundTimerText;
    public TMP_Text buildRoundObjectsPlacedText;

    public GameObject parkourRoundPanel;
    public GameObject buildRoundPanel;

    private Transitions transitions;
    private Game game;
    
    private void Awake()
    {
        transitions = GetComponent<Transitions>();
        game = GetComponent<Game>();
        
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
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        string formattedTime = time.ToString(@"m\:ss");
        parkourRoundTimerText.text = formattedTime;
        double wholeSeconds = Math.Floor(time.TotalSeconds); 
        transitions.countdownText.text = wholeSeconds.ToString("N0");
    }
    
    public void UpdateBuildRoundTimer(float currentTime)
    {
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        string formattedTime = time.ToString(@"m\:ss");
        buildRoundTimerText.text = formattedTime;
        double wholeSeconds = Math.Floor(time.TotalSeconds); 
        transitions.countdownText.text = wholeSeconds.ToString("N0");
    }

    public void UpdateBuildRoundObjectsPlaced()
    {
        buildRoundObjectsPlacedText.text = $"{game.numOfPlacedObjects}/{game.GetActivePlayers().Length} objects placed";
    }

    public void ToParkourRoundUI()
    {
        foreach (PlayerStats player in FindObjectsByType<PlayerStats>(FindObjectsSortMode.None))
        {
            UpdateHillPoints(player);
        }
        parkourRoundPanel.SetActive(true);
        buildRoundPanel.SetActive(false);
    }
    
    public void ToBuildRoundUI()
    {
        parkourRoundPanel.SetActive(false);
        buildRoundPanel.SetActive(true);
    }
}