using System.Net;
using UnityEngine;

public class HitscanProjectile : Projectile
{
    private void Start()
    {
        Physics.Raycast(transform.position,transform.forward, out RaycastHit hit);


        LineRenderer beam = GetComponent<LineRenderer>();
        beam.useWorldSpace = true;
        beam.widthMultiplier = 0.03f;
        beam.positionCount = 2;
        beam.SetPosition(0, hitscanMuzzle != null ? hitscanMuzzle.position : transform.position);
        beam.SetPosition(1, endPoint);
        Destroy(beam.gameObject, 0.08f);
    }
}
