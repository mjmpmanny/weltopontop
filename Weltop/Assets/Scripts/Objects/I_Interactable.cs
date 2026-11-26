using UnityEngine;

public interface I_Interactable
{
    /// <summary>
    /// Called when the player interacts with the object
    /// </summary>
    public void Interact(PlayerItemInteraction playerinteraction);

    /// <summary>
    /// Return weather or not the object can be interacted with
    /// </summary>
    /// <returns> Return weather or not the object can be interacted with</returns>
    public bool CanBeInteractedWith();
    /// <summary>
    /// Returns the position of the object
    /// </summary>
    public Vector3 GetPosition();

}
