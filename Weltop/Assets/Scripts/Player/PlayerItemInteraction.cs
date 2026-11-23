using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(PlayerController), typeof(PlayerInput))]
public class PlayerItemInteraction : MonoBehaviour
{
    [SerializeField] private float pickupRange = 2.5f;

    private Game game;
    private PlayerController player;
    private PlayerStats stats;
    private InputAction interact;
    private InputAction Attack;
    private Camera Cam;
    private I_Item heldItem;
    public bool IsHoldingItem => heldItem != null && heldItem.IsBeginHeldBy(player);
    
    private int lastItemAttackFrame = -1;

    public bool UsedItemThisFrame => lastItemAttackFrame == Time.frameCount;

    public void MarkItemAttackUsed()
    {
        lastItemAttackFrame = Time.frameCount;
    }

    private void Start()
    {
        game = FindFirstObjectByType<Game>();
        player = GetComponent<PlayerController>();
        stats = GetComponent<PlayerStats>();
        interact = GetComponent<PlayerInput>().actions["Interact"];
        Attack = GetComponent<PlayerInput>().actions["Attack"];
        Cam = transform.parent.GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if (game == null || !game.gameIsActive ||
            game.currentRoundState != Game.RoundState.Parkour ||
            Time.timeScale == 0f || !player.canMove ||
            (stats != null && stats.IsDead))
            return;

        if (interact.WasPressedThisFrame())
        {
            if (heldItem != null)
            {
                ThrowItem();
                return;
            }
            PickupItem(new CallbackContext());
        }
        if (Attack.WasPressedThisFrame() && IsHoldingItem)
        {
            UseItem();
        }
    }
    
    private void PickupItem(CallbackContext ctx)
    {

        I_Item nearest = null;
        float nearestDist = float.MaxValue;
        float nearestDistanceSquared = pickupRange * pickupRange;

        Collider[] cols = Physics.OverlapSphere(transform.position, pickupRange, 1 << 16);
        List<I_Item> items = new List<I_Item>();
        foreach (Collider col in cols)
        {
            I_Item item = col.gameObject.GetComponent<I_Item>();
            if (item.CanPickUp() && (transform.position - item.GetPosition()).magnitude < nearestDist)
            {
                nearest = item;
            }
        }


        if (nearest == null)
            return;

        nearest.EquipItem(player,Cam.gameObject);
        heldItem = nearest;
    }

    public void DropHeldItem(bool discardIfNoGround = false)
    {
        I_Item item = heldItem;
        heldItem = null;

        if (item == null || item.IsBeginHeldBy(player))
            return;

        Transform aim = player.GetComponent<Shove>().playerCam.transform;
        Quaternion facing = Quaternion.Euler(0f, aim.eulerAngles.y, 0f);
        Vector3 dropPosition = transform.position + (facing * Vector3.forward) * 1.25f;

        bool foundGround = Physics.Raycast(
            dropPosition + Vector3.up, Vector3.down,
            out RaycastHit ground, 4f, 1 << 0, QueryTriggerInteraction.Ignore);

        if (foundGround)
            dropPosition.y = ground.point.y + 0.25f;

        //item.OnDrop();

        if (!foundGround && discardIfNoGround)
        {
            //Destroy(item.gameObject);
            return;
        }

        //item.transform.SetPositionAndRotation(dropPosition, facing);
    }

    public void ThrowItem()
    {
        if (IsHoldingItem)
        {
            heldItem.ThrowItem(Cam.transform);
            heldItem = null;
        }
    }

    public void UseItem()
    {
       if (heldItem != null)
        {
            heldItem.UseItem(Cam.transform);
        }
    }
}