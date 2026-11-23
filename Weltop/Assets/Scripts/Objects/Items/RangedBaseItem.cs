using DG.Tweening;
using System;
using Unity.Collections;
using Unity.Multiplayer.PlayMode;
using UnityEngine;
using UnityEngine.ProBuilder;

public class RangedBaseItem : MonoBehaviour, I_Item
{

    protected bool IsBeingHeld;
    protected PlayerController player;
    protected bool isBeingThrown;

    protected Rigidbody rb => _rb ??= GetComponent<Rigidbody>();
    protected Rigidbody _rb;
    protected Collider collider => _collider ??= GetComponent<Collider>();
    protected Collider _collider;

    [Header("Basic Settings")]
    public Vector3 holdingOffset;
    [Tooltip("The speed of which the item is thrown. (The force applied to the player when hit is based on speed of the item)")]
    public float ThrowSpeed;

    [Header("Ranged Settings")]
    public Vector3 MussleOffset;
    public Projectile projectile;
    

    public void EquipItem(PlayerController player, GameObject toParentTo)
    {
        if (!CanPickUp()) { Debug.LogError("Can not pick up Item"); return; }

        collider.excludeLayers = (1 << player.gameObject.layer);
        rb.constraints = RigidbodyConstraints.FreezeAll;

        transform.SetParent(toParentTo.transform);
        transform.localRotation = Quaternion.identity;
        transform.localPosition = holdingOffset;
        IsBeingHeld = true;
        this.player = player;
    }
    public virtual void UseItem(Vector3 origin, Vector3 rotation)
    {
        Projectile instantiatedProjectile = Instantiate(projectile, origin, Quaternion.LookRotation(rotation,Vector3.up),transform);
        instantiatedProjectile.playerOwner = player.gameObject;
    }
    public void ThrowItem(Vector3 origin, Vector3 rotation)
    {
        transform.SetParent(null);
        transform.position = origin;
        IsBeingHeld = false;
        isBeingThrown = true;

        rb.constraints = RigidbodyConstraints.None;
        rb.excludeLayers = (1 << player.gameObject.layer);
        rb.AddForce(rotation * ThrowSpeed);
    }
    public void DropItem(Vector3 pos)
    {
        transform.SetParent(null);
        transform.position = pos;
        IsBeingHeld = false;
        collider.excludeLayers = (15 << 6);

        rb.constraints = RigidbodyConstraints.None;
    }
    public bool CanPickUp()
    {
        return !IsBeingHeld && !isBeingThrown;
    }
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    public bool IsBeginHeldBy(PlayerController player)
    {
        return IsBeingHeld;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == 0)
        {

            collider.excludeLayers = (15 << 6);//Collider cant interact players when not being thrown (can be changed if funny)
        } else if (collision.collider.gameObject.layer >= 6 && collision.collider.gameObject.layer <= 9 && isBeingThrown)
        {
            Shove.ApplyShoveHit(collision.collider.gameObject, transform.position, rb.linearVelocity);
        }
        isBeingThrown = false;
    }

    public void DestroyItem()
    {
        if (IsBeingHeld)
        {
            player.GetComponent<PlayerItemInteraction>().DropHeldItem();
        }
        Destroy(gameObject);
    }
}
