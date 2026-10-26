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

    private float pitch;
    private PlayerInput input;

    private CinemachineCamera Cam;
    private PlaceableObject follow;
    public bool active = false;

    public bool testMode = true;
    [HideInInspector] public bool hasPlacedObject = false;

    private UI ui;
    private Game game;
    private Rigidbody rb;
    private ViewSwitcher viewSwitcher;

    private System.Action<InputAction.CallbackContext> _place;
    private System.Action<InputAction.CallbackContext> _rotate;
    void Start()
    {
        game = FindFirstObjectByType<Game>();
        ui = FindFirstObjectByType<UI>();
        viewSwitcher = transform.parent.transform.parent.GetComponentInChildren<ViewSwitcher>();
        input = viewSwitcher.GetComponent<PlayerInput>();
        Camspeed = viewSwitcher.GetCamSpeed();
        Cam = transform.parent.GetComponentInChildren<CinemachineCamera>();
        
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
        if (obj != null) { // Has switched to buildmode
            follow = obj.GetComponent<PlaceableObject>();
            active = true;
        } else // Has switched from buildmode
        {
            follow = null;
            active = false;
        }
    }

    public void Place(CallbackContext ctx)
    {
        if (follow != null && follow.Place())
        {
            hasPlacedObject = true;
            game.CheckIfAllPlayersPlacedAnObject(this);
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

            rb.linearVelocity = transform.forward.ProjectOntoPlane(Vector3.up) * moveinput.y * speed + transform.right.ProjectOntoPlane(Vector3.up) * moveinput.x * speed + Vector3.up * vert * speed;
            

            if (!hasPlacedObject)
            {
                follow.MoveTo(transform.position);
            }
        }
    }
}
