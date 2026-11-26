using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Transitions : MonoBehaviour
{
    private Game game;
    private UI ui;

    public GameObject timerCountdownPanel;
    public GameObject parkourRoundPanel;
    public TMP_Text countdownText;

    public Coroutine currentTransitionCoroutine = null;

    private Audio audio;
    
    private void Awake()
    {
        audio = GetComponent<Audio>();
        ui = GetComponent<UI>();
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
        
        parkourRoundPanel.SetActive(false);
        timerCountdownPanel.SetActive(true);
        
        float timer = GlobalParameters.CountdownToGameSeconds;
        
        audio.PlayMusic(Audio.MusicType.ParkourRound);
        
        while (timer >= -1)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                countdownText.text = "GO!";
            }
            else
            {
                //countdownText.text = timer.ToString("N0");
                ui.UpdateParkourRoundTimer(timer);
            }
            yield return null;
        }
        
        parkourRoundPanel.SetActive(true);
        timerCountdownPanel.SetActive(false);
        
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
        
        float timer = GlobalParameters.StartRoundCountDownAtXSeconds;

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
