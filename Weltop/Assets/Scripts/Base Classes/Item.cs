using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Item : MonoBehaviour
{
    #region Exposed Parameters
    [Header("Basic Stats")]
    [Tooltip("How many times the item can be used before it is automatically discarded (-1 = unlimited)")]
    public int useAmount = 1;
    [Tooltip("How many seconds between uses does this item have")]
    public float secondsBetweenUse = 1f;
    [Tooltip("Increases/Decreases walk speed.\nLeave at 0 for no effect. -1 will stop movement completely.")]
    public float walkSpeedModifier = 0f;
    [Tooltip("Increases/Decreases jump height.\nLeave at 0 for no effect. -1 will stop jumping completely.")]
    public float jumpStrengthModifier = 0f;
    
    [Header("Visual Settings")]
    [Tooltip("Where the center of the object is snapped to on the player game object")]
    public Vector3 itemHoldPositionOffset;
    
    [Header("Player Disadvantages")]
    [Tooltip("Disables the ability to jump")]
    public bool disableJump = false;
    [Tooltip("Disables the ability to shove")]
    public bool disableShove = false;
    
    [Header("Projectile Settings")]
    [Tooltip("Does this item shoot a projectile when used?")]
    public bool shootsProjectile = false;
    [Tooltip("Does this item not shoot a physical projectile, but fires a hitscan?")]
    public bool shootsHitscan = false;
    [Tooltip("The projectile that this item shoots")]
    public Projectile projectile;
    #endregion

    /// <summary>
    /// Is this item currently equipped by a player?
    /// </summary>
    public bool isEquipped;

    /// <summary>
    /// The player that has this item equipped
    /// </summary>
    public PlayerController currentPlayer;
    private PlayerInput inp;
    private bool foundPlayerInput = false;

    private int howManyTimesHasThisBeenUsed = 0;

    private bool canUseItem = true;
    
    private Game game;
    
    [Header("Hitscan Visuals")]
    [SerializeField] private LineRenderer hitscanBeamPrefab;
    [SerializeField] private Transform hitscanMuzzle;
    [SerializeField, Min(0f)] private float hitscanAimAssistRadius = 0.20f;

    private void Start()
    {
        game = FindFirstObjectByType<Game>();
    }
    
    private void Update()
    {
        if (isEquipped)
        {
            if (!foundPlayerInput)
            {
                inp = currentPlayer.GetComponent<PlayerInput>();
                foundPlayerInput = true;
            }
            
            AdjustPlayerStats();
            UpdateHoldPosition();
            
            if (game == null || !game.gameIsActive ||
                game.currentRoundState != Game.RoundState.Parkour ||
                Time.timeScale == 0f || !currentPlayer.canMove)
                return;
            
            OnHold();
            
            bool attackPressed = inp.actions["Secondary Attack"].triggered;

            if (attackPressed)
            {
                if(canUseItem)
                {
                    OnUse();
                    
                    if (shootsProjectile || shootsHitscan)
                    {
                        ShootProjectile();
                    }
                    else
                    {
                        GameObject targetPlayer = FindPlayerToTarget(2);

                        if (targetPlayer != null)
                        {
                            OnHitOpponentMelee(targetPlayer);
                        }
                    }
                    
                    howManyTimesHasThisBeenUsed += 1;

                    if (howManyTimesHasThisBeenUsed >= useAmount)
                    {
                        if(useAmount != -1)
                            DiscardItem();
                    }
                    
                    StartCoroutine(UseItemCooldown());
                }
            }
        }
    }

    private void ShootProjectile()
    {
        if (shootsHitscan)
        {
            FireHitscan();
        }
        else
        {
            Projectile instantiatedProjectile = Instantiate(projectile, currentPlayer.GetComponent<Shove>().playerCam.transform.position, currentPlayer.GetComponent<Shove>().playerCam.transform.rotation);
            instantiatedProjectile.playerOwner = currentPlayer.gameObject;
            Rigidbody projectileRb = instantiatedProjectile.GetComponent<Rigidbody>();
            projectileRb.linearVelocity = currentPlayer.GetComponent<Shove>().playerCam.transform.forward * instantiatedProjectile.speed;
        }
    }
    
    private GameObject FindPlayerToTarget(int raycastDistance)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, currentPlayer.GetComponent<Shove>().playerCam.transform.forward, out hit, raycastDistance, currentPlayer.GetComponent<Shove>().layerMask))
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }
    
    private void FireHitscan()
    {
        const float range = 999f;

        Shove shove = currentPlayer.GetComponent<Shove>();
        Transform aim = shove.playerCam.transform;

        // Include the player layers and solid geometry on Default.
        int hitMask = shove.layerMask.value | (1 << 0);
        RaycastHit[] hits = Physics.RaycastAll(
            aim.position, aim.forward, range, hitMask, QueryTriggerInteraction.Ignore);

        PlayerStats shooter = currentPlayer.GetComponent<PlayerStats>();
        PlayerStats target = null;
        float nearestDistance = range;
        Vector3 endPoint = aim.position + aim.forward * range;

        foreach (RaycastHit hit in hits)
        {
            if (hit.distance >= nearestDistance ||
                hit.collider.transform.IsChildOf(transform))
                continue;

            PlayerStats hitPlayer = hit.collider.GetComponentInParent<PlayerStats>();
            
            if (hitPlayer != null && hitPlayer == shooter)
                continue;

            nearestDistance = hit.distance;
            endPoint = hit.point;
            target = hitPlayer;
        }
        
        if (target == null && hitscanAimAssistRadius > 0f && IsUsingGamepad())
        {
            target = FindAssistedHitscanTarget(
                aim, shove.layerMask.value, shooter, nearestDistance, ref endPoint);
        }

        if (hitscanBeamPrefab != null)
        {
            LineRenderer beam = Instantiate(hitscanBeamPrefab);
            beam.useWorldSpace = true;
            beam.widthMultiplier = 0.03f;
            beam.positionCount = 2;
            beam.SetPosition(0, hitscanMuzzle != null ? hitscanMuzzle.position : transform.position);
            beam.SetPosition(1, endPoint);
            Destroy(beam.gameObject, 0.08f);
        }

        if (target != null)
            OnHitOpponentHitscan(target.gameObject);
    }
    
    private bool IsUsingGamepad()
    {
        foreach (InputDevice device in inp.devices)
        {
            if (device is Gamepad)
                return true;
        }

        return false;
    }

    private PlayerStats FindAssistedHitscanTarget(
        Transform aim,
        int playerMask,
        PlayerStats shooter,
        float maxDistance,
        ref Vector3 endPoint)
    {
        RaycastHit[] nearbyHits = Physics.SphereCastAll(
            aim.position,
            hitscanAimAssistRadius,
            aim.forward,
            maxDistance,
            playerMask,
            QueryTriggerInteraction.Ignore);

        PlayerStats nearestPlayer = null;
        float nearestDistance = maxDistance;

        foreach (RaycastHit hit in nearbyHits)
        {
            if (hit.distance <= 0f || hit.distance >= nearestDistance)
                continue;

            PlayerStats candidate = hit.collider.GetComponentInParent<PlayerStats>();
            if (candidate == null || candidate == shooter)
                continue;

            // Check the angled path to the assisted hit for arena walls.
            if (Physics.Linecast(
                    aim.position, hit.point, 1 << 0, QueryTriggerInteraction.Ignore))
                continue;

            nearestPlayer = candidate;
            nearestDistance = hit.distance;
            endPoint = hit.point;
        }

        return nearestPlayer;
    }

    private void AdjustPlayerStats()
    {
        if (disableJump)
            currentPlayer.canJump = false;
        if (disableShove)
            currentPlayer.GetComponent<Shove>().blockedByHeldItem = true;
        
        currentPlayer.itemMovementMultiplier = walkSpeedModifier;
        currentPlayer.itemJumpMultiplier = jumpStrengthModifier;
    }

    private IEnumerator UseItemCooldown()
    {
        canUseItem = false;
        float timer = secondsBetweenUse;

        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        canUseItem = true;
    }

    private void UpdateHoldPosition()
    {
        Transform aim = currentPlayer.GetComponent<Shove>().playerCam.transform;
        Quaternion facing = Quaternion.Euler(0f, aim.eulerAngles.y, 0f);

        transform.SetPositionAndRotation(
            currentPlayer.holdItemPosition.position + facing * itemHoldPositionOffset,
            facing);
    }

    /// <summary>
    /// Use this method to equip this item to a player
    /// </summary>
    /// <param name="playerEquipping">The player equipping this item</param>
    public void EquipItem(PlayerController playerEquipping)
    {
        if (playerEquipping == null || isEquipped || currentPlayer != null)
            return;

        currentPlayer = playerEquipping;
        isEquipped = true;
        OnEquip();
    }
    
    /// <summary>
    /// <para>Called once when the player presses the [Attack] button</para>
    /// <para>Spawns a projectile if <see cref="shootsProjectile"/> is true</para>
    /// </summary>
    public virtual void OnUse()
    {
        Debug.Log($"Used {gameObject.name}");
    }

    /// <summary>
    /// <para>Called when an item that does NOT have a projectile hits an opponent</para>
    /// <para>Useful for applying knockback, effects, ragdolls and other various effects when hitting an opponent.</para>
    /// </summary>
    public virtual void OnHitOpponentMelee(GameObject playerHit)
    {
        
    }
    
    /// <summary>
    /// <para>Called when the hitscan projectile of an item hits an opponent</para>
    /// <para>Useful for applying knockback, effects, ragdolls and other various effects when hitting an opponent.</para>
    /// </summary>
    public virtual void OnHitOpponentHitscan(GameObject playerHit)
    {
        
    }
    
    /// <summary>
    /// Called once when the player equips up this item
    /// </summary>
    public virtual void OnEquip()
    {
        
    }

    /// <summary>
    /// Called every frame while this item is equipped by the player
    /// </summary>
    public virtual void OnHold()
    {
        
    }
    
    /// <summary>
    /// Called once when the player voluntarily drops this item. CALL base.OnDrop() FIRST!!!
    /// </summary>
    public virtual void OnDrop()
    {
        if (disableJump)
            currentPlayer.canJump = true;
        if (disableShove)
            currentPlayer.GetComponent<Shove>().blockedByHeldItem = false;
        
        currentPlayer.itemMovementMultiplier = 0;
        currentPlayer.itemJumpMultiplier = 0;
        isEquipped = false;
        currentPlayer = null;
        foundPlayerInput = false;
        inp = null;
    }

    private void DiscardItem()
    {
        OnDiscard();
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Called once when the item runs out of charges/uses and is discarded. CALL base.OnDiscard() FIRST!!!
    /// </summary>
    public virtual void OnDiscard()
    {
        if (disableJump)
            currentPlayer.canJump = true;
        if (disableShove)
            currentPlayer.GetComponent<Shove>().blockedByHeldItem = false;
        
        currentPlayer.itemMovementMultiplier = 0;
        currentPlayer.itemJumpMultiplier = 0;
        isEquipped = false;
        currentPlayer = null;
        foundPlayerInput = false;
        inp = null;
    }
    
    /// <summary>
    /// Helper method to have a projectile apply knockback
    /// </summary>
    /// <param name="playerTarget">The player that will get knocked back</param>
    /// <param name="knockbackForce">The knockback strength</param>
    public void ApplyKnockback(GameObject playerTarget, float knockbackForce, Vector3 direction)
    {
        Rigidbody rb = playerTarget.GetComponent<Rigidbody>();
        if (rb == null)
            return;
        
        rb.AddForce(direction * knockbackForce, ForceMode.Impulse);
    }
}
