using System;
using UnityEngine;

public class Transitions : MonoBehaviour
{
    private Game game;

    private void Awake()
    {
        game = GetComponent<Game>();
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        game.ExecuteCodeOnAllPlayerControllers(controller =>
        {
            controller.canMove = true;
        });
        
        game.ExecuteCodeOnAllPlayerCameras(camera =>
        {
            camera.canLook = true;
        });
    }

    public void TransitionToParkour()
    {
        
    }

    public void TransitionToBuild()
    {
        
    }
}
