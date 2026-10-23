using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Transitions : MonoBehaviour
{
    private Game game;

    public GameObject startGamePanel;
    public TMP_Text countdownText;
    
    private void Awake()
    {
        game = GetComponent<Game>();
    }

    private void Start()
    {
        StartCoroutine(StartGame());
    }

    public IEnumerator StartGame()
    {
        game.gameIsActive = false;
        startGamePanel.gameObject.SetActive(true);

        float timer = GlobalParameters.CountdownToGameSeconds;

        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            countdownText.text = timer.ToString("N0");
            yield return null;
        }
        
        startGamePanel.gameObject.SetActive(false);
        
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

    public IEnumerator TransitionToParkour()
    {
        float timer = GlobalParameters.CountdownToGameSeconds;

        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            countdownText.text = timer.ToString("N0");
            yield return null;
        }
    }

    public IEnumerator TransitionToBuild()
    {
        yield return null;
    }
}
