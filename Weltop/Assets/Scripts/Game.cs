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
    
    #region Build Round
    private float buildRoundTimer;
    public int numOfPlacedObjects;
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

    public GameObject[] GetActivePlayers()
    {
        return GameObject.FindGameObjectsWithTag("Player");
    }
    
    private void InitializeParkourRound()
    {
        ui.ToParkourRoundUI();
        parkourRoundTimer = GlobalParameters.ParkourRoundTimeSeconds;
        
        foreach (GameObject player in GetActivePlayers())
        {
            Debug.Log(player);
            ViewSwitcher viewSwitcher = player.GetComponent<ViewSwitcher>();
            
            viewSwitcher.SwitchToPlayer();
            spawnpointManager.SendToSpawnpoint(player);
        }
    }

    private void EndRound()
    {
        if (!gameIsActive)
            return;
        
        switch (currentRoundState)
        {
            case RoundState.Parkour:
                parkourRoundTimer = 0;
                currentRoundState = RoundState.Build;
                InitializeBuildRound();
                break;
            case RoundState.Build:
                numOfPlacedObjects = 0;
                currentRoundState = RoundState.Parkour;
                InitializeParkourRound();
                break;
        }
    }
    
    private void InitializeBuildRound()
    {
        ui.ToBuildRoundUI();
        ui.UpdateBuildRoundObjectsPlaced();
        buildRoundTimer = GlobalParameters.BuildRoundTimeSeconds;

        foreach (GameObject player in GetActivePlayers())
        {
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
                buildRoundTimer -= Time.deltaTime;
                ui.UpdateBuildRoundTimer(buildRoundTimer);
                
                if (buildRoundTimer <= 0)
                {
                    EndRound();
                }
                break;
        }
    }

    public void CheckIfAllPlayersPlacedAnObject(BuildModeController buildModeController)
    {
        int playerCount = GetActivePlayers().Length;
        
        if (buildModeController.hasPlacedObject)
        {
            numOfPlacedObjects += 1;
        }
        
        ui.UpdateBuildRoundObjectsPlaced();
        
        if (numOfPlacedObjects >= playerCount)
        {
            EndRound();
        }
    }
}
