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

    private float pitch;

    // Use this for initialization
    void Start()
    {
        input = transform.parent.GetComponentInChildren<PlayerInput>();
        speed = (input.devices[0] is Gamepad) ? ControllerSpeed : KeyboardSpeed;

        pitch = transform.eulerAngles.x;
        if (pitch > 180) pitch -= 360;
    }

    void FixedUpdate()
    {
        Vector2 lookinput = input.actions["Look"].ReadValue<Vector2>();

        pitch = Mathf.Clamp(pitch - lookinput.y*speed, -80f, 80f);

        transform.rotation = Quaternion.Euler(pitch,
            transform.rotation.eulerAngles.y + lookinput.x*speed, 0);
        transform.position = transform.parent.GetComponentInChildren<PlayerController_3dPerson>().transform.position;
    }
}