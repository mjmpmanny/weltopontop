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