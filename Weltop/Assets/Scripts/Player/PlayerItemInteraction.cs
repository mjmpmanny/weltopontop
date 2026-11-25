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
    public GameObject RightHand;
    private Game game;
    private PlayerController player;
    private PlayerStats stats;
    private InputAction interact;
    private InputAction Attack;
    private PlayerAnimationController playerAnimationController;
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
        game = FindAnyObjectByType<Game>();
        player = GetComponent<PlayerController>();
        stats = GetComponent<PlayerStats>();
        interact = GetComponent<PlayerInput>().actions["Interact"];
        Attack = GetComponent<PlayerInput>().actions["Attack"];
        Cam = transform.parent.GetComponentInChildren<Camera>();
        playerAnimationController = transform.root.GetComponentInChildren<PlayerAnimationController>();
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


        nearest.EquipItem(player,Cam.gameObject,RightHand);
        heldItem = nearest;
        playerAnimationController.SetAnimationStyle(heldItem.GetAnimationSet());
    }

    public void DropHeldItem(bool discardIfNoGround = false)
    {
        if (heldItem != null)
        {
            heldItem.DropItem(player.LastSolidGround);
            heldItem = null;

            playerAnimationController.SetAnimationStyle(0);
        }
    }

    public void ThrowItem()
    {
        if (IsHoldingItem)
        {
            heldItem.ThrowItem(Cam.transform.position, Cam.transform.forward);
            heldItem = null;
            playerAnimationController.SetAnimationStyle(0);
        }
    }

    public void UseItem()
    {
       if (heldItem != null)
        {
            heldItem.UseItem(Cam.transform.position, Cam.transform.forward);
        }
    }
}