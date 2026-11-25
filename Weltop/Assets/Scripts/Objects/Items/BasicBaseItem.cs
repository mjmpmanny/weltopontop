using DG.Tweening;
using Unity.Multiplayer.PlayMode;
using UnityEngine;

public class BasicBaseItem : MonoBehaviour , I_Item
{
    protected BasicBaseItem fakeItem;
    protected bool IsBeingHeld;
    protected PlayerController player;
    protected bool isBeingThrown;
    protected Rigidbody rb => _rb ??= GetComponent<Rigidbody>();
    private Rigidbody _rb;
    protected Collider collider => _collider ??= GetComponent<Collider>();
    private Collider _collider;

    [Header("Basic Settings")]
    public Vector3 holdingOffset;
    [Tooltip("The speed of which the item is thrown. (The force applied to the player when hit is based on speed of the item)")]
    public float ThrowSpeed;
    [Tooltip("The Animation set that the player will use when holdiong this item")]
    public AnimationType AnimationSet;

    public enum AnimationType
    {
        Default = 0,
        Rifle = 1,
        OneHanded = 2,
        twohanded = 3
    }

    public virtual void EquipItem(PlayerController player, GameObject toParentTo, GameObject HandToParentTo)
    {
        if (!CanPickUp()) { Debug.LogError("Can not pick up Item"); return; }

        collider.excludeLayers = (1 << player.gameObject.layer);
        rb.constraints = RigidbodyConstraints.FreezeAll;

        transform.SetParent(toParentTo.transform);
        transform.localRotation = Quaternion.identity;
        transform.localPosition = holdingOffset;
        IsBeingHeld = true;
        this.player = player;



        if (HandToParentTo != null)
        {
            SetAllChildLayers(gameObject,player.gameObject.layer + 6);
            SetColliderEnabled(gameObject,false);

            fakeItem = Instantiate(gameObject,HandToParentTo.transform).GetComponent<BasicBaseItem>();



            SetAllChildLayers(fakeItem.gameObject, player.gameObject.layer);

            //this should not become a problem
            foreach(Collider col in fakeItem.GetComponentsInChildren<Collider>())
            {
                Destroy(col);
            }

            fakeItem.transform.localPosition = Vector3.zero;
            fakeItem.transform.localRotation = Quaternion.Euler(-105,60,29);
            fakeItem.transform.localScale =new Vector3(1,1,1) / fakeItem.transform.lossyScale.x;
        }
    }
    public virtual void UseItem(Vector3 origin, Vector3 rotation)
    {
    }
    public virtual void ThrowItem(Vector3 origin, Vector3 rotation)
    {
        player.animationController.PlayAttackAnimation();

        SetAllChildLayers(gameObject, 16);
        SetColliderEnabled(gameObject, true);
        fakeItem.DestroyItem();

        transform.SetParent(null);
        transform.position = origin;
        IsBeingHeld = false;
        isBeingThrown = true;

        rb.constraints = RigidbodyConstraints.None;
        rb.excludeLayers = (1 << player.gameObject.layer);
        rb.AddForce(rotation * ThrowSpeed);
    }
    public virtual void DropItem(Vector3 pos)
    {

        SetColliderEnabled(gameObject, true);
        SetAllChildLayers(gameObject, 16);
        fakeItem.DestroyItem();
        transform.SetParent(null);
        transform.position = pos;
        IsBeingHeld = false;
        rb.excludeLayers = (15 << 6);
        rb.constraints = RigidbodyConstraints.None;
    }
    public virtual bool CanPickUp()
    {
        return !IsBeingHeld && !isBeingThrown;
    }
    public virtual Vector3 GetPosition()
    {
        return transform.position;
    }
    public virtual bool IsBeginHeldBy(PlayerController player)
    {
        return IsBeingHeld;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (!IsBeingHeld) {
            OnCollision(collision);
        }
    }
    /// <summary>
    /// Runs when The item hits another object when not held
    /// </summary>
    protected virtual void OnCollision(Collision collision)
    {
        if (collision.collider.gameObject.layer == 0)
        {

            rb.excludeLayers = (15 << 6);//Collider cant interact players when not being thrown (can be changed if funny)
        } else if (collision.collider.gameObject.layer >= 6 && collision.collider.gameObject.layer <= 9 && isBeingThrown)
        {
            Shove.ApplyShoveHit(collision.collider.gameObject, transform.position, rb.linearVelocity);
        }
        isBeingThrown = false;

    }
    public virtual void DestroyItem()
    {
        if (IsBeingHeld && player != null)
        {
            player.GetComponent<PlayerItemInteraction>().DropHeldItem();
        }
        if (fakeItem != null)
        {
            fakeItem.DestroyItem();
        }
        Destroy(gameObject);
    }


    /// <summary>
    /// Used show the visual of the item boing used but does not have funtionality after that (ie: The mussle flash of the gun or the laser from the laser gun)
    /// (This should be used on the fake item that is attached to the hand and the main item) 
    /// </summary>
    /// <param name="origin">Stores the direction and position where the item is being used</param>
    /// <param name="forward">Stores the rotation where the item is being thrown</param>
    public virtual void UseVisual(Vector3 origin, Vector3 forward)
    {

    }
    /// <param name="EndPoint">Optional if there is an endpoint that is needed for the visual</param>
    public virtual void UseVisual(Vector3 origin, Vector3 forward, Vector3 EndPoint)
    {

    }

    public int GetAnimationSet()
    {
        return (int)AnimationSet;
    }

    protected void SetAllChildLayers(GameObject obj, int layer)
    {
        foreach (Transform child in obj.GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = layer;
        }
    }
    protected void SetColliderEnabled(GameObject obj, bool enabled)
    {
        foreach (Collider child in obj.GetComponentsInChildren<Collider>())
        {
            child.enabled = enabled;
        }
    }

}
