using System;
using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private SpawnpointManager spawnManager;
    private PlayerController_3dPerson playerController;
    
    public Cam3dPerson playerCamera;

    public bool isInvincible;

    private void Awake()
    {
        playerController = GetComponent<PlayerController_3dPerson>();
        spawnManager = FindFirstObjectByType<SpawnpointManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KillZone"))
        {
            StartCoroutine(Kill());
        }
    }

    private IEnumerator Kill()
    {
        HideBody();
        
        yield return new WaitForSeconds(GlobalParameters.RespawnDelaySeconds);
        
        spawnManager.SendToSpawnpoint(gameObject);
        ShowBody();
        StartCoroutine(InvincibilityFrames());
    }

    private void HideBody()
    {
        playerController.canMove = false;
        playerCamera.canLook = false;
        
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
    }
    
    private void ShowBody()
    {
        playerController.canMove = true;
        playerCamera.canLook = true;
        
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = true;
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
