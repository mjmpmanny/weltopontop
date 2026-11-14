using System;
using System.Collections;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Shove : MonoBehaviour
{
    private GameObject player;
    
    public GameObject shoveArms;
    
    public Camera playerCam;
    
    public LayerMask layerMask;
    
    private Rigidbody rb;
    
    private bool canShove;
    public bool blockedByHeldItem;

    private PlayerInput inp;
    
    private PlayerController playerController;
    public PlayerAudio playerAudio;
    private PlayerItemInteraction itemInteraction;

    public Volume playerVolume;
    
    private void Awake()
    {
        inp = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();
        itemInteraction = GetComponent<PlayerItemInteraction>();
    }

    private void Start()
    {
        player = gameObject;
        rb = player.GetComponent<Rigidbody>();

        canShove = true;
    }

    private void Update()
    {
        TryShove();
    }

    private IEnumerator ShoveCooldown()
    {
        canShove = false;
        
        float time = GlobalParameters.ShoveCooldownSeconds;

        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        canShove = true;
    }

    private void TryShove()
    {
        bool shovePressed = inp.actions["Attack"].triggered;

        if (shovePressed)
        {
            if (!canShove || blockedByHeldItem || !playerController.canMove || (itemInteraction != null &&
                 (itemInteraction.IsHoldingItem || itemInteraction.UsedItemThisFrame)))
                return;
                
            transform.parent.GetComponentInChildren<Animator>().Play("Attack", 1);
            OperateShoveHands();
            
            GameObject playerToShove = FindPlayerToShove();

            if (ApplyShoveHit(playerToShove, playerCam.transform.forward))
                playerAudio.PlaySoundEffect(PlayerAudio.PlayerSoundEffect.Shove);

            StartCoroutine(ShoveCooldown());
        }
    }

    private void OperateShoveHands()
    {
        shoveArms.SetActive(true);
        shoveArms.transform.DOLocalMoveZ(1, 0.25f)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => shoveArms.SetActive(false));
    }

    private GameObject FindPlayerToShove()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward,
                out hit, GlobalParameters.ShoveRaycastDistance, layerMask))
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }

    public static bool ApplyShoveHit(
        GameObject target,
        Vector3 direction,
        float strengthMultiplier = 1f)
    {
        if (target == null || direction.sqrMagnitude < 0.0001f)
            return false;

        PlayerStats stats = target.GetComponentInParent<PlayerStats>();
        if (stats == null || stats.isInvincible)
            return false;

        RagdollController ragdoll = stats.GetComponent<RagdollController>();
        ViewSwitcher view = stats.GetComponent<ViewSwitcher>();
        if (ragdoll == null || view == null || ragdoll.isRagdolled || ragdoll.isGetingUp)
            return false;

        Vector3 force = direction.normalized *
                        (GlobalParameters.ShovePower * strengthMultiplier);

        ragdoll.Ragdoll(stats.transform.position, force);
        if (!ragdoll.isRagdolled)
            return false;

        view.ShakeCams(
            GlobalParameters.ShoveCameraShakeDurationSeconds,
            GlobalParameters.ShoveCameraShakeStrength);

        return true;
    }
    
    private IEnumerator RecoverFromShove(PlayerController shoveTarget)
    {
        Shove targetShove = shoveTarget.gameObject.GetComponent<Shove>();
        //shoveTarget.canMove = false;
        yield return new WaitForSeconds(GlobalParameters.RecoverAfterShoveDelaySeconds);
        //StartCoroutine(RemoveShovePostProcessing(targetShove));
        //shoveTarget.canMove = true;
    }
}

#region Legacy Shove System for Shared Camera
/*
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shove : MonoBehaviour
{
    private Vector3 lastDirection;

    private GameObject player;

    public LayerMask layerMask;
    
    private Rigidbody rb;
    
    private bool canShove;

    private void Start()
    {
        player = gameObject;
        rb = player.GetComponent<Rigidbody>();

        canShove = true;
    }

    private void Update()
    {
        TryShove();
    }

    private IEnumerator ShoveCooldown()
    {
        canShove = false;
        
        float time = GlobalParameters.ShoveCooldownSeconds;

        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        canShove = true;
    }

    private void FixedUpdate()
    {
        CalculateFacingDirection();
    }

    //Made to work with the shared camera. Should change if we move to FP or TP
    private void CalculateFacingDirection()
    {
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            lastDirection = rb.linearVelocity.normalized;
        }
    }

    private void TryShove()
    {
        bool shovePressed = InputSystem.actions["Attack"].triggered;

        if (shovePressed)
        {
            if (!canShove)
                return;
                
            Debug.Log("Shove Pressed");
            GameObject playerToShove = FindPlayerToShove();
            
            if (playerToShove != null)
            {
                ExecuteShove(playerToShove);
            }

            StartCoroutine(ShoveCooldown());
        }
    }

    private GameObject FindPlayerToShove()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(lastDirection), out hit, 2, layerMask))
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }

    private void ExecuteShove(GameObject shoveTarget)
    {
        Rigidbody rigidBody = shoveTarget.GetComponent<Rigidbody>();
        Debug.Log("added force");
        rigidBody.AddForce(lastDirection * GlobalParameters.ShovePower);
    }
}
*/
#endregion