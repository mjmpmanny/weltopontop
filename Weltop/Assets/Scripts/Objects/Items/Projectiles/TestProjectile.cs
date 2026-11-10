using UnityEngine;

public class TestProjectile : Projectile
{
    public override void OnHitOpponent(GameObject playerTarget)
    {
        Debug.Log($"Successfully hit a player with {gameObject.name}");
    }
}
