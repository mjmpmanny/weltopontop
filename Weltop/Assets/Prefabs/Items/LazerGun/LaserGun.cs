using Unity.VisualScripting;
using UnityEngine;

public class LazerGun : RangedBaseItem
{
    public override void UseItem(Vector3 origin, Vector3 forward)
    {
        Vector3 norigin = transform.position + transform.rotation * MussleOffset;
        Vector3 nforward = forward;
        RaycastHit hit;
        bool raycast = Physics.Raycast(origin, forward, out hit, 1000, ~(1 << gameObject.layer));
        if (raycast)
        {
            nforward = hit.point - norigin;
        } 
        Projectile instantiatedProjectile = Instantiate(projectile, norigin, Quaternion.LookRotation(nforward, Vector3.up), transform);
        instantiatedProjectile.playerOwner = player.gameObject;
        if (instantiatedProjectile is HitscanProjectile)
        {
            ((HitscanProjectile)instantiatedProjectile).HitPosition = raycast ? hit.point : norigin + nforward * 1000;
        }
    }
}
