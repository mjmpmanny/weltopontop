using DG.Tweening;
using System;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Component))]
public class MeleeBaseItem : BasicBaseItem
{
    [SerializeField] protected Transform swingVisual;
    protected Sequence swingSequence;
    protected Quaternion swingRestRotation;

    [Header("Melee Settings")]
    [Tooltip("The cooldown inbetween swings")]
    public float swingCooldown;
    protected float swingCooldownTimer;
    [Tooltip("The force applyed to the player when hit")]
    public float hitForce;


    private void Update()
    {
        swingCooldownTimer -= Time.deltaTime;
    }
    public override void UseItem(Vector3 origin, Vector3 rotation)
    {

        if (swingVisual == null || swingCooldownTimer >0)
            return;

        if (swingSequence != null && swingSequence.IsActive())
        {
            swingSequence.Kill();
            swingVisual.localRotation = swingRestRotation;
        }

        swingRestRotation = swingVisual.localRotation;
        Quaternion swingRotation =
            swingRestRotation * Quaternion.Euler(90f, 0, 0f);

        swingSequence = DOTween.Sequence()
            .Append(swingVisual.DOLocalRotateQuaternion(swingRotation, 0.09f))
            .Append(swingVisual.DOLocalRotateQuaternion(swingRestRotation, 0.16f))
            .SetLink(gameObject);

        player.animationController.PlayAttackAnimation();
        GameObject target = FindPlayerToTarget(2, origin, rotation);
        if (target != null)
        {
            target.transform.root.GetComponentInChildren<RagdollController>().Ragdoll(player.transform.position, hitForce * transform.forward);
        }

        swingCooldownTimer = swingCooldown;
    }

    private GameObject FindPlayerToTarget(int raycastDistance, Vector3 origin, Vector3 forward)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, forward, out hit, raycastDistance, ((15<<6) & ~player.gameObject.layer)))
        {
            return hit.collider.gameObject;
        } else
        {
            return null;
        }
    }

}
