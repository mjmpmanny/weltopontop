using UnityEngine;

public enum ObjectEffectType
{
    MovementSpeed,
    JumpStrength,
    Impulse
}

[CreateAssetMenu(
    fileName = "NewObjectEffect",
    menuName = "WELTOP/Objects/Object Effect")]
public class ObjectEffectDefinition : ScriptableObject
{
    [SerializeField] private ObjectEffectType effectType;

    [Tooltip("Used by movement and jump effects. 1 means unchanged.")]
    [SerializeField, Min(0f)] private float multiplier = 1f;

    [Tooltip("Duration of movement and jump effects, in seconds.")]
    [SerializeField, Min(0f)] private float durationSeconds = 3f;

    [Tooltip("Instant force in the activating object's local direction.")]
    [SerializeField] private Vector3 localImpulse = Vector3.up * 10f;

    public ObjectEffectType EffectType => effectType;
    public float Multiplier => multiplier;
    public float DurationSeconds => durationSeconds;
    public Vector3 LocalImpulse => localImpulse;
}