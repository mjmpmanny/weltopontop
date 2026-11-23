using DG.Tweening;
using System;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Component))]
public class MeleeBaseItem : MonoBehaviour, I_Item
{
    [SerializeField] protected Transform swingVisual;
    protected Sequence swingSequence;
    protected Quaternion swingRestRotation;

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

    [Header("Melee Settings")]
    [Tooltip("The cooldown inbetween swings")]
    public float swingCooldown;
    protected float swingCooldownTimer;
    [Tooltip("The force applyed to the player when hit")]
    public float hitForce;

    private void Update()
    {
        swingCooldownTimer -= Time.deltaTime;
    }
    public void EquipItem(PlayerController player, GameObject toParentTo)
    {
        if (!CanPickUp()) { Debug.LogError("Can not pick up Item"); return; }

        collider.excludeLayers = (1 << player.gameObject.layer);
        rb.constraints = RigidbodyConstraints.FreezeAll;
        this.player = player;

        transform.SetParent(toParentTo.transform);
        transform.localRotation = Quaternion.identity;
        transform.localPosition = holdingOffset;
        IsBeingHeld = true;
        this.player = player;

    }
    public void UseItem(Vector3 origin, Vector3 rotation)
    {

        if (swingVisual == null || swingCooldownTimer >0)
            return;

        if (swingSequence != null && swingSequence.IsActive())
        {
            swingSequence.Kill();
            swingVisual.localRotation = swingRestRotation;
        }

        swingRestRotation = swingVisual.localRotation;
        Quaternion swingRotation =
            swingRestRotation * Quaternion.Euler(90f, 0, 0f);

        swingSequence = DOTween.Sequence()
            .Append(swingVisual.DOLocalRotateQuaternion(swingRotation, 0.09f))
            .Append(swingVisual.DOLocalRotateQuaternion(swingRestRotation, 0.16f))
            .SetLink(gameObject);


        GameObject target = FindPlayerToTarget(2, origin, rotation);
        if (target != null)
        {
            target.transform.root.GetComponentInChildren<RagdollController>().Ragdoll(player.transform.position, hitForce * transform.forward);
        }

        swingCooldownTimer = swingCooldown;
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
        isBeingThrown = true;
        rb.excludeLayers = (15 << 6);

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

            rb.excludeLayers = (15 << 6);
        } else if (collision.collider.gameObject.layer >= 6 && collision.collider.gameObject.layer <= 9 && isBeingThrown)
        {
            Shove.ApplyShoveHit(collision.collider.gameObject, transform.position, rb.linearVelocity);

        }
        isBeingThrown = false;
    }

    private GameObject FindPlayerToTarget(int raycastDistance, Vector3 origin, Vector3 forward)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, forward, out hit, raycastDistance, ((15<<6) & ~player.gameObject.layer)))
        {
            return hit.collider.gameObject;
        } else
        {
            return null;
        }
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
