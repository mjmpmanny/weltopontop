using UnityEngine;
using UnityEngine.InputSystem;

public class Cam3dPerson : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speed;

    // Use this for initialization
    void Start()
    {

    }

    void FixedUpdate()
    {
        Vector2 lookinput = InputSystem.actions["Look"].ReadValue<Vector2>();
        
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x - lookinput.y*speed*Time.deltaTime, 
            transform.rotation.eulerAngles.y + lookinput.x*speed * Time.deltaTime, 0);
        transform.position = transform.parent.GetComponentInChildren<PlayerController_3dPerson>().transform.position;
    }
}
