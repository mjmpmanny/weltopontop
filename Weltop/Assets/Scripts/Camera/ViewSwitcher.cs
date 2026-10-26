using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using Unity.Collections.Tests.CoreCLR.TestJobs;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Experimental.Animations;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static UnityEngine.InputSystem.InputAction;

public class ViewSwitcher : MonoBehaviour
{
    private PlayerController playerController;
    private BuildModeController BuildMode;
    private CamBrain camBrain;

    private float Camspeed;
    public float CamControllerSpeed;
    public float CamKeyboardSpeed;
    private PlayerInput playerInput;

    public GameObject TestObjectToHoldOnTo; // test only



    private enum camState
    {
        Parkour,
        Ragdoll,
        Build
    }
    private camState state;
    private CinemachineCamera cam;
    public CinemachineCamera RagdollCam;
    private System.Action<InputAction.CallbackContext> testbm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {

        cam = transform.parent.GetComponentInChildren<CinemachineCamera>();
        BuildMode = transform.parent.GetComponentInChildren<BuildModeController>();
        camBrain = transform.parent.GetComponentInChildren<CamBrain>();
        playerController = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
        Camspeed = (playerInput.devices[0] is Gamepad) ? CamControllerSpeed : CamKeyboardSpeed;
    }

    private void Start()
    {
        testbm = TestEnterBuldMode;
        //GetComponent<PlayerInput>().actions["Next"].started += testbm;
    }

    public void TestEnterBuldMode(CallbackContext ctx)// for testing only
    {
        SwitchToBuild(TestObjectToHoldOnTo, new Vector3(0,10,0));
    }

    public void SwitchToBuild(GameObject obj, Vector3 StartPos)
    {
        state = camState.Build;
        GameObject nobj = Instantiate(obj);
        BuildMode.SetController(nobj, 10, StartPos);
        playerController.canMove = false;
        cam.Priority = 0;
    }
    public void SwitchToPlayer()
    {
        state = camState.Parkour;
        BuildMode.SetController(null,0,new Vector3(0,-1000,0));
        playerController.canMove = true;
        RagdollCam.Priority = 0;
        cam.Priority = 10;
        camBrain.EnableCullingMask();


    }

    public bool SwitchToRagdoll()
    {
        if (state != camState.Build) {
            state = camState.Ragdoll;
            cam.Priority = 0;
            RagdollCam.Priority = 10;
            camBrain.DisableCullingMask();
            return true;
        }
        return false;
    }


    public float GetCamSpeed()
    {
        return Camspeed;
    }

    public PlayerInput GetPlayerInput()
    {
        return playerInput;
    }
    private void OnDestroy()
    {
        //GetComponent<PlayerInput>().actions["Next"].started -= testbm;
    }

}
