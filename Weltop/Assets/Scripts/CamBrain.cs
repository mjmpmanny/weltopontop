using JetBrains.Annotations;
using System.Dynamic;
using Unity.Cinemachine;
using UnityEngine;

public class CamBrain : MonoBehaviour
{
    private SplitScreenController con;
    public int channel;
    public int PlayerNum;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        con = GameObject.FindGameObjectWithTag("SplitScreenController").GetComponent<SplitScreenController>();
        SetChannel();
        con.AddPlayer(this);
    }

    public void ChangeDim(Vector2 pos, Vector2 size)
    {
        Rect rec = new Rect();
        rec.x = pos.x;
        rec.y = pos.y;
        rec.width = size.x;
        rec.height = size.y;
        GetComponent<Camera>().rect = rec;
    }

    public void SetChannel()
    {
        channel = con.GetChannel();
        transform.parent.GetComponentInChildren<CinemachineCamera>().OutputChannel = (OutputChannels)(1<<channel);
        GetComponent<CinemachineBrain>().ChannelMask = (OutputChannels)(1<<channel);
    }

    public void SetPlayer(int PlayerNumber)
    {
        PlayerNum = PlayerNumber;
        GetComponent<Camera>().cullingMask = (~(1 << PlayerNum + 6));
        GameObject model = transform.parent.GetComponentInChildren<PlayerController>().gameObject;
        model.layer = PlayerNum + 6;
        foreach (Transform child in model.GetComponentsInChildren<Transform>(true))
        {
            child.gameObject.layer = PlayerNum + 6;
        }
    }
}
