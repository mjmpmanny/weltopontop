using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Basic Settings")]
    [Tooltip("Does this projectile get destroyed after hitting its target?")]
    public bool destroyOnHit = true;
    
    [HideInInspector] //Used so that the projectile does not affect the player who shot it
    public GameObject playerOwner;


<<<<<<< HEAD
=======
    private bool hitWall = false;
    private bool hitOpponent = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }
>>>>>>> main

    private void Update()
    {
        OnFlying();
    }
<<<<<<< HEAD
=======

    private void FixedUpdate()
    {
        Vector3 customGravity = Physics.gravity * gravityModifier;
        rb.AddForce(customGravity, ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject != playerOwner && !hitOpponent)
            {
                OnHitOpponent(other.gameObject);
                hitOpponent = true;
                if(destroyOnHit)
                    Destroy(gameObject);
            }
        }
        
        if (!other.gameObject.CompareTag("Player") && !hitWall)
        {
            if (other.gameObject.CompareTag("HillZone") || other.gameObject.CompareTag("KillZone"))
                return;
            
            OnHitWall(other.gameObject);
            hitWall = true;
        }
    }

>>>>>>> main
    /// <summary>
    /// Called once per frame while the object exists
    /// </summary>
    public virtual void OnFlying()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerOwner != null && other.gameObject.layer != playerOwner.layer)
            {
                OnHitOpponent(other.gameObject);
                if (destroyOnHit)
                    Destroy(gameObject);
            }
        }
        if (other.gameObject.layer == 0)
        {
            if (destroyOnHit)
                Destroy(gameObject);

        }
    }
    /// <summary>
    /// Called once when this projectile hits an opponent
    /// </summary>
    /// <param name="playerTarget">The player that this projectile hit</param>
    public virtual void OnHitOpponent(GameObject playerTarget)
    {
        
    }
    
    /// <summary>
    /// Called once when this projectile hits a collider
    /// </summary>
    /// <param name="wallHit">The object that this projectile hit</param>
    public virtual void OnHitWall(GameObject wallHit)
    {
        
    }

    /// <summary>
    /// Helper method to destroy projectile.
    /// </summary>
    public virtual void KillSelf()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// <para>Helper method to have a projectile apply knockback</para>
    /// <para>WARNING! Is not normalized by default</para>
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
