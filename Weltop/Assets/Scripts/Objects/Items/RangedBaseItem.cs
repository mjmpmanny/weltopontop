using DG.Tweening;
using System;
using Unity.Collections;
using Unity.Multiplayer.PlayMode;
using UnityEngine;
using UnityEngine.ProBuilder;

public class RangedBaseItem : BasicBaseItem
{


    [Header("Ranged Settings")]
    public Vector3 MussleOffset;
    public Projectile projectile;
    public int AmmoAmount;

    public override void UseItem(Vector3 origin, Vector3 rotation)
    {
        Projectile instantiatedProjectile = Instantiate(projectile, origin, Quaternion.LookRotation(rotation,Vector3.up),transform);
        instantiatedProjectile.playerOwner = player.gameObject;
    }
    protected override void OnCollision(Collision collision)
    {
        if (collision.collider.gameObject.layer == 0)
        {

            collider.excludeLayers = (15 << 6);//Collider cant interact players when not being thrown (can be changed if funny)
        } else if (collision.collider.gameObject.layer >= 6 && collision.collider.gameObject.layer <= 9 && isBeingThrown)
        {
            Shove.ApplyShoveHit(collision.collider.gameObject, transform.position, rb.linearVelocity);
        }
        isBeingThrown = false;
        if (AmmoAmount <= 0)
        {
            //base.DestroyItem();
        }
    }

}
