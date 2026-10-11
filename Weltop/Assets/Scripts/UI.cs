using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [Header("Player Score Displays")]
    [SerializeField] private TMP_Text[] playerScoreTexts = new TMP_Text[4];

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
}