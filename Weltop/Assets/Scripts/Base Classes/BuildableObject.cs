using UnityEngine;

public class BuildableObject : MonoBehaviour
{
    public BuildObjectType buildObjectType;

    public float secondsBetweenStates;
    public float cooldownSeconds;
    public float knockbackForce;

    public enum BuildObjectType
    {
        Displacement,
        Kill,
        Collider
    }
    
    public virtual void OnTouch()
    {
        
    }
    
    public virtual void OnKnockPlayer()
    {
        
    }
    
    public virtual void OnActivate()
    {
        
    }
    
    public virtual void OnDeactivate()
    {
        
    }
    
    public virtual void OnCooldown()
    {
        
    }
}
