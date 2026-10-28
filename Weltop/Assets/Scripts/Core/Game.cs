using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Game : MonoBehaviour
{
    public RoundState currentRoundState;
    public bool gameIsActive;
    
    [Header("Match Progress")]
    public int currentParkourRound = 1;
    public int currentBuildRound = 0;
    public bool isGameComplete = false;
    
    public int TotalBuildRounds =>
        Mathf.Max(0, GlobalParameters.TotalParkourRounds - 1);
    
    private SpawnpointManager spawnpointManager;
    private Transitions transitions;
    private UI ui;
    
    public bool currentlyInTransition = false;
    
    #region Parkour Round
    public float parkourRoundTimer;
    #endregion
    
    #region Build Round
    public float buildRoundTimer;
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
        transitions = GetComponent<Transitions>();
        spawnpointManager = GetComponent<SpawnpointManager>();
    }

    private void Start()
    {
        isGameComplete = false;
        
        currentParkourRound = 1;
        currentBuildRound = 0;
    
        currentRoundState = RoundState.Parkour;
        InitializeParkourRound();
    
        Debug.Log($"[Match] Starting Parkour {currentParkourRound}/{GlobalParameters.TotalParkourRounds}");
    }

    public GameObject[] GetActivePlayers()
    {
        return GameObject.FindGameObjectsWithTag("Player");
    }

    #region Code Executions for All Players
    public void ExecuteCodeOnAllPlayerControllers(Action<PlayerController> action)
    {
        foreach (GameObject player in GetActivePlayers())
        {
            if (player == null)
                return;
                
            PlayerController playerController = player.GetComponent<PlayerController>();
            
            action(playerController);
        }
    }
    
    public void ExecuteCodeOnAllBuildControllers(Action<BuildModeController> action)
    {
        foreach (GameObject player in GetActivePlayers())
        {
            if (player == null)
                return;
            
            BuildModeController buildModeController = player.GetComponent<BuildModeController>();
            
            action(buildModeController);
        }
    }
    
    public void ExecuteCodeOnAllPlayerCameras(Action<Cam3dPerson> action)
    {
        foreach (GameObject player in GetActivePlayers())
        {
            if (player == null)
                return;
            
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats == null)
                return;

            Cam3dPerson camera = playerStats.playerCamera;
            
            action(camera);
        }
    }
    
    public void ExecuteCodeOnAllViewSwitchers(Action<ViewSwitcher> action)
    {
        foreach (GameObject player in GetActivePlayers())
        {
            if (player == null)
                return;
            
            ViewSwitcher viewSwitcher = player.GetComponent<ViewSwitcher>();
            
            action(viewSwitcher);
        }
    }
    #endregion
    
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

    public void EndRound()
    {
        if (!gameIsActive || isGameComplete)
            return;

        switch (currentRoundState)
        {
            case RoundState.Parkour:
                parkourRoundTimer = 0;

                // Check whether the final Parkour round has ended.
                if (currentParkourRound >= GlobalParameters.TotalParkourRounds)
                {
                    CompleteGame();
                    return;
                }

                currentBuildRound++;

                currentRoundState = RoundState.Build;
                InitializeBuildRound();

                Debug.Log(
                    $"[Match] Starting Build {currentBuildRound}/{TotalBuildRounds}"
                );
                break;

            case RoundState.Build:
                numOfPlacedObjects = 0;

                currentParkourRound++;

                currentRoundState = RoundState.Parkour;
                InitializeParkourRound();

                Debug.Log(
                    $"[Match] Starting Parkour {currentParkourRound}/{GlobalParameters.TotalParkourRounds}"
                );
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

                if (parkourRoundTimer <= GlobalParameters.StartRoundCountDownAtXSeconds + 1)
                {
                    if (!currentlyInTransition)
                    {
                        transitions.StartTransitionToNextRound();
                        currentlyInTransition = true;
                    }
                }
                break;
            case RoundState.Build:
                buildRoundTimer = Mathf.Max(0f, buildRoundTimer - Time.deltaTime);
                ui.UpdateBuildRoundTimer(buildRoundTimer);
                
                if (buildRoundTimer <= GlobalParameters.StartRoundCountDownAtXSeconds + 1)
                {
                    if (!currentlyInTransition)
                    {
                        transitions.StartTransitionToNextRound();
                        currentlyInTransition = true;
                    }
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
            //If timer starts going down, this will stop it immediately
            transitions.StopTransitionCoroutine();
            EndRound();
        }
    }
    
    private void CalculateWinners()
    {
        GameObject[] players = GetActivePlayers();

        if (players.Length == 0)
        {
            Debug.LogWarning("[Match] No players found!");
            return;
        }

        int highestScore = int.MinValue;
        List<PlayerStats> winners = new List<PlayerStats>();

        foreach (GameObject player in players)
        {
            PlayerStats stats = player.GetComponent<PlayerStats>();

            if (stats == null)
                continue;

            int score = stats.currentHillPoints;

            if (score > highestScore)
            {
                highestScore = score;
                winners.Clear();
                winners.Add(stats);
            }
            else if (score == highestScore)
            {
                winners.Add(stats);
            }
        }

        if (winners.Count == 0)
        {
            Debug.LogWarning("[Match] No valid player scores found!");
            return;
        }

        Debug.Log($"[Match] Winning Score: {highestScore}");

        foreach (PlayerStats winner in winners)
        {
            PlayerInput playerInput = winner.GetComponent<PlayerInput>();

            if (playerInput != null)
            {
                int playerNumber = playerInput.playerIndex + 1;
                Debug.Log($"[Match] Winner: Player {playerNumber}");
            }
            else
            {
                Debug.LogWarning("[Match] Winner has no PlayerInput component!");
            }
        }

        if (winners.Count > 1)
        {
            Debug.Log($"[Match] JOINT WINNERS! {winners.Count} players tied.");
        }
    }
    
    private void CompleteGame()
    {
        isGameComplete = true;
        gameIsActive = false;

        CalculateWinners();

        ExecuteCodeOnAllPlayerControllers(controller =>
        {
            controller.canMove = false;
        });

        ExecuteCodeOnAllPlayerCameras(camera =>
        {
            camera.canLook = false;
        });

        Debug.Log("[Match] GAME COMPLETE!");
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
