using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BuildableObject : MonoBehaviour
{
    private ObjectEffectSource effectSource;
    
    #region Exposed Parameters
    public BuildObjectType buildObjectType;
    public float secondsBetweenStates;
    public float cooldownSeconds;
    #endregion
    
    private bool isActive;
    
    public enum BuildObjectType
    {
        Displacement,
        Kill,
        Collider
    }

    private void Awake()
    {
        isActive = true;
        effectSource = GetComponent<ObjectEffectSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(isActive) 
                OnTouch(other.gameObject);
        }
    }

    private void Update()
    {
        //While on cooldown, calls OnCooldown() every frame
        if (!isActive)
        {
            OnCooldown();
        }
    }

    /// <summary>
    /// Called when the player touches the trigger collider of this object
    /// </summary>
    /// <param name="playerTarget">The player gameobject that enters the trigger collider</param>
    public virtual void OnTouch(GameObject playerTarget)
    {
        PlayerObjectEffects player = playerTarget.GetComponent<PlayerObjectEffects>();
        
        //Some collider buildables may not have an effect source because it doesn't do anything
        if(effectSource != null)
            effectSource.ApplyTo(player);
        
        //If this object has a cooldown, start it
        if(cooldownSeconds > 0)
            StartCoroutine(StartCooldown());
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
