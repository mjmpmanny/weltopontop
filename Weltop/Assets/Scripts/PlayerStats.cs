using System;
using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private SpawnpointManager spawnManager;
    private UI ui;

    public bool isInvincible;

    public int currentHillPoints;

    private float pointGivingCounter;

    private void Awake()
    {
        spawnManager = FindFirstObjectByType<SpawnpointManager>();
        ui = FindObjectOfType<UI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KillZone"))
        {
            Kill();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("HillZone"))
        {
            GiveHillPoints();
        }
    }

    private void GiveHillPoints()
    {
        pointGivingCounter += Time.deltaTime;

        if (pointGivingCounter >= GlobalParameters.DelayBetweenPointsSeconds)
        {
            currentHillPoints += 1;
            pointGivingCounter = 0;
            ui.UpdateHillPoints(this);
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
