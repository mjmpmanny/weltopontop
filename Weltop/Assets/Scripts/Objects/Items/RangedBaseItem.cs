using DG.Tweening;
using System;
using Unity.Collections;
using Unity.Multiplayer.PlayMode;
using UnityEngine;
using UnityEngine.ProBuilder;

public class RangedBaseItem : MonoBehaviour, I_Item
{

    private bool IsBeingHeld;
    private PlayerController player;
    private bool isBeingThrown; 
    private Rigidbody rb => _rb != null ? _rb : (_rb = GetComponent<Rigidbody>());
    private Rigidbody _rb;
    private Collider collider => _collider != null ? _collider : (_collider = GetComponent<Collider>());
    private Collider _collider;

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

    }
    public virtual void UseItem(Transform pos)
    {
        Projectile instantiatedProjectile = Instantiate(projectile, pos.position, pos.rotation);
        instantiatedProjectile.playerOwner = player.gameObject;
    }
    public void ThrowItem(Transform pos)
    {
        transform.SetParent(null);
        IsBeingHeld = false;
        isBeingThrown = true;

        rb.constraints = RigidbodyConstraints.None;
        rb.AddForce(pos.forward * ThrowSpeed);
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
            RagdollController rd = collision.collider.transform.root.GetComponentInChildren<RagdollController>();
            rd.Ragdoll(transform.position, rb.linearVelocity);
            rd.gameObject.GetComponent<ViewSwitcher>().ShakeCams(
                        GlobalParameters.ShoveCameraShakeDurationSeconds,
                        GlobalParameters.ShoveCameraShakeStrength);
        }
        isBeingThrown = false;
    }
}
