using JetBrains.Annotations;
using NUnit.Framework;
using System.Dynamic;
using Unity.Cinemachine;
using Unity.Collections.Tests.CoreCLR.TestJobs;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CamBrain : MonoBehaviour
{
    private SplitScreenController con;
    [HideInInspector]
    public int channel;
    [HideInInspector]
    public int PlayerNum;
    private int CullMask;
    private int DisabledCullMask;
    [Header("Hide From Other Players")]
    public GameObject[] HideFromOtherPlayers;

    
    private Camera camera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        camera = GetComponent<Camera>();
    }
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
        camera.rect = rec;
    }

    public void SetChannel()
    {
        channel = con.GetChannel();
        CinemachineCamera[] cams = transform.parent.GetComponentsInChildren<CinemachineCamera>();
        foreach (CinemachineCamera cam in cams)
        {
            cam.OutputChannel = (OutputChannels)(1 << channel);
        }

        GetComponent<CinemachineBrain>().ChannelMask = (OutputChannels)(1<<channel);
    }

    public void SetPlayer(int PlayerNumber)
    {
        PlayerNum = PlayerNumber;
        CullMask = ~((1 << PlayerNum + 6) + ((15 << 12) - (1 << PlayerNum + 12)));
        DisabledCullMask = ~((15 << 12) - (1 << PlayerNum + 12));
        EnableCullingMask();
        GameObject model = transform.parent.GetComponentInChildren<Animator>().gameObject;
        GameObject player = transform.parent.GetComponentInChildren<PlayerController>().gameObject;
        player.layer = PlayerNum + 6;
        foreach (Transform child in model.GetComponentsInChildren<Transform>(true))
        {
            child.gameObject.layer = PlayerNum + 6;
        }
        foreach (GameObject hidden in HideFromOtherPlayers)
        {
            foreach(Transform child in hidden.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = PlayerNum + 12;

            }
        }
    }
    public void DisableCullingMask()
    {

        camera.cullingMask = DisabledCullMask;
    }
    public void EnableCullingMask()
    {

        camera.cullingMask = CullMask;
    }
    public void EnableFullCullingMask()
    {

        camera.cullingMask = ~((15 <<  6) + (15 << 12));
    }
}
