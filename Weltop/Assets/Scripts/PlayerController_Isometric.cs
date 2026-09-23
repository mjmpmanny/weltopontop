using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController_Isometric : MonoBehaviour
{
    //MOVE THESE TO A GLOBAL PARAMETERS CLASS AFTER TESTING
    public float speed;
    public float jumpPower;
    
    private Rigidbody rb;
    private GameObject player;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        Jump();
    }

    void Move()
    {
        Vector2 moveInput = InputSystem.actions["Move"].ReadValue<Vector2>();

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * moveInput.y + right * moveInput.x;

        rb.linearVelocity = new Vector3(
            move.x * speed,
            rb.linearVelocity.y,
            move.z * speed);
    }

    void Jump()
    {
        bool jumpPressed = InputSystem.actions["Jump"].triggered;

        if (jumpPressed)
        {
            Debug.Log("Pressed Jump");
            rb.AddForce(Vector3.up * jumpPower);
        }
    }
}
