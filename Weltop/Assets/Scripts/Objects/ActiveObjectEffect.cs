using UnityEngine;

// Runtime data for one effect applied to one player
// This is not a component or a ScriptableObject
public class ActiveObjectEffect
{
    public ObjectEffectDefinition Definition { get; }
    public GameObject Source { get; }
    public float RemainingSeconds { get; private set; }

    public bool HasExpired =>
        Source == null || RemainingSeconds <= 0f;

    public ActiveObjectEffect(
        ObjectEffectDefinition definition,
        GameObject source)
    {
        Definition = definition;
        Source = source;
        RemainingSeconds = definition.DurationSeconds;
    }

    public void Tick(float deltaTime)
    {
        RemainingSeconds =
            Mathf.Max(0f, RemainingSeconds - deltaTime);
    }
}