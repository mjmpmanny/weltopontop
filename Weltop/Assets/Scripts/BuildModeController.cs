using Unity.Cinemachine;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.InputAction;

public class BuildModeController : MonoBehaviour
{

    public float speed;

    private float Camspeed;
    public float CamControllerSpeed;
    public float CamKeyboardSpeed;
    private float pitch;
    private PlayerInput input;

    public CinemachineCamera Cam;
    public PlaceableObject follow;
    public bool active = false;

    public bool testMode = true;

    private Rigidbody rb;
    private System.Action<InputAction.CallbackContext> _place;
    private System.Action<InputAction.CallbackContext> _rotate;
    void Start()
    {
        input = transform.parent.transform.parent.GetComponentInChildren<PlayerInput>();
        Cam = transform.parent.GetComponentInChildren<CinemachineCamera>();
        Camspeed = (input.devices[0] is Gamepad) ? CamControllerSpeed : CamKeyboardSpeed;
        rb = GetComponent<Rigidbody>();
        pitch = transform.eulerAngles.x;
        if (pitch > 180) pitch -= 360;


        _place = Place;
        _rotate = Rotate;
        input.actions["Place"].started += _place;
        input.actions["Rotate"].started += _rotate;
    }
    private void OnDestroy()
    {
        input.actions["Place"].started -= _place;
        input.actions["Rotate"].started -= _rotate;

    }

    public void SetController(GameObject obj, int camPriority,Vector3 location)
    {
        transform.position = location;
        Cam.Priority = camPriority;
        if (obj != null) {
            follow = obj.GetComponent<PlaceableObject>();
            active = true;
        } else
        {
            follow = null;
            active = false;
        }
    }

    public void Place(CallbackContext ctx)
    {
        if (follow != null && follow.Place())
        {
        transform.parent.transform.parent.GetComponentInChildren<ViewSwitcher>().SwitchToPlayer();

        }
    }
    public void Rotate(CallbackContext ctx)
    {

        if (follow != null)
        {
            follow.Rotate();
        }
    }

    private void Update()
    {

        if (active)
        {


            Vector2 lookinput = input.actions["Look"].ReadValue<Vector2>();

            pitch = Mathf.Clamp(pitch - lookinput.y * Camspeed, -80f, 80f);

            transform.rotation = Quaternion.Euler(
                pitch,
                transform.rotation.eulerAngles.y + lookinput.x * Camspeed,
                0
            );



        }
    }

    void FixedUpdate()
    {
        if (active)
        {
            Vector2 moveinput = input.actions["Move"].ReadValue<Vector2>();
            int vert = (input.actions["Jump"].IsPressed() ? 1:0) + (input.actions["Crouch"].IsPressed() ? -1 : 0);

            if (testMode)
            {

            rb.linearVelocity = transform.forward * moveinput.y * speed + transform.right * moveinput.x * speed + Vector3.up * vert * speed;

            } else
            {

                rb.linearVelocity = transform.forward.ProjectOntoPlane(Vector3.up) * moveinput.y * speed + transform.right.ProjectOntoPlane(Vector3.up) * moveinput.x * speed + Vector3.up * vert * speed;
            }

            follow.MoveTo(transform.position);
        }
    }
}
