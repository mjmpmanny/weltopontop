using System;
using UnityEngine;

public class Game : MonoBehaviour
{
    public RoundState currentRoundState;
    public bool gameIsActive;
    
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
        currentRoundState = RoundState.Parkour;
        InitializeParkourRound();
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
