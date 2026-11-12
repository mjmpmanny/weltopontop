using UnityEngine;

public class TestProjectile : Projectile
{
    public override void OnHitOpponent(GameObject playerTarget)
    {
        Shove.ApplyShoveHit(playerTarget, GetComponent<Rigidbody>().linearVelocity);
    }
}
