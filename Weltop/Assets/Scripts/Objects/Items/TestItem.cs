using UnityEngine;
using DG.Tweening;

public class TestItem : Item
{
    
    [SerializeField] private Transform swingVisual;

    private Sequence swingSequence;
    private Quaternion swingRestRotation;

    public override void OnUse()
    {
        base.OnUse();

        if (swingVisual == null)
            return;

        if (swingSequence != null && swingSequence.IsActive())
        {
            swingSequence.Kill();
            swingVisual.localRotation = swingRestRotation;
        }

        swingRestRotation = swingVisual.localRotation;
        Quaternion swingRotation =
            swingRestRotation * Quaternion.Euler(0f, 90f, 0f);

        swingSequence = DOTween.Sequence()
            .Append(swingVisual.DOLocalRotateQuaternion(swingRotation, 0.09f))
            .Append(swingVisual.DOLocalRotateQuaternion(swingRestRotation, 0.16f))
            .SetLink(gameObject);
    }
    
    public override void OnHitOpponentMelee(GameObject playerHit)
    {
        Shove.ApplyShoveHit(
            playerHit, currentPlayer.GetComponent<Shove>().playerCam.transform.forward);
    }
}