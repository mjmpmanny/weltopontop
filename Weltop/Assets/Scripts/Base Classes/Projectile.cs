using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Basic Settings")]
    [Tooltip("How fast the projectile moves through space")]
    public float speed = 1f;
    [Tooltip("A multiplier of how this object is affected by gravity. Use 0 to ignore gravity.")]
    public float gravityModifier = 0f;
    [Tooltip("Does this projectile get destroyed after hitting its target?")]
    public bool destroyOnHit = true;
    
    [HideInInspector] //Used so that the projectile does not affect the player who shot it
    public GameObject playerOwner;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void Update()
    {
        OnFlying();
    }

    private void FixedUpdate()
    {
        Vector3 customGravity = Physics.gravity * gravityModifier;
        rb.AddForce(customGravity, ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject != playerOwner)
            {
                OnHitOpponent(other.gameObject);
                if(destroyOnHit)
                    Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Called once per frame while the object exists
    /// </summary>
    public virtual void OnFlying()
    {
        
    }
    
    /// <summary>
    /// Called once when this projectile hits an opponent
    /// </summary>
    /// <param name="playerTarget">The player that this projectile hit</param>
    public virtual void OnHitOpponent(GameObject playerTarget)
    {
        
    }

    /// <summary>
    /// Helper method to have a projectile apply knockback
    /// </summary>
    /// <param name="playerTarget">The player that will get knocked back</param>
    /// <param name="knockbackForce">The knockback strength</param>
    public void ApplyKnockback(GameObject playerTarget, float knockbackForce, Vector3 direction)
    {
        Rigidbody rb = playerTarget.GetComponent<Rigidbody>();
        if (rb == null)
            return;
        
        rb.AddForce(direction * knockbackForce, ForceMode.Impulse);
    }
}
