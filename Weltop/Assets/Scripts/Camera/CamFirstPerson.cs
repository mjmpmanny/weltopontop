using UnityEngine;
using UnityEngine.InputSystem;

public class CamFirstPerson : MonoBehaviour
{
    private float speed;
    public float ControllerSpeed;
    public float KeyboardSpeed;
    private PlayerInput input;

    private bool usingController;
    private float pitch;
    public bool canLook = true;

    void Start()
    {
        canLook = false;

        input = transform.parent.GetComponentInChildren<PlayerInput>();

        usingController = input.devices.Count > 0 &&
                          input.devices[0] is Gamepad;

        speed = usingController ? ControllerSpeed : KeyboardSpeed;

        pitch = transform.eulerAngles.x;
        if (pitch > 180) pitch -= 360;
    }

    void Update()
    {
        if (!canLook)
            return;

        Vector2 lookinput = input.actions["Look"].ReadValue<Vector2>();

        // Gamepad stick input is continuous and must be
        // scaled by elapsed time.
        // Mouse delta input should not be scaled this way.
        float multiplier = usingController ? Time.deltaTime : 1f;

        float horizontalLook = lookinput.x * speed * multiplier;
        float verticalLook = lookinput.y * speed * multiplier;

        pitch = Mathf.Clamp(
            pitch - verticalLook,
            -80f,
            80f
        );

        transform.rotation = Quaternion.Euler(
            pitch,
            transform.rotation.eulerAngles.y + horizontalLook,
            0f
        );

        transform.position = transform.parent
            .GetComponentInChildren<PlayerController>()
            .transform.position;
    }
}