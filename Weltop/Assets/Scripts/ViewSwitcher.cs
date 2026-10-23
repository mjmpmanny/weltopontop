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
    private PlayerController playerController;
    private BuildModeController BuildMode;

    public GameObject TestObjectToHoldOnTo; // test only

    private CinemachineCamera cam;
    private System.Action<InputAction.CallbackContext> testbm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = transform.parent.GetComponentInChildren<CinemachineCamera>();
        BuildMode = transform.parent.GetComponentInChildren<BuildModeController>();
        playerController = GetComponent<PlayerController>();
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
        cam.Priority = 0;
    }
    public void SwitchToPlayer()
    {
        BuildMode.SetController(null,0,new Vector3(0,-1000,0));
        playerController.canMove = true;
        cam.Priority = 10;
    }
    private void OnDestroy()
    {
        GetComponent<PlayerInput>().actions["Next"].started -= testbm;
    }

}
