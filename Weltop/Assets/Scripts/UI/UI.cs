using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [Header("Player Score Displays")]
    [SerializeField] private TMP_Text[] playerScoreTexts = new TMP_Text[4];

    public TMP_Text parkourRoundTimerText;
    public TMP_Text buildRoundTimerText;
    public TMP_Text buildRoundObjectsPlacedText;
    
    [Header("Round Tracker")]
    [SerializeField] private TMP_Text roundTrackerText;
    
    [Header("Game Results")]
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private TMP_Text finalScoresText;
    [SerializeField] private Button returnToMenuButton;

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

        UpdateRoundTracker();
    }
    
    public void ToBuildRoundUI()
    {
        parkourRoundPanel.SetActive(false);
        buildRoundPanel.SetActive(true);

        UpdateRoundTracker();
    }
    
    public void UpdateRoundTracker()
    {
        if (roundTrackerText == null)
            return;

        if (game.currentRoundState == Game.RoundState.Parkour)
        {
            int current = game.currentParkourRound;
            int total = GlobalParameters.TotalParkourRounds;

            if (current >= total)
            {
                roundTrackerText.text =
                    $"PARKOUR {current} / {total} FINAL ROUND";
            }
            else
            {
                roundTrackerText.text =
                    $"PARKOUR {current} / {total}";
            }
        }
        else
        {
            roundTrackerText.text =
                $"BUILD {game.currentBuildRound} / {game.TotalBuildRounds}";
        }
    }
    
    public void ShowResults()
    {
        List<PlayerStats> players = new List<PlayerStats>();

        foreach (GameObject player in game.GetActivePlayers())
        {
            PlayerStats stats = player.GetComponent<PlayerStats>();

            if (stats != null)
                players.Add(stats);
        }

        players = players
            .OrderByDescending(stats => stats.currentHillPoints)
            .ThenBy(stats => stats.PlayerIndex)
            .ToList();

        if (players.Count == 0)
        {
            winnerText.text = "NO PLAYERS FOUND";
            finalScoresText.text = "";
            resultsPanel.SetActive(true);
            return;
        }

        int highestScore = players[0].currentHillPoints;

        List<string> winnerNames = new List<string>();

        foreach (PlayerStats stats in players)
        {
            if (stats.currentHillPoints == highestScore)
                winnerNames.Add($"PLAYER {stats.PlayerIndex + 1}");
        }

        if (winnerNames.Count == 1)
        {
            winnerText.text = $"{winnerNames[0]} WINS!";
        }
        else
        {
            winnerText.text =
                $"JOINT WINNERS:\n{string.Join(" & ", winnerNames)}!";
        }

        finalScoresText.text = "";

        foreach (PlayerStats stats in players)
        {
            string playerName = $"Player {stats.PlayerIndex + 1}";
            int score = stats.currentHillPoints;

            string winnerMarker = score == highestScore ? " (WINNER)" : "";

            finalScoresText.text +=
                $"{playerName}: {score} points{winnerMarker}\n";
        }

        resultsPanel.SetActive(true);
        
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(
            returnToMenuButton.gameObject
        );
    }
    
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        MenuPlayerSelection.Clear();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("MainMenu");
    }
    
}