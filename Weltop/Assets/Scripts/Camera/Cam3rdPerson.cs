using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cam3rdPerson : MonoBehaviour , ICamera
{
    private float pitch;
    private float Camspeed;
    private PlayerInput input;
    public ViewSwitcher ViewSwitcher;
    public Transform follow;
    public CinemachineCamera cam;

    private void Start()
    {
        input = ViewSwitcher.GetPlayerInput();
    }
    void Update()
    {
        Vector2 lookinput = input.actions["Look"].ReadValue<Vector2>() * (input.devices[0] is Gamepad ? Time.deltaTime : 1);

        pitch = Mathf.Clamp(pitch - lookinput.y * Camspeed, -80f, 80f);

        transform.rotation = Quaternion.Euler(
            pitch,
            transform.rotation.eulerAngles.y + lookinput.x * Camspeed,
            0
        );

    }
    private void FixedUpdate()
    {
        transform.position = follow.transform.position + Vector3.up * 0.5f;
    }
    public void SwitchTo()
    {
        pitch = 45;
        cam.Priority = 10;
    }
    public void SwitchFrom()
    {
        cam.Priority = 0;
    }
    public void SetSensitivity(float _Sensitivity)
    {
        Camspeed = _Sensitivity;
    }
}
