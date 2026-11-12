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
    public Quaternion rotation;
    private void Start()
    {
        input = ViewSwitcher.GetPlayerInput();
    }
    void Update()
    {
        Vector2 lookinput = input.actions["Look"].ReadValue<Vector2>() * (input.devices[0] is Gamepad ? Time.deltaTime : 1);

        pitch = Mathf.Clamp(pitch - lookinput.y * Camspeed, -80f, 80f);

        rotation = Quaternion.Euler(
            pitch,
            rotation.eulerAngles.y + lookinput.x * Camspeed,
            0
        );
        transform.rotation = rotation;
    }
    private void FixedUpdate()
    {
        transform.position = follow.transform.position + Vector3.up * 0.5f;
    }
    public void SwitchTo()
    {
        cam.Priority = 10;
        pitch = 45;
    }
    public void SwitchFrom()
    {
        cam.Priority = 0;
    }
    public void SetSensitivity(float _Sensitivity)
    {
        Camspeed = _Sensitivity;
    }

    public void RotateTo(Quaternion rot)
    {
        rotation = rot;
        pitch = 45;
    }
}
