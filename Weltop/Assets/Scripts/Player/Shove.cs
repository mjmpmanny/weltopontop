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

    private PlayerInput inp;
    
    public PlayerAudio playerAudio;

    public Volume playerVolume;
    
    private void Awake()
    {
        inp = GetComponent<PlayerInput>();
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
            if (!canShove)
                return;
                
            transform.parent.GetComponentInChildren<Animator>().Play("Attack", 1);
            OperateShoveHands();
            
            GameObject playerToShove = FindPlayerToShove();
            
            if (playerToShove != null)
            {
                if(CanBeShoved(playerToShove));
                {
                    playerAudio.PlaySoundEffect(PlayerAudio.PlayerSoundEffect.Shove);
                    ExecuteShove(playerToShove);
                }
            }

            StartCoroutine(ShoveCooldown());
        }
    }

    private void OperateShoveHands()
    {
        shoveArms.SetActive(true);
        shoveArms.transform.DOLocalMoveZ(2, 0.25f)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => shoveArms.SetActive(false));
    }

    private GameObject FindPlayerToShove()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerCam.transform.forward, out hit, 2, layerMask))
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }

    private bool CanBeShoved(GameObject shoveTarget)
    {
        PlayerStats targetStats = shoveTarget.GetComponent<PlayerStats>();

        return !targetStats.isInvincible;
    }

    private void ExecuteShove(GameObject shoveTarget)
    {
        Rigidbody rigidBody = shoveTarget.GetComponent<Rigidbody>();
        CamShake targetCameraShake = shoveTarget.GetComponent<CamShake>();
        Shove targetShoveCode = shoveTarget.GetComponent<Shove>();

        StartCoroutine(ApplyShovePostProcessing(targetShoveCode));
        //rigidBody.AddForce(playerCam.transform.forward * GlobalParameters.ShovePower);

        shoveTarget.transform.parent.GetComponentInChildren<PlayerAnimationController>().TriggerRagdoll(playerCam.transform.forward * GlobalParameters.ShovePower, shoveTarget.transform.position);

        targetCameraShake.ShakeCamera(GlobalParameters.ShoveCameraShakeDurationSeconds, GlobalParameters.ShoveCameraShakeStrength);

        StartCoroutine(RecoverFromShove(shoveTarget.GetComponent<PlayerController>()));
    }

    IEnumerator ApplyShovePostProcessing(Shove shoveTarget)
    {
        float time = 0;
        Volume targetVolume = shoveTarget.playerVolume;
        
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            targetVolume.weight += time * 2;
            yield return null;
}

        targetVolume.weight = 1;
    }
    
    IEnumerator RemoveShovePostProcessing(Shove shoveTarget)
    {
        float time = 0;
        Volume targetVolume = shoveTarget.playerVolume;
        
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            targetVolume.weight -= time * 2;
            yield return null;
        }

        targetVolume.weight = 0;
    }

    private IEnumerator RecoverFromShove(PlayerController shoveTarget)
    {
        Shove targetShove = shoveTarget.gameObject.GetComponent<Shove>();
        shoveTarget.canMove = false;
        yield return new WaitForSeconds(GlobalParameters.RecoverAfterShoveDelaySeconds);
        StartCoroutine(RemoveShovePostProcessing(targetShove));
        shoveTarget.canMove = true;
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