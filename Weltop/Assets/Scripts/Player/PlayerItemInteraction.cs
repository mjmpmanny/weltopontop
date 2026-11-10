using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController), typeof(PlayerInput))]
public class PlayerItemInteraction : MonoBehaviour
{
    [SerializeField] private float pickupRange = 2.5f;

    private Game game;
    private PlayerController player;
    private PlayerStats stats;
    private InputAction interact;
    private Item heldItem;

    private void Start()
    {
        game = FindFirstObjectByType<Game>();
        player = GetComponent<PlayerController>();
        stats = GetComponent<PlayerStats>();
        interact = GetComponent<PlayerInput>().actions["Interact"];
    }

    private void Update()
    {
        if (game == null || !game.gameIsActive ||
            game.currentRoundState != Game.RoundState.Parkour ||
            Time.timeScale == 0f || !player.canMove ||
            (stats != null && stats.IsDead))
            return;

        if (!interact.WasPressedThisFrame())
            return;

        if (heldItem != null)
        {
            DropHeldItem();
            return;
        }

        Item nearest = null;
        float nearestDistanceSquared = pickupRange * pickupRange;

        foreach (Item item in FindObjectsByType<Item>(FindObjectsSortMode.None))
        {
            if (!item.isActiveAndEnabled || item.isEquipped ||
                item.currentPlayer != null)
                continue;

            float distanceSquared =
                (item.transform.position - transform.position).sqrMagnitude;

            if (distanceSquared > nearestDistanceSquared)
                continue;

            nearest = item;
            nearestDistanceSquared = distanceSquared;
        }

        if (nearest == null)
            return;

        nearest.EquipItem(player);
        if (nearest.currentPlayer == player)
            heldItem = nearest;
    }
    
    private void DropHeldItem()
    {
        Item item = heldItem;
        heldItem = null;

        Vector3 dropPosition = transform.position + transform.forward * 1.25f;

        if (Physics.Raycast(dropPosition + Vector3.up, Vector3.down,
                out RaycastHit ground, 4f, 1 << 0, QueryTriggerInteraction.Ignore))
        {
            dropPosition.y = ground.point.y + 0.25f;
        }

        item.OnDrop();
        item.transform.position = dropPosition;
    }
}