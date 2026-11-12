using System;
using System.Collections;
using UnityEngine;
using Random = System.Random;

public class HillMover : MonoBehaviour
{
    public GameObject hill;

    public Transform[] possibleSpawnLocations;

    private Game game;
    private UI ui;

    private int previousChoice;
    
    private void Awake()
    {
        game = GetComponent<Game>();
        ui = GetComponent<UI>();
    }

    //Game calls this script, which automatically starts the timing and everything.
    public void StartHillRoutine()
    {
        StartCoroutine(HillMovementTimer());
    }

    private IEnumerator HillMovementTimer()
    {
        RemoveHill();

        float delayedTimer = GlobalParameters.DelayBetweenMovingHillSeconds;

        while (delayedTimer >= 0f)
        {
            if (game.currentRoundState != Game.RoundState.Parkour)
            {
                RemoveHill();
                yield break;
            }

            delayedTimer -= Time.deltaTime;
            yield return null;
        }

        SpawnHillRandom();

        float timer = GlobalParameters.HowLongTheHillStaysSeconds;
        ui.UpdateHillMoverTimer(timer);

        // Show the hill during the opening countdown and hold its timer at 20.
        while (!game.gameIsActive)
        {
            if (game.currentRoundState != Game.RoundState.Parkour)
            {
                RemoveHill();
                yield break;
            }

            yield return null;
        }

        // Count down once players can move.
        while (timer > 0f)
        {
            if (!game.gameIsActive ||
                game.currentRoundState != Game.RoundState.Parkour)
            {
                RemoveHill();
                yield break;
            }

            ui.UpdateHillMoverTimer(timer);
            timer -= Time.deltaTime;
            yield return null;
        }

        StartHillRoutine();
    }

    private void SpawnHillRandom()
    {
        hill.SetActive(true);

        int choice = UnityEngine.Random.Range(0, possibleSpawnLocations.Length);

        if (choice == previousChoice)
        {
            if (previousChoice == possibleSpawnLocations.Length - 1)
            {
                choice = previousChoice - 1;
            }
            else
            {
                choice = previousChoice + 1;
            }
        }
        
        hill.transform.position = possibleSpawnLocations[choice].position;
        previousChoice = choice;
    }

    private void RemoveHill()
    {
        //Make sure everyone stops recieving points when its exited
        game.ExecuteCodeOnAllPlayerStats(stats => stats.PlayerExitHill());
        ui.HillMovingText();
        hill.SetActive(false);
    }
}
