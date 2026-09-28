<<<<<<< Updated upstream
=======
using Unity.Cinemachine;
using Unity.VisualScripting;
>>>>>>> Stashed changes
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines.Interpolators;
using static Unity.Burst.Intrinsics.X86.Avx;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController_3dPerson : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed;
<<<<<<< Updated upstream
=======
    public float accel;
    public float momentumDampening;

    [Header("Jump Settings")]
    public float CoyoteJumpTime;
    public float JumpStrength;
    public float LandingCooldown;
    public float AirAccel;
    public float MaxAirSpeed;
    public float AirDampening;

    private Vector2 origionaldir;
    private float LandingTimer;



    private System.Action<InputAction.CallbackContext> jump_;
>>>>>>> Stashed changes
    private Rigidbody rb;
    private GameObject cam;
    private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        jump_ = jump;
        InputSystem.actions["Jump"].started += jump_;
    }
    private void OnDestroy()
    {
        InputSystem.actions["Jump"].started -= jump_;

    }
    void Start()
    {
        cam = transform.parent.GetComponentInChildren<Camera>().gameObject;
        rb = GetComponent<Rigidbody>();

    }

<<<<<<< Updated upstream
    // Update is called once per frame
    void Update()
=======
    float OnGround;
    private void Update()
    {
        OnGround -= Time.deltaTime;
        LandingTimer -= Time.deltaTime;



    }


    void jump(CallbackContext ctx)
    {
        if ( OnGround > 0)
        {
            OnGround = 0;
            LandingTimer = LandingCooldown;
            rb.linearDamping = 0;
            rb.AddForce(Vector3.up * JumpStrength);
            origionaldir = new Vector2(rb.linearVelocity.x,rb.linearVelocity.z);
        } else
        {
            Debug.Log("not on ground");
        }
    }
    void FixedUpdate()
>>>>>>> Stashed changes
    {

        //basic movement
        Vector2 moveinput = InputSystem.actions["Move"].ReadValue<Vector2>();
        Vector3 move = new Vector3(cam.transform.forward.x,0,cam.transform.forward.z).normalized * moveinput.y
            + new Vector3(cam.transform.right.x,0, cam.transform.right.z).normalized * moveinput.x;

        //jumping
        CapsuleCollider cap = GetComponent<CapsuleCollider>();
        RaycastHit hit;
        bool raycastHit = Physics.SphereCast(transform.position - new Vector3(0, cap.height / 2 - cap.radius, 0), cap.radius * 0.9f, Vector3.down, out hit, (cap.radius - cap.radius * 0.1f) + 0.01f, ~(1 << 3));
        
        if (raycastHit && LandingTimer < 0)
        {//Ground Movement
            OnGround = CoyoteJumpTime;
            rb.linearDamping = momentumDampening;


            bool goingForward = Vector3.Dot(move, rb.linearVelocity) > 0;
            if (rb.linearVelocity.magnitude > speed && goingForward) {
                    Vector3 cross = Vector3.Cross(Vector3.up, rb.linearVelocity);
                    move = Vector3.Project(move, cross);

            }
            rb.AddForce(move * accel, ForceMode.VelocityChange);

        } else//air movement
        {
            rb.linearDamping = 0;

            bool goingForward = Vector3.Dot(move,rb.linearVelocity) > 0;

            if (goingForward && (rb.linearVelocity - Vector3.up * rb.linearVelocity.y).magnitude >= MaxAirSpeed)
            {
                Vector3 cross = Vector3.Cross(Vector3.up,rb.linearVelocity);
                move = Vector3.Project(move,cross);

            }
            rb.linearVelocity = rb.linearVelocity - new Vector3(rb.linearVelocity.x,0,rb.linearVelocity.z) * AirDampening / 100;
            rb.AddForce(move * AirAccel, ForceMode.VelocityChange);

        }











<<<<<<< Updated upstream
        rb.linearVelocity = move * speed;

=======
>>>>>>> Stashed changes

    }

   
}
