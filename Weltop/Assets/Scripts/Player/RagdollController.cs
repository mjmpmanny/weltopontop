using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class RagdollController : MonoBehaviour
{
    [SerializeField]
    private Vector3 _testRagdollHitFrom;
    [SerializeField]
    private bool _testRagdoll;
    private PlayerController playerCon;
    private ViewSwitcher viewSwitcher;
    private Rigidbody rb;
    private Collider playerCollider;
    private CamBrain camBrain;
    [SerializeField]
    private PlayerAnimationController playerAnimationController;
    [SerializeField]
    private float RagdollTime;
    private float RagdollTimer;
    public bool isRagdolled;
    public bool isGetingUp;
    public Cam3rdPerson ragdollCam;
    private void Start()
    {
        camBrain = transform.parent.GetComponentInChildren<CamBrain>();
        rb = GetComponent<Rigidbody>();
        viewSwitcher = GetComponent<ViewSwitcher>();
        playerCollider = GetComponent<Collider>();
        playerCon = GetComponent<PlayerController>();
    }
    private void Update()
    {
        if (_testRagdoll)
        {
            _testRagdoll = false;
            Ragdoll(transform.position, _testRagdollHitFrom);
        }

        if (isRagdolled)
        {
            RagdollTimer -= Time.deltaTime;
            if (RagdollTimer < 0) GetUp();
        } 
    }
    public void Ragdoll(Vector3 hitpoint, Vector3 force)
    {
        if (viewSwitcher.SwitchToRagdoll())
        {
            isRagdolled = true;
            RagdollTimer = RagdollTime;
            playerAnimationController.TriggerRagdoll(hitpoint, force,rb.linearVelocity);
            playerCollider.excludeLayers = 960;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            playerCon.canMove = false;
        }
    }

    public void GetUp()
    {
        if ( isRagdolled)
        {
        isRagdolled = false;
        isGetingUp = true;
        playerAnimationController.Getup();
        playerCollider.excludeLayers = 0;
        rb.useGravity = true;
            ragdollCam.SwitchedTo();
        }
    }

    public void HasGotup()
    {
        isGetingUp = false;
        viewSwitcher.SwitchFromRagdoll();

    }
    public void MovePlayer(Vector3 pos)
    {
        if (isRagdolled) 
        {
            playerAnimationController.MoveHip(pos);
        } else if (!isGetingUp)
        {
            transform.position = pos;
        } else
        {
            playerAnimationController.transform.position = pos;

        }
    }
    public void RotatePlayer(quaternion rot)
    {
        if (isRagdolled)
        {
            playerAnimationController.RotateHip(rot);
        } else if (!isGetingUp)
        {
            transform.rotation = rot;
        } else
        {
            playerAnimationController.transform.rotation = rot;
        }
    }

    public Rigidbody CurrentRigidBody()
    {
        if (isRagdolled)
        {
            return playerAnimationController.GetHipRigidbody();
        } else if (!isGetingUp)
        {
            return rb;
        }
        Debug.LogError("Tried to get rigidbody when getting up.");
        return null;
        
    }

}
