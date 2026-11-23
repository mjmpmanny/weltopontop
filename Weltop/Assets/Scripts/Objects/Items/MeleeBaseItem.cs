using DG.Tweening;
using System;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Component))]
public class MeleeBaseItem : MonoBehaviour, I_Item
{
    [SerializeField] private Transform swingVisual;
    private Sequence swingSequence;
    private Quaternion swingRestRotation;

    private bool IsBeingHeld;
    private PlayerController player;
    private bool isBeingThrown;
    
    private Rigidbody rb => _rb ??= GetComponent<Rigidbody>();
    private Rigidbody _rb;
    private Collider collider => _collider ??= GetComponent<Collider>();
    private Collider _collider;


    [Header("Basic Settings")]
    public Vector3 holdingOffset;
    [Tooltip("The speed of which the item is thrown. (The force applied to the player when hit is based on speed of the item)")]
    public float ThrowSpeed;

    [Header("Melee Settings")]
    [Tooltip("The cooldown inbetween swings")]
    public float swingCooldown;
    private float swingCooldownTimer;
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

    }
    public void UseItem(Transform pos)
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


        GameObject target = FindPlayerToTarget(2);
        if (target != null)
        {
            target.transform.root.GetComponentInChildren<RagdollController>().Ragdoll(player.transform.position, hitForce * transform.forward);
        }

        swingCooldownTimer = swingCooldown;
    }
    public void ThrowItem(Transform pos)
    {
        transform.SetParent(null);
        IsBeingHeld = false;
        isBeingThrown = true;

        rb.constraints = RigidbodyConstraints.None;
        rb.AddForce(pos.forward * ThrowSpeed);
    }
    public void DropItem(Vector3 pos)
    {
        transform.SetParent(null);
        transform.position = pos;
        IsBeingHeld = false;
        isBeingThrown = true;

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
            RagdollController rd = collision.collider.transform.root.GetComponentInChildren<RagdollController>();
            rd.Ragdoll(transform.position, rb.linearVelocity);
            rd.gameObject.GetComponent<ViewSwitcher>().ShakeCams(
                        GlobalParameters.ShoveCameraShakeDurationSeconds,
                        GlobalParameters.ShoveCameraShakeStrength);

        }
        isBeingThrown = false;
    }

    private GameObject FindPlayerToTarget(int raycastDistance)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.parent.forward, out hit, raycastDistance, ((15<<6) & ~player.gameObject.layer)))
        {
            return hit.collider.gameObject;
        } else
        {
            return null;
        }
    }

}
