using UnityEngine;


public interface I_Item
{
    /// <summary>
    /// Equips item to a player.
    /// </summary>
    /// <param name="player">The player to equip to</param>
    /// <param name="toParentTo">The Object that the item follows when it is equiped (ie. the camera)</param>
    public void EquipItem(PlayerController player, GameObject toParentTo);
    /// <summary>
    /// Is called to use the item
    /// </summary>
    /// <param name="origin">Stores the direction and position where the item is being used</param>
    public void UseItem(Transform origin);
    /// <summary>
    /// Is called to throw the item
    /// </summary>
    /// <param name="origin">Stores the direction and position where the item is being thrown</param>
    public void ThrowItem(Transform origin);
    /// <summary>
    /// Drops the item at pos
    /// </summary>
    /// <param name="origin">The place the item is dropped</param>
    public void DropItem(Vector3 pos);
    /// <summary>
    /// Returns if the item is able to ba picked up
    /// </summary>
    public bool CanPickUp();
    /// <summary>
    /// Returns the position of the item
    /// </summary>
    public Vector3 GetPosition();

    /// <summary>
    /// Returns if the object is being held by a player
    /// </summary>
    /// <param name="player">The player to check</param>
    public bool IsBeginHeldBy(PlayerController player);
}
