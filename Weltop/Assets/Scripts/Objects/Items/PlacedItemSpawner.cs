using UnityEngine;

[RequireComponent(typeof(PlaceableObject))]
public class PlacedItemSpawner : MonoBehaviour
{
    [SerializeField] private Item itemPrefab;
    [SerializeField] private Transform spawnPoint;

    private bool hasSpawned;

    public void SpawnIfPlaced()
    {
        if (hasSpawned || !GetComponent<PlaceableObject>().IsPlaced)
            return;

        if (itemPrefab == null || spawnPoint == null)
        {
            Debug.LogError("Assign an item prefab and spawn point.", this);
            return;
        }

        Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation);
        hasSpawned = true;
    }
}