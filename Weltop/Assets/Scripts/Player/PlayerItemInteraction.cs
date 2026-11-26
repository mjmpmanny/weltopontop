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
            Interact(new CallbackContext());
        }
        if (Attack.WasPressedThisFrame() && IsHoldingItem)
        {
            UseItem();
        }
    }
    
    public void Interact(CallbackContext ctx)
    {
        I_Interactable nearest = null;
        float nearestDist = float.MaxValue;
        float nearestDistanceSquared = pickupRange * pickupRange;

        Collider[] cols = Physics.OverlapSphere(transform.position, pickupRange, 1 << 16);
        List<I_Interactable> InteractableObjects = new List<I_Interactable>();
        foreach (Collider col in cols)
        {
            I_Interactable InteractableObject = col.gameObject.GetComponent<I_Interactable>();
            if (InteractableObject.CanBeInteractedWith() && (transform.position - InteractableObject.GetPosition()).magnitude < nearestDist)
            {
                nearest = InteractableObject;
            }
        }


        if (nearest == null)
            return;

        nearest.Interact(this);
    }

    public void PickupItem(I_Item item)
    {
        item.EquipItem(player,Cam.gameObject,RightHand);
        heldItem = item;
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