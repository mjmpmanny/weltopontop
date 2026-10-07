using System;
using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private SpawnpointManager spawnManager;

    public bool isInvincible;

    private void Awake()
    {
        spawnManager = FindFirstObjectByType<SpawnpointManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KillZone"))
        {
            Kill();
        }
    }

    private void Kill()
    {
        spawnManager.SendToSpawnpoint(gameObject);
        StartCoroutine(InvincibilityFrames());
    }

    IEnumerator InvincibilityFrames()
    {
        Debug.Log("Enabled InvincibilityFrames");
        isInvincible = true;
        float time = 0;

        while (time < GlobalParameters.InvincibilityFramesSeconds)
        {
            time += Time.deltaTime;
            yield return null;
        }
        
        Debug.Log("Disabled InvincibilityFrames");
        isInvincible = false;
    }
}
