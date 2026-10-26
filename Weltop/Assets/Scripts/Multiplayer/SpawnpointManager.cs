using System.Collections.Generic;
using UnityEngine;

public class SpawnpointManager : MonoBehaviour
{
    public Transform spawnpoint;
    public Transform[] spawnpoints;

    private readonly Dictionary<GameObject, Transform> playerSpawns = new();

    public void SendToSpawnpoint(GameObject objectToSend)
    {
        objectToSend.transform.position = spawnpoint.position;
    }

    public void SendToSpawnpointsRandom(GameObject objectToSend)
    {
        if (spawnpoints == null || spawnpoints.Length == 0)
        {
            SendToSpawnpoint(objectToSend);
            return;
        }

        List<Transform> availableSpawns = new List<Transform>();

        foreach (Transform point in spawnpoints)
        {
            if (point != null && !playerSpawns.ContainsValue(point))
            {
                availableSpawns.Add(point);
            }
        }

        if (availableSpawns.Count == 0)
        {
            availableSpawns.AddRange(spawnpoints);
        }

        Transform selected = availableSpawns[Random.Range(0, availableSpawns.Count)];

        playerSpawns[objectToSend] = selected;
        objectToSend.GetComponent<RagdollController>().MovePlayer(selected.position);
    }

    public void ResetSpawnAssignments()
    {
        playerSpawns.Clear();
    }
}