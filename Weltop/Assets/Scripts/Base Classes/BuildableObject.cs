using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class BuildableObject : MonoBehaviour
{
    private ObjectEffectSource effectSource;
    
    #region Exposed Parameters
    public BuildObjectType buildObjectType;
    public float secondsBetweenStates = 0;
    //Oscillates between 0 and 1
    public int currentState = 0;
    public float cooldownSeconds = 0;
    /// <summary>
    /// If this is true, then <see cref="secondsBetweenStates"/> acts as the time it takes for this object travel between its two movement points
    /// </summary>
    public bool movingObject = false;
    /// <summary>
    /// If <see cref="movingObject"/> is true, the object will oscillate between its starting position and the position of this variable
    /// </summary>
    public Transform movingObjectDestinationPoint;
    
    /// <summary>
    /// Necessary if the object has multiple states. There should be an animation for entering state 2 from state 1, and vice versa.
    /// </summary>
    public bool animatingObject = false;
    /// <summary>
    /// Each object that animates should have its own animator with its own animations.
    /// </summary>
    public Animator animator;
    #endregion

    private float stateCountdownSeconds;
    private bool isActive;
    
    public enum BuildObjectType
    {
        Displacement,
        Kill,
        Collider
    }

    private void Awake()
    {
        stateCountdownSeconds = secondsBetweenStates;
        isActive = true;
        effectSource = GetComponent<ObjectEffectSource>();
        
        if(movingObject)
            TriggerMovingObject();
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

        //If this object has a value for secondsBetweenStates, this counter is triggered
        if (secondsBetweenStates > 0)
        {
            CountdownStateChange(secondsBetweenStates);
        }
    }

    private void CountdownStateChange(float initialSeconds)
    {
        stateCountdownSeconds -= Time.deltaTime;

        if (stateCountdownSeconds <= 0)
        {
            OnStateChange();
            
            currentState = currentState == 0 ? 1 : 0;
            
            stateCountdownSeconds = initialSeconds;
        }
    }

    private void TriggerMovingObject()
    {
        transform.DOMove(movingObjectDestinationPoint.position, secondsBetweenStates)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
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

    /// <summary>
    /// <para>Called once every time the <see cref="secondsBetweenStates"/> lapses.</para>
    /// <para>Useful for calling animations or effects.</para>
    /// </summary>
    public virtual void OnStateChange()
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
