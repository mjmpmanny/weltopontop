using UnityEngine;

public class PlacedObjectEffectTrigger : MonoBehaviour
{
    [SerializeField] private PlaceableObject placeable;
    [SerializeField] private ObjectEffectSource effectSource;

    private void OnTriggerEnter(Collider other)
    {
        if (!placeable.IsPlaced)
            return;

        PlayerObjectEffects player = other.GetComponentInParent<PlayerObjectEffects>();
        if (player != null)
            effectSource.ApplyTo(player);
    }
}