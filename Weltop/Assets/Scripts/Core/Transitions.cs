using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Transitions : MonoBehaviour
{
    private Game game;

    public GameObject timerCountdownPanel;
    public TMP_Text countdownText;

    public Coroutine currentTransitionCoroutine = null;
    
    private void Awake()
    {
        game = GetComponent<Game>();
    }
    

    public IEnumerator StartParkourRound()
    {
        game.ExecuteCodeOnAllPlayerControllers(controller =>
        {
            controller.canMove = false;
        });
        
        game.ExecuteCodeOnAllPlayerCameras(camera =>
        {
            camera.canLook = GlobalParameters.CanPlayerLookDuringRoundStartCountdown;
        });
        
        game.gameIsActive = false;
        timerCountdownPanel.gameObject.SetActive(true);

        float timer = GlobalParameters.CountdownToGameSeconds;

        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            if (timer < 1)
            {
                countdownText.text = "GO!";
            }
            else
            {
                countdownText.text = timer.ToString("N0");
            }
            yield return null;
        }
        
        timerCountdownPanel.gameObject.SetActive(false);
        
        game.ExecuteCodeOnAllPlayerControllers(controller =>
        {
            controller.canMove = true;
        });
        
        game.ExecuteCodeOnAllPlayerCameras(camera =>
        {
            camera.canLook = true;
        });

        game.gameIsActive = true;
        currentTransitionCoroutine = null;
    }

    public void StartTransitionToNextRound()
    {
        currentTransitionCoroutine = StartCoroutine(TransitionToOtherRound());
    }

    public void StopTransitionCoroutine()
    {
        if (currentTransitionCoroutine != null)
        {
            timerCountdownPanel.SetActive(false);
            StopCoroutine(currentTransitionCoroutine);
            currentTransitionCoroutine = null;
            game.currentlyInTransition = false;
        }
    }

    private IEnumerator TransitionToOtherRound()
    {
        timerCountdownPanel.gameObject.SetActive(true);
        
        float timer = GlobalParameters.StartRoundCountDownAtXSeconds + 1;

        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            //Countdown text is handled in UI.cs
            yield return null;
        }

        timerCountdownPanel.gameObject.SetActive(false);
        
        game.EndRound();
        
        game.currentlyInTransition = false;
        currentTransitionCoroutine = null;
    }
}
