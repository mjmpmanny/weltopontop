using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlaceableObject))]
public class PlacedItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform spawnPoint;

    private int lastSpawnedParkourRound = -1;

    public void SpawnIfPlaced(int parkourRound)
    {
        if (lastSpawnedParkourRound == parkourRound ||
            !GetComponent<PlaceableObject>().IsPlaced)
            return;

        if (itemPrefab == null || spawnPoint == null)
        {
            Debug.LogError("Assign an item prefab and spawn point.", this);
            return;
        }

        GameObject obj = Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation);
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.excludeLayers = (15 << 6);
        lastSpawnedParkourRound = parkourRound;
    }
}