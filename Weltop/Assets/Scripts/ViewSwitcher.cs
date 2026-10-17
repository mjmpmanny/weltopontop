using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Experimental.Animations;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class ViewSwitcher : MonoBehaviour
{
    public PlayerController playerController;
    public BuildModeController BuildMode;

    public GameObject TestObjectToHoldOnTo; // test only

    private bool playerview;
    private CinemachineCamera cam;
    private OutputChannels channel;
    private System.Action<InputAction.CallbackContext> testbm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        cam = transform.parent.GetComponentInChildren<CinemachineCamera>();
        BuildMode = transform.parent.GetComponentInChildren<BuildModeController>();
        playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        testbm = TestEnterBuldMode;
        GetComponent<PlayerInput>().actions["Next"].started += testbm;
    }

    public void TestEnterBuldMode(CallbackContext ctx)// for testing only
    {
        SwitchToBuild(TestObjectToHoldOnTo, new Vector3(0,10,0));
    }

    public void SwitchToBuild(GameObject obj, Vector3 StartPos)
    {
        GameObject nobj = Instantiate(obj);
        BuildMode.SetController(nobj, 10, StartPos);
        playerController.canMove = false;
        BuildMode.active = true;
        cam.Priority = 0;
    }
    public void SwitchToPlayer()
    {
        BuildMode.SetController(null,0,new Vector3(0,-1000,0));
        playerController.canMove = true;
        BuildMode.active = false;
        cam.Priority = 10;
    }
    private void OnDestroy()
    {
        GetComponent<PlayerInput>().actions["Next"].started -= testbm;
    }

}
