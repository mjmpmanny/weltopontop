using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    public TMP_Text hillPointsTracker;

    public void UpdateHillPoints(PlayerStats playerStats)
    {
        hillPointsTracker.text = $"Player 1: {playerStats.currentHillPoints}";
    }
}
