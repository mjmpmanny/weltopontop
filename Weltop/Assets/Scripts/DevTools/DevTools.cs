
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DevTools : MonoBehaviour
{
    [SerializeField] private Game game;
    [SerializeField] private SpawnpointManager spawnpointManager;
    [SerializeField] private Transform hillTeleportPoint;

    private int selectedPlayerIndex = 0;

    private bool visible;
    private Rect windowRect = new Rect(20, 20, 400, 370);

    private int selectedTab;

    private readonly string[] tabs =
    {
        "Game",
        "Players",
        "Build"
    };

    private float previousTimeScale = 1f;

    private readonly List<InputActionMap> disabledActionMaps = new();

    private void Awake()
    {
        if (game == null)
            game = FindFirstObjectByType<Game>();

        if (spawnpointManager == null)
            spawnpointManager = FindFirstObjectByType<SpawnpointManager>();
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.backquoteKey.wasPressedThisFrame)
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        visible = !visible;

        if (visible)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;

            disabledActionMaps.Clear();

            foreach (PlayerInput player in PlayerInput.all)
            {
                InputActionMap map = player.currentActionMap;

                if (map != null && map.enabled &&
                    !disabledActionMaps.Contains(map))
                {
                    map.Disable();
                    disabledActionMaps.Add(map);
                }
            }

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            RestoreGameplay();
        }
    }

    private void RestoreGameplay()
    {
        Time.timeScale = previousTimeScale;

        foreach (InputActionMap map in disabledActionMaps)
        {
            if (map != null)
                map.Enable();
        }

        disabledActionMaps.Clear();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        if (visible)
        {
            visible = false;
            RestoreGameplay();
        }
    }

    private void OnGUI()
    {
        if (!visible)
            return;

        windowRect = GUI.Window(
            987654,
            windowRect,
            DrawWindow,
            "WELTOP Dev Tools"
        );
    }

    private void DrawWindow(int id)
    {
        selectedTab = GUI.Toolbar(
            new Rect(10, 30, 380, 30),
            selectedTab,
            tabs
        );

        switch (selectedTab)
        {
            case 0:
                DrawGameTab();
                break;

            case 1:
                DrawPlayersTab();
                break;

            case 2:
                DrawBuildTab();
                break;
        }

        GUI.DragWindow(new Rect(0, 0, 400, 20));
    }

    private void DrawGameTab()
    {
        if (game == null)
        {
            GUI.Label(
                new Rect(15, 80, 350, 25),
                "Game manager not found!"
            );
            return;
        }

        GUI.Label(
            new Rect(15, 75, 350, 25),
            "Current Round: " + game.currentRoundState
        );

        if (GUI.Button(
            new Rect(15, 110, 175, 35),
            "Start Parkour"))
        {
            game.DebugStartParkourRound();
        }

        if (GUI.Button(
            new Rect(205, 110, 175, 35),
            "Start Build"))
        {
            game.DebugStartBuildRound();
        }

        if (GUI.Button(
            new Rect(15, 155, 365, 35),
            "End Current Round"))
        {
            game.DebugEndRound();
        }

        GUI.Label(
            new Rect(15, 205, 365, 25),
            "Parkour Duration: " +
            GlobalParameters.ParkourRoundTimeSeconds.ToString("F0") +
            " seconds"
        );

        GlobalParameters.ParkourRoundTimeSeconds =
            Mathf.Round(GUI.HorizontalSlider(
                new Rect(15, 235, 365, 20),
                GlobalParameters.ParkourRoundTimeSeconds,
                10f,
                240f
            ));

        GUI.Label(
            new Rect(15, 270, 365, 25),
            "Build Duration: " +
            GlobalParameters.BuildRoundTimeSeconds.ToString("F0") +
            " seconds"
        );

        GlobalParameters.BuildRoundTimeSeconds =
            Mathf.Round(GUI.HorizontalSlider(
                new Rect(15, 300, 365, 20),
                GlobalParameters.BuildRoundTimeSeconds,
                10f,
                240f
            ));
    }
    
    private void DrawPlayersTab()
    {
        PlayerInput selectedPlayer = null;

        // Display buttons only for players currently in the match.
        for (int i = 0; i < PlayerInput.all.Count; i++)
        {
            PlayerInput player = PlayerInput.all[i];

            if (GUI.Toggle(
                new Rect(15 + i * 95, 80, 90, 30),
                selectedPlayerIndex == player.playerIndex,
                "Player " + (player.playerIndex + 1),
                "Button"))
            {
                selectedPlayerIndex = player.playerIndex;
            }
        }

        foreach (PlayerInput player in PlayerInput.all)
        {
            if (player.playerIndex == selectedPlayerIndex)
            {
                selectedPlayer = player;
                break;
            }
        }

        if (selectedPlayer == null)
        {
            GUI.Label(
                new Rect(15, 130, 350, 25),
                "No player selected."
            );
            return;
        }

        PlayerStats stats = selectedPlayer.GetComponent<PlayerStats>();
        Rigidbody rb = selectedPlayer.GetComponent<Rigidbody>();

        GUI.Label(
            new Rect(15, 125, 350, 25),
            "Selected: Player " + (selectedPlayer.playerIndex + 1)
        );

        if (GUI.Button(
            new Rect(15, 165, 175, 35),
            "Respawn"))
        {
            if (spawnpointManager != null)
            {
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                spawnpointManager.SendToSpawnpointsRandom(
                    selectedPlayer.gameObject
                );
            }
        }

        if (GUI.Button(
            new Rect(205, 165, 175, 35),
            "Teleport to Hill"))
        {
            if (hillTeleportPoint != null)
            {
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                selectedPlayer.transform.position =
                    hillTeleportPoint.position;
            }
        }

        if (stats == null)
            return;

        GUI.Label(
            new Rect(15, 215, 350, 25),
            "Hill Points: " + stats.currentHillPoints
        );

        if (GUI.Button(
            new Rect(15, 255, 175, 35),
            "+10 Points"))
        {
            stats.DebugAddPoints(10);
        }

        if (GUI.Button(
            new Rect(205, 255, 175, 35),
            "Reset Score"))
        {
            stats.DebugResetPoints();
        }
    }
    
    private void DrawBuildTab()
    {
        GUI.Label(
            new Rect(15, 80, 350, 25),
            "Build Mode Controls"
        );

        if (game == null)
        {
            GUI.Label(
                new Rect(15, 115, 350, 25),
                "Game manager not found!"
            );
            return;
        }

        GUI.Label(
            new Rect(15, 120, 350, 25),
            "Objects Placed: " + game.numOfPlacedObjects
        );

        if (GUI.Button(
                new Rect(15, 160, 365, 35),
                "Reset Placement Count"))
        {
            game.DebugResetPlacementCount();
        }

        GUI.Label(
            new Rect(15, 215, 365, 50),
            "Clear Placed Objects:"
        );
    }
}
