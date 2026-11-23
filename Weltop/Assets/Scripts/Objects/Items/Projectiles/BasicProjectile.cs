using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class BasicProjectile : Projectile
{
    [Tooltip("How fast the projectile moves through space")]
    public float speed = 1f;
    [Tooltip("A multiplier of how this object is affected by gravity. Use 0 to ignore gravity.")]
    public float gravityModifier = 0f;
    private Rigidbody rb;
    private void Start()
    {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.linearVelocity = transform.forward * speed;
        transform.SetParent(null);
    }
    private void FixedUpdate()
    {
            Vector3 customGravity = Physics.gravity * gravityModifier;
            rb.AddForce(customGravity, ForceMode.Acceleration);
        
    }
    public override void OnHitOpponent(GameObject playerTarget)
    {
        Shove.ApplyShoveHit(playerTarget, GetComponent<Rigidbody>().linearVelocity);
    }

    
}
