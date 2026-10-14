using System;
using UnityEngine;

public class Game : MonoBehaviour
{
    public RoundState currentRoundState;
    public bool gameIsActive;
    
    private SpawnpointManager spawnpointManager;
    private UI ui;
    
    #region Parkour Round
    private float parkourRoundTimer;
    #endregion
    
    public enum RoundState
    {
        Parkour,
        Build
    }

    private void Awake()
    {
        ui = GetComponent<UI>();
        spawnpointManager = GetComponent<SpawnpointManager>();
    }

    private void Start()
    {
        currentRoundState = RoundState.Parkour;
        InitializeParkourRound();
    }

    private GameObject[] GetActivePlayers()
    {
        return GameObject.FindGameObjectsWithTag("Player");
    }
    
    private void InitializeParkourRound()
    {
        ui.ShowParkourRoundUI();
        parkourRoundTimer = GlobalParameters.ParkourRoundTimeSeconds;
        
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
        {
            foreach (GameObject player in players)
            {
                spawnpointManager.SendToSpawnpoint(player);
            }
        }
    }

    private void EndRound()
    {
        if (!gameIsActive)
            return;
        
        switch (currentRoundState)
        {
            case RoundState.Parkour:
                ui.HideParkourRoundUI();
                parkourRoundTimer = 0;
                currentRoundState = RoundState.Build;
                InitializeBuildRound();
                break;
            case RoundState.Build:
                currentRoundState = RoundState.Parkour;
                InitializeParkourRound();
                break;
        }
    }
    
    private void InitializeBuildRound()
    {
        foreach (GameObject player in GetActivePlayers())
        {
            Debug.Log(player);
            ViewSwitcher viewSwitcher = player.GetComponent<ViewSwitcher>();
            
            viewSwitcher.SwitchToBuild(viewSwitcher.TestObjectToHoldOnTo, new Vector3(0,10,0));
        }
    }

    private void Update()
    {
        switch (currentRoundState)
        {
            case RoundState.Parkour:
                parkourRoundTimer -= Time.deltaTime;
                ui.UpdateParkourRoundTimer(parkourRoundTimer);

                if (parkourRoundTimer <= 0)
                {
                    EndRound();
                }
                break;
            case RoundState.Build:
                break;
        }
    }
}
