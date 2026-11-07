using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class PowerUp : MonoBehaviour
{
    private ObjectEffectSource effectSource;
    
    private PlaceableObject placeable;
    
    #region Exposed Parameters
    [Tooltip("How many times can this power-up be obtained by players before deactivating for the round (0 = infinite)")]
    public int useAmount = 1;
    [Tooltip("How many seconds does the player keep this power up for (0 = infinite)")]
    public float secondsActive = 0;
    public float cooldownSeconds = 0;
    #endregion

    private bool isActive;
    
    private void Awake()
    {
        isActive = true;
        effectSource = GetComponent<ObjectEffectSource>();
        placeable = GetComponent<PlaceableObject>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (placeable != null && placeable.IsBeingPlaced)
            return;
        
        if (other.CompareTag("Player"))
        {
            if(isActive) 
                OnTouch(other.gameObject);
        }
    }

    private void Update()
    {
        if (placeable != null && placeable.IsBeingPlaced)
            return;
        
        //While on cooldown, calls OnCooldown() every frame
        if (!isActive)
        {
            OnCooldown();
        }
        else
        {
            OnCurrentlyActive();
        }
    }

    /// <summary>
    /// Called when the player touches the trigger collider of this object while it is Active
    /// </summary>
    /// <param name="playerTarget">The player gameobject that enters the trigger collider</param>
    public virtual void OnTouch(GameObject playerTarget)
    {
        PlayerObjectEffects player = playerTarget.GetComponent<PlayerObjectEffects>();
        
        //Some collider buildables may not have an effect source because it doesn't do anything
        if(effectSource != null)
            effectSource.ApplyTo(player);
        
        //If this power-up has a cooldown, start it
        if(cooldownSeconds > 0)
            StartCoroutine(StartCooldown());
        
        //If this power-up has active seconds, start that countdown
        if (secondsActive > 0)
            StartCoroutine(StartActiveSecondsCountdown(player));
    }
    
    /// <summary>
    /// Called every frame while the object is active. Useful for effects while active
    /// </summary>
    public virtual void OnCurrentlyActive()
    {
        
    }
    
    /// <summary>
    /// Called once when the object exits its cooldown. Useful for effects/sounds when it activates
    /// </summary>
    public virtual void OnActivate()
    {
        
    }
    
    /// <summary>
    /// Called once when the object enters its cooldown. Useful for effects/sounds when it deactivates
    /// </summary>
    public virtual void OnDeactivate()
    {
        
    }
    
    /// <summary>
    /// Called every frame while this object is on cooldown. Useful for effects while on cooldown
    /// </summary>
    public virtual void OnCooldown()
    {
        
    }

    /// <summary>
    /// <para>Called once when <see cref="secondsActive"/> lapses.</para>
    /// <para>Make sure to call base.OnPlayerLosePowerUp first</para>
    /// </summary>
    public virtual void OnPlayerLosePowerUp(PlayerObjectEffects playerEffects)
    {
        effectSource.ReleaseFrom(playerEffects);
    }

    private IEnumerator StartActiveSecondsCountdown(PlayerObjectEffects playerEffects)
    {
        //Calls when first entering cooldown
        float timer = secondsActive;

        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        //Calls when first exiting cooldown
        OnPlayerLosePowerUp(playerEffects);
    }
    
    private IEnumerator StartCooldown()
    {
        isActive = false;
        //Calls when first entering cooldown
        OnDeactivate();
        float timer = cooldownSeconds;

        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        isActive = true;
        //Calls when first exiting cooldown
        OnActivate();
    }
}
