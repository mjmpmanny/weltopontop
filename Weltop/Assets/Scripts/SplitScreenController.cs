using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class SplitScreenController : MonoBehaviour
{
    public List<CamBrain> players = new List<CamBrain>();
    private int freeChannel = 0;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    public void AddPlayer(CamBrain cam)
    {
        players.Add(cam);
        switch (players.Count)
        {
            case 1:
                players[0].ChangeDim(Vector2.zero,Vector2.one);
                break;
            case 2:
                players[0].ChangeDim(Vector2.zero, new Vector2(0.5f,1));
                players[1].ChangeDim(new Vector2(0.5f,0), new Vector2(0.5f, 1));
                break;
            case 3:
                players[0].ChangeDim(Vector2.zero, new Vector2(1f, 0.5f));
                players[1].ChangeDim(new Vector2(0, 0.5f), new Vector2(0.5f, 0.5f));
                players[2].ChangeDim(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
                break;
            case 4:
                players[0].ChangeDim(Vector2.zero, new Vector2(0.5f, 0.5f));
                players[1].ChangeDim(new Vector2(0, 0.5f), new Vector2(0.5f, 0.5f));
                players[2].ChangeDim(new Vector2(0.5f, 0), new Vector2(0.5f, 0.5f));
                players[3].ChangeDim(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
                break;
        }
    }

    public int GetChannel()
    {
        return ++freeChannel;
    }
}
