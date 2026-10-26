using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Cam3rdPerson : MonoBehaviour
{
    private float pitch;
    private float Camspeed;
    private PlayerInput input;
    public ViewSwitcher ViewSwitcher;
    public Transform follow;

    private void Start()
    {
        input = ViewSwitcher.GetPlayerInput();
        Camspeed = ViewSwitcher.GetCamSpeed();
    }
    void Update()
    {
        Vector2 lookinput = input.actions["Look"].ReadValue<Vector2>();

        pitch = Mathf.Clamp(pitch - lookinput.y * Camspeed, -80f, 80f);

        transform.rotation = Quaternion.Euler(
            pitch,
            transform.rotation.eulerAngles.y + lookinput.x * Camspeed,
            0
        );
    }
    private void FixedUpdate()
    {
        transform.position = follow.transform.position + Vector3.up * 0.1f;
    }
}
