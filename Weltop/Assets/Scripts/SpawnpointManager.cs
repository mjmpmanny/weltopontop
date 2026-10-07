using UnityEngine;

public class SpawnpointManager : MonoBehaviour
{
    //If we do random...
    //public Transform[] spawnpoints;
    
    //If we don't...
    public Transform spawnpoint;

    public void SendToSpawnpoint(GameObject objectToSend)
    {
        objectToSend.transform.position = spawnpoint.position;
    }

    public void SendToSpawnpointsRandom(GameObject objectToSend)
    {
        //spawnpoints random code
    }
}
