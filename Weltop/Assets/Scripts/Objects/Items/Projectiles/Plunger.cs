using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Plunger : BasicProjectile
{
    private float timer = 0;
    private bool hasRetracted;

    public float plungeStrength;
    
    public override void OnFlying()
    {
        timer += Time.deltaTime;

        //Reduce or increase this float to make the range of the plunger closer or further
        if (timer >= 0.6f && !hasRetracted)
        {
            RetractPlunger();
        }
    }

    private void RetractPlunger()
    {
        hasRetracted = true;
        //Change duration to make the plunger retrack back to the player faster
        transform.DOMove(playerOwner.transform.position, 0.4f)
            .SetEase(Ease.OutSine)
            .OnComplete(() => KillSelf());
    }
    
    public override void OnHitOpponent(GameObject playerTarget)
    {
        if (hasRetracted)
            return;
        
        ApplyKnockback(playerTarget, plungeStrength, (playerOwner.transform.position - playerTarget.transform.position).normalized + new Vector3(0, 0.75f, 0));
        RetractPlunger();
    }

    public override void OnHitWall(GameObject wallHit)
    {
        if (hasRetracted)
            return;
        
        RetractPlunger();
    }
}
