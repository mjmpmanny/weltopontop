using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Plunger : Projectile
{
    private float timer = 0;
    private bool hasRetracted;
    
    public override void OnFlying()
    {
        timer += Time.deltaTime;

        if (timer >= 0.6f && !hasRetracted)
        {
            RetractPlunger();
            hasRetracted = true;
        }
    }

    private void RetractPlunger()
    {
        transform.DOMove(playerOwner.transform.position, 0.4f)
            .SetEase(Ease.OutSine)
            .OnComplete(() => KillSelf());
    }
    
    public override void OnHitOpponent(GameObject playerTarget)
    {
        RetractPlunger();
    }

    public override void OnHitWall(GameObject wallHit)
    {
        RetractPlunger();
    }
}
