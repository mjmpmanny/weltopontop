using System.Collections.Generic;
using UnityEngine;

public class ObjectEffectSource : MonoBehaviour
{
    [SerializeField] private ObjectDefinition definition;

    [Header("Manual Testing")]
    [SerializeField] private PlayerObjectEffects testTarget;

    private readonly HashSet<PlayerObjectEffects> affectedPlayers =
        new HashSet<PlayerObjectEffects>();

    public bool ApplyTo(PlayerObjectEffects target)
    {
        if (!isActiveAndEnabled || definition == null || target == null)
            return false;

        bool applied = false;

        foreach (ObjectEffectDefinition effect in definition.Effects)
        {
            if (target.ApplyEffect(effect, gameObject))
                applied = true;
        }

        if (applied)
            affectedPlayers.Add(target);

        return applied;
    }

    public void ReleaseFrom(PlayerObjectEffects target)
    {
        if (target == null)
            return;

        target.ReleaseSource(gameObject);
        affectedPlayers.Remove(target);
    }

    [ContextMenu("Apply Effects To Test Target")]
    private void ApplyToTestTarget()
    {
        if (Application.isPlaying)
            ApplyTo(testTarget);
    }

    [ContextMenu("Release Effects From Test Target")]
    private void ReleaseTestTarget()
    {
        if (Application.isPlaying)
            ReleaseFrom(testTarget);
    }

    private void OnDisable()
    {
        foreach (PlayerObjectEffects target in affectedPlayers)
        {
            if (target != null)
                target.ReleaseSource(gameObject);
        }

        affectedPlayers.Clear();
    }
}