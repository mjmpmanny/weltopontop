using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController_3dPerson : MonoBehaviour
{
    public float speed;
    private Rigidbody rb;
    private GameObject cam;
    private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = transform.parent.GetComponentInChildren<Camera>().gameObject;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveinput = InputSystem.actions["Move"].ReadValue<Vector2>();
        Vector3 move = new Vector3(cam.transform.forward.x,0,cam.transform.forward.z).normalized * moveinput.y
            + new Vector3(cam.transform.right.x,0, cam.transform.right.z).normalized * moveinput.x;


        rb.linearVelocity = move * speed;


    }
}
