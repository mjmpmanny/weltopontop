using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using static UnityEngine.InputSystem.InputAction;

public class BuildModeController : MonoBehaviour
{
    public float acceleration;
    public float maxSpeed;
    private Vector3 speed;

    private float Camspeed;

    private float pitch;
    private PlayerInput input;

    private CinemachineCamera Cam;
    private PlaceableObject follow;
    public bool active = false;

    public bool testMode = true;
    [HideInInspector] public bool hasPlacedObject = false;

    private Vector3 inWorldPos;
    private UI ui;
    private Game game;
    private Rigidbody rb;
    private ViewSwitcher viewSwitcher;

    private System.Action<InputAction.CallbackContext> _place;
    private System.Action<InputAction.CallbackContext> _rotate;
    private System.Action<InputAction.CallbackContext> _firstMove;
    void Start()
    {
        game = FindFirstObjectByType<Game>();
        ui = FindFirstObjectByType<UI>();
        viewSwitcher = transform.parent.transform.parent.GetComponentInChildren<ViewSwitcher>();
        input = viewSwitcher.GetComponent<PlayerInput>();
        Camspeed = viewSwitcher.GetCamSpeed();
        Cam = transform.parent.GetComponentInChildren<CinemachineCamera>();

        //rb = GetComponent<Rigidbody>();
        pitch = transform.eulerAngles.x;
        if (pitch > 180) pitch -= 360;


        _place = Place;
        _rotate = Rotate;
        _firstMove = firstMove;
        input.actions["Place"].started += _place;
        input.actions["Rotate"].started += _rotate;
        input.actions["Move"].started += _firstMove;
        input.actions["Jump"].started += _firstMove;
        input.actions["Crouch"].started += _firstMove;
    }
    private void OnDestroy()
    {
        input.actions["Place"].started -= _place;
        input.actions["Rotate"].started -= _rotate;
        input.actions["Move"].started -= _firstMove;
        input.actions["Jump"].started -= _firstMove;
        input.actions["Crouch"].started -= _firstMove;

    }

    public void SetController(GameObject obj, int camPriority, Vector3 location)
    {
        transform.position = location;
        Cam.Priority = camPriority;
        if (obj != null)
        { // Has switched to buildmode
            follow = obj.GetComponent<PlaceableObject>();
            hasPlacedObject = false;
            active = true;
            inWorldPos = location;
            obj.transform.position = transform.position;
        } else // Has switched from buildmode
        {
            if (!hasPlacedObject && follow != null)
            {
                OnTimeOut();
            }
            follow = null;
            active = false;
            hasPlacedObject = true;
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

    public void OnTimeOut()
    {
        follow.EndOfRoundPlace();
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
            int vert = (input.actions["Jump"].IsPressed() ? 1 : 0) + (input.actions["Crouch"].IsPressed() ? -1 : 0);

            if (moveinput != Vector2.zero || vert != 0)
            {

                inWorldPos += MovementVector(input,speed);
                speed += new Vector3(Mathf.Abs(moveinput.y), Mathf.Abs(vert), Mathf.Abs(moveinput.x)) * acceleration;
                speed = Vector3.ClampMagnitude(speed, maxSpeed);
            } else
            {
                speed = Vector3.zero;
                inWorldPos = transform.position;
            }

            if (!hasPlacedObject)
            {
                float GridSize = GlobalParameters.GridSize;
                Vector3 inGridPos = ToGrid(inWorldPos);


                if (Physics.OverlapBox(inGridPos, Vector3.one * GridSize * 0.9f, Quaternion.identity, 1).Length > 0 && Physics.OverlapBox(transform.position, Vector3.one * GridSize * 0.9f, Quaternion.identity, 1).Length == 0)
                {
                    speed = Vector3.zero;
                    inWorldPos = transform.position;
                } else
                {
                    transform.position = inGridPos;
                }

                follow.MoveTo(transform.position);
            } else
            {
                transform.position = inWorldPos;
            }
        }
    }
    void firstMove(CallbackContext ctx)
    {
        if (active && !hasPlacedObject)
        {

            inWorldPos += MovementVector(input,Vector3.one);
            inWorldPos = ToGrid(inWorldPos);
        }
        
    }
    Vector3 ToGrid(Vector3 pos)
    {
        float GridSize = GlobalParameters.GridSize;
        return new Vector3(GridSize * Mathf.Round(pos.x / GridSize), GridSize * Mathf.Round(pos.y / GridSize), GridSize * Mathf.Round(pos.z / GridSize)) + GlobalParameters.GridOffset;
    }

    Vector3 MovementVector(PlayerInput inp,Vector3 speed)
    {
        Vector2 moveinput = (inp.actions["Move"].ReadValue<Vector2>()).normalized;
        int vert = (inp.actions["Jump"].IsPressed() ? 1 : 0) + (inp.actions["Crouch"].IsPressed() ? -1 : 0);
        float GridSize = GlobalParameters.GridSize;

        return (transform.forward.ProjectOntoPlane(Vector3.up).normalized * moveinput.y * GridSize * speed.x + transform.right.ProjectOntoPlane(Vector3.up).normalized * moveinput.x * GridSize * speed.z + Vector3.up * vert * GridSize * speed.y);

    }
}