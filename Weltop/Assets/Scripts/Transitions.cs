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
    
    private void Awake()
    {
        game = GetComponent<Game>();
    }

    private void Start()
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        game.gameIsActive = false;
        timerCountdownPanel.gameObject.SetActive(true);

        float timer = GlobalParameters.CountdownToGameSeconds;

        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            countdownText.text = timer.ToString("N0");
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
        Debug.Log("Start Game!");
    }

    public void StartTransitionToNextRound()
    {
        StartCoroutine(TransitionToOtherRound());
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
    }
}
