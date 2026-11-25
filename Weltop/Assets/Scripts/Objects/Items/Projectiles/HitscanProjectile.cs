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


        LineRenderer beam = GetComponent<LineRenderer>();
        beam.widthMultiplier = 0.03f;
        beam.positionCount = 2;
        beam.SetPosition(0, Vector3.zero);
        beam.SetPosition(1, Quaternion.Inverse(transform.rotation) * endPoint);
        Destroy(beam.gameObject, 0.08f);
        
    }

}
