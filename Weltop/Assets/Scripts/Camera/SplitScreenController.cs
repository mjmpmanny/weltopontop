using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class SplitScreenController : MonoBehaviour
{
    public List<CamBrain> players = new List<CamBrain>();
    private int freeChannel = 0;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    public void AddPlayer(CamBrain cam)
    {
        PlayerInput input = cam.transform.parent.GetComponentInChildren<PlayerInput>();

        if (input == null)
        {
            Debug.LogError("PlayerInput not found for split-screen camera!");
            return;
        }

        int playerIndex = input.playerIndex;

        while (players.Count <= playerIndex)
        {
            players.Add(null);
        }

        players[playerIndex] = cam;

        int playerCount = PlayerInput.all.Count;

        if (playerCount != players.Count)
            return;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == null)
                return;

            players[i].SetPlayer(i);
        }

        switch (playerCount)
        {
            case 1:
                players[0].ChangeDim(Vector2.zero, Vector2.one);
                break;

            case 2:
                players[0].ChangeDim(Vector2.zero, new Vector2(0.5f, 1));
                players[1].ChangeDim(new Vector2(0.5f, 0), new Vector2(0.5f, 1));
                break;

            case 3:
                players[0].ChangeDim(new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f));
                players[1].ChangeDim(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
                players[2].ChangeDim(Vector2.zero, new Vector2(1f, 0.5f));
                break;
            
            case 4:
                players[0].ChangeDim(new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f));
                players[1].ChangeDim(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
                players[2].ChangeDim(Vector2.zero, new Vector2(0.5f, 0.5f));
                players[3].ChangeDim(new Vector2(0.5f, 0f), new Vector2(0.5f, 0.5f));
                break;

        }
    }

    public int GetChannel() 
    {
        return ++freeChannel;
    }
}
