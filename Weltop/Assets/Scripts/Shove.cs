using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shove : MonoBehaviour
{
    private Vector3 lastDirection;

    private GameObject player;

    public LayerMask layerMask;
    
    private Rigidbody rb;

    private void Start()
    {
        player = gameObject;
        rb = player.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        TryShove();
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
            Debug.Log("Shove Pressed");
            GameObject playerToShove = FindPlayerToShove();
            
            if (playerToShove != null)
            {
                ExecuteShove(playerToShove);
            }
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
