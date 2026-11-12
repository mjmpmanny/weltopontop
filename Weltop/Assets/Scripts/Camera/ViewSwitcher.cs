using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class ViewSwitcher : MonoBehaviour
{
    private PlayerController playerController;
    private BuildModeController BuildMode;
    private CamBrain camBrain;

    [Header("Sensitivity Settings")]
    private float Camspeed;
    public float CamControllerSpeed;
    public float CamKeyboardSpeed;
    private PlayerInput playerInput;


    [Header("Cameras to Shake")]
    public CamShake[] camShakes;

    [Header("PostProccesing Volumes")]
    public Volume[] PostProccesingVolumes;


    public GameObject TestObjectToHoldOnTo; // test only



    private enum camState
    {
        Parkour,
        Ragdoll,
        Build
    }
    private camState state;
    [Header("Cameras")]
    private ICamera cam;
    public Cam3rdPerson RagdollCam;
    private ICamera CurCam;
    public Action OnBuildMode;
    public Action OnRagdoll;
    public Action OnParkor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {

        cam = transform.parent.GetComponentInChildren<ICamera>();
        BuildMode = transform.parent.GetComponentInChildren<BuildModeController>();
        camBrain = transform.parent.GetComponentInChildren<CamBrain>();
        playerController = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
        Camspeed = (playerInput.devices[0] is Gamepad) ? CamControllerSpeed : CamKeyboardSpeed;
    }

    private void Start()
    {
        foreach (ICamera c in transform.parent.GetComponentsInChildren<ICamera>())
        {
            c.SetSensitivity(Camspeed);
        }
    }


    public void SwitchToBuild(GameObject obj, Vector3 startPos)
    {
        OnBuildMode.Invoke();
        state = camState.Build;
        SwitchFromLastCam();
    
        GameObject buildObject = Instantiate(obj);
        BuildMode.SetController(buildObject, 10, startPos);
    
        CurCam = BuildMode;
        playerController.canMove = false;
    }
    public void SwitchToPlayer()
    {
        OnParkor.Invoke();
        state = camState.Parkour;
        SwitchFromLastCam();
        playerController.canMove = true;
        cam.SwitchTo();
        CurCam = cam;
        camBrain.EnableCullingMask();


    }

    public bool SwitchToRagdoll()
    {
        OnRagdoll.Invoke();
        if (state != camState.Build) {
            state = camState.Ragdoll;
            SwitchFromLastCam();
            RagdollCam.SwitchTo();
            CurCam = RagdollCam;
            camBrain.DisableCullingMask();
            return true;
        }
        return false;
    }
    public void SwitchFromRagdoll()
    {
        if (state != camState.Build)
        {
            SwitchToPlayer();
        }
    }

    private void SwitchFromLastCam()
    {
        CurCam?.SwitchFrom();
    }
    public float GetSensitivity()
    {
        return Camspeed;
    }

    public PlayerInput GetPlayerInput()
    {
        return playerInput;
    }

    public void ShakeCams(float duration,float strength)
    {
        foreach (CamShake cs in camShakes)
        {
            cs.ShakeCamera(duration, strength);
        }
    }


}
