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

        spawnpointManager.ResetSpawnAssignments();

        foreach (GameObject player in GetActivePlayers())
        {
            ViewSwitcher viewSwitcher = player.GetComponent<ViewSwitcher>();

            viewSwitcher.SwitchToPlayer();
            spawnpointManager.SendToSpawnpointsRandom(player);
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
            PlayerController playerController = player.GetComponent<PlayerController>();
            BuildModeController buildModeController = playerController.buildModeController;

            buildModeController.hasPlacedObject = false;
            viewSwitcher.SwitchToBuild(viewSwitcher.TestObjectToHoldOnTo, new Vector3(0,10,0));
        }
    }

    private void Update()
    {
        if (!gameIsActive)
            return;
        
        switch (currentRoundState)
        {
            case RoundState.Parkour:
                parkourRoundTimer = Mathf.Max(0f, parkourRoundTimer - Time.deltaTime);
                ui.UpdateParkourRoundTimer(parkourRoundTimer);

                if (parkourRoundTimer <= 0)
                {
                    EndRound();
                }
                break;
            case RoundState.Build:
                buildRoundTimer = Mathf.Max(0f, buildRoundTimer - Time.deltaTime);
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
            Debug.Log($"[Build Mode] Objects placed: {numOfPlacedObjects}/{playerCount}");
        }
        
        ui.UpdateBuildRoundObjectsPlaced();
        
        if (numOfPlacedObjects >= playerCount)
        {
            EndRound();
        }
    }
    
    public void DebugStartParkourRound()
    {
        currentRoundState = RoundState.Parkour;
        numOfPlacedObjects = 0;
        InitializeParkourRound();
    }

    public void DebugStartBuildRound()
    {
        currentRoundState = RoundState.Build;
        numOfPlacedObjects = 0;
        InitializeBuildRound();
    }

    public void DebugEndRound()
    {
        EndRound();
    }
    
    public void DebugResetPlacementCount()
    {
        numOfPlacedObjects = 0;
        ui.UpdateBuildRoundObjectsPlaced();
    }
    
}
