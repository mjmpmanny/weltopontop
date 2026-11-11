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
            GameObject target = FindPlayerToTarget(999);
            
            if(target != null)
                OnHitOpponentHitscan(target);
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
        gameObject.transform.position = currentPlayer.holdItemPosition.position + itemHoldPositionOffset;
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
