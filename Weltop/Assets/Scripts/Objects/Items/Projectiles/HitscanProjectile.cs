using System.Net;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class HitscanProjectile : Projectile
{
    public float HitStrength;
    public Vector3 HitPosition;
    private void Start()
    {

        Vector3 endPoint = HitPosition - transform.position;
        Collider[] cols = Physics.OverlapBox(HitPosition, Vector3.one * 0.1f, Quaternion.identity, ~((1 << playerOwner.gameObject.layer) + (1 << 16)));
        if (cols.Length != 0)
        {
            foreach (Collider col in cols)
            {
                if (col.gameObject.layer >= 6 && col.gameObject.layer <= 9)
                {
                    HitPlayer(col, transform.position);
                }
            }
        }


        LineRenderer beam = GetComponent<LineRenderer>();
        beam.widthMultiplier = 0.03f;
        beam.positionCount = 2;
        beam.SetPosition(0, Vector3.zero);
        beam.SetPosition(1, Quaternion.Inverse(transform.rotation) * endPoint);
        Destroy(beam.gameObject, 0.08f);
        
    }
    public void HitPlayer(Collider col,Vector3 pos)
    {
        Shove.ApplyShoveHit(col.gameObject,pos,transform.forward);
    }

}
