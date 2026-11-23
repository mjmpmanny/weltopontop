using DG.Tweening;
using Unity.Multiplayer.PlayMode;
using UnityEngine;

public class BasicBaseItem : MonoBehaviour , I_Item
{

    private bool IsBeingHeld;
    private PlayerController player;
    private bool isBeingThrown;
    private Rigidbody rb => _rb != null ? _rb : (_rb = GetComponent<Rigidbody>());//Lazy loads the compoent
    private Rigidbody _rb;
    private Collider collider => _collider != null ? _collider : (_collider = GetComponent<Collider>());//Lazy loads the compoent
    private Collider _collider;

    [Header("Basic Settings")]
    public Vector3 holdingOffset;
    [Tooltip("The speed of which the item is thrown. (The force applied to the player when hit is based on speed of the item)")]
    public float ThrowSpeed;


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
    public void UseItem(Transform pos)
    {
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
