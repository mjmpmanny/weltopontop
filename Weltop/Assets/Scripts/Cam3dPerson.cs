using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cam3dPerson : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speed;
    private Cinemachine3rdPersonFollow c3pf;
    // Use this for initialization
    void Start()
    {
        c3pf = GetComponent<Cinemachine3rdPersonFollow>();
    }

    Vector2 lookinput;
    void LateUpdate()
    {
        lookinput = lookinput + InputSystem.actions["Look"].ReadValue<Vector2>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        


    }
    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x - lookinput.y * speed,
            transform.rotation.eulerAngles.y + lookinput.x * speed, 0);
        lookinput = new Vector2(0,0);
        transform.position = transform.parent.GetComponentInChildren<PlayerController_3dPerson>().transform.position;

    }
}
