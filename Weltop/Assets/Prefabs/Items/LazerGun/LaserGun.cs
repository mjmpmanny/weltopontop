using Unity.VisualScripting;
using UnityEngine;

public class LazerGun : RangedBaseItem
{
    public override void UseItem(Vector3 origin, Vector3 forward)
    {
        Vector3 norigin = transform.position + transform.rotation * MussleOffset;
        RaycastHit hit;
        bool raycast = Physics.Raycast(origin, forward, out hit, 1000, ~(1 << player.gameObject.layer));
        
        UseVisual(norigin, forward, raycast ? hit.point : origin + forward * 1000);
        base.fakeItem.UseVisual(norigin, forward, raycast ? hit.point : origin + forward * 1000);
        if (raycast) {
            CheckIfHitPlayer(hit,forward);
        }


    }

    public void CheckIfHitPlayer(RaycastHit hit,Vector3 direction)
    {
        if (hit.collider.gameObject.layer >= 6 && hit.collider.gameObject.layer <= 9)
        {   
            Shove.ApplyShoveHit(hit.collider.gameObject,hit.point, direction);
        }
    }

    public override void UseVisual(Vector3 origin, Vector3 forward, Vector3 endpoint)
    {

        Vector3 norigin = MussleOffset;
        Projectile instantiatedProjectile = Instantiate(projectile, norigin, Quaternion.LookRotation(forward, Vector3.up), transform);
        instantiatedProjectile.transform.localPosition = norigin;
        instantiatedProjectile.gameObject.layer = gameObject.layer;
        if (instantiatedProjectile is HitscanProjectile)
        {
            ((HitscanProjectile)instantiatedProjectile).HitPosition = endpoint;
        }

    }
}
