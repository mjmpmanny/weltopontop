using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Basic Settings")]
    [Tooltip("Does this projectile get destroyed after hitting its target?")]
    public bool destroyOnHit = true;
    
    [HideInInspector] //Used so that the projectile does not affect the player who shot it
    public GameObject playerOwner;

    private bool hitWall = false;
    private bool hitOpponent = false;
    protected Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void Update()
    {
        OnFlying();
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
    public static void ApplyKnockback(GameObject playerTarget, float knockbackForce, Vector3 direction)
    {
        Rigidbody rb = playerTarget.GetComponent<Rigidbody>();
        if (rb == null)
            return;

        rb.AddForce(direction * knockbackForce, ForceMode.Impulse);
    }
}
