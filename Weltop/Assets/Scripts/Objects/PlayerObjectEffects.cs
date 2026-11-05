using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerObjectEffects : MonoBehaviour
{
    public float MovementMultiplier { get; private set; } = 1f;
    public float JumpMultiplier { get; private set; } = 1f;

    private readonly List<ActiveObjectEffect> activeEffects =
        new List<ActiveObjectEffect>();

    private PlayerStats playerStats;
    private RagdollController ragdollController;
    private Rigidbody rb;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        ragdollController = GetComponent<RagdollController>();
        rb = GetComponent<Rigidbody>();
    }

    public bool ApplyEffect(
        ObjectEffectDefinition definition,
        GameObject source)
    {
        if (!isActiveAndEnabled || definition == null || source == null)
            return false;

        if (playerStats != null && playerStats.IsDead)
            return false;

        if (definition.EffectType == ObjectEffectType.Impulse)
            return ApplyImpulse(definition, source);

        if (definition.DurationSeconds <= 0f)
            return false;

        if (definition.EffectType != ObjectEffectType.MovementSpeed &&
            definition.EffectType != ObjectEffectType.JumpStrength)
        {
            return false;
        }

        // Reapplying the same effect from the same source refreshes it.
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            if (activeEffects[i].Definition == definition &&
                activeEffects[i].Source == source)
            {
                activeEffects.RemoveAt(i);
            }
        }

        activeEffects.Add(new ActiveObjectEffect(definition, source));
        RecalculateMultipliers();
        return true;
    }

    private bool ApplyImpulse(
        ObjectEffectDefinition definition,
        GameObject source)
    {
        if (playerStats != null && playerStats.isInvincible)
            return false;

        if (ragdollController != null && ragdollController.isGetingUp)
            return false;

        Rigidbody targetBody = ragdollController != null
            ? ragdollController.CurrentRigidBody()
            : rb;

        if (targetBody == null || targetBody.isKinematic)
            return false;

        Vector3 impulse =
            source.transform.TransformDirection(definition.LocalImpulse);

        if (definition.causesRagdoll)
        {
            //THIS DOESN'T WORK!!! Please fix
            ragdollController.Ragdoll(transform.position, definition.LocalImpulse * definition.Multiplier);
        }
        else
        {
            targetBody.AddForce(impulse, ForceMode.Impulse);
        }
        return true;
    }

    public void ReleaseSource(GameObject source)
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            if (activeEffects[i].Source == source)
                activeEffects.RemoveAt(i);
        }

        RecalculateMultipliers();
    }

    private void Update()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            ActiveObjectEffect effect = activeEffects[i];
            effect.Tick(Time.deltaTime);

            if (effect.HasExpired || effect.Definition == null)
                activeEffects.RemoveAt(i);
        }

        RecalculateMultipliers();
    }

    private void RecalculateMultipliers()
    {
        MovementMultiplier = 1f;
        JumpMultiplier = 1f;

        foreach (ActiveObjectEffect effect in activeEffects)
        {
            float multiplier =
                Mathf.Max(0f, effect.Definition.Multiplier);

            switch (effect.Definition.EffectType)
            {
                case ObjectEffectType.MovementSpeed:
                    MovementMultiplier *= multiplier;
                    break;

                case ObjectEffectType.JumpStrength:
                    JumpMultiplier *= multiplier;
                    break;
            }
        }
    }

    public void ClearEffects()
    {
        activeEffects.Clear();
        RecalculateMultipliers();
    }

    private void OnDisable()
    {
        ClearEffects();
    }
}