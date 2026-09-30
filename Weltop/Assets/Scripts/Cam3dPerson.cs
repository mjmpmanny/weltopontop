using Unity.Collections.Tests.CoreCLR.TestJobs;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cam3dPerson : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speed;
    public float ControllerSpeed;
    public float KeyboardSpeed;
    private PlayerInput input;
    // Use this for initialization
    void Start()
    {
        input = transform.parent.GetComponentInChildren<PlayerInput>();
        speed = (input.devices[0] is Gamepad) ? ControllerSpeed : KeyboardSpeed;
    }

    void FixedUpdate()
    {
        Vector2 lookinput = input.actions["Look"].ReadValue<Vector2>();
        
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x - lookinput.y*speed, 
            transform.rotation.eulerAngles.y + lookinput.x*speed, 0);
        transform.position = transform.parent.GetComponentInChildren<PlayerController_3dPerson>().transform.position;
    }
}
