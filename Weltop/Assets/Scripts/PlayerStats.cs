using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    private SpawnpointManager spawnManager;
    private UI ui;
    private PlayerInput playerInput;

    public bool isInvincible;
    public int currentHillPoints;

    private float pointGivingCounter;
    private bool isInsideHill;

    public int PlayerIndex =>
        playerInput != null ? playerInput.playerIndex : -1;

    private void Awake()
    {
        spawnManager = FindFirstObjectByType<SpawnpointManager>();
        ui = FindFirstObjectByType<UI>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        // Display the player's score when they join.
        if (ui != null)
        {
            ui.UpdateHillPoints(this);
        }
    }

    private void Update()
    {
        if (!isInsideHill)
            return;

        float interval = GlobalParameters.DelayBetweenPointsSeconds;

        if (interval <= 0f)
            return;

        pointGivingCounter += Time.deltaTime;

        // Award points at consistent intervals.
        while (pointGivingCounter >= interval)
        {
            currentHillPoints++;
            pointGivingCounter -= interval;

            if (ui != null)
            {
                ui.UpdateHillPoints(this);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KillZone"))
        {
            Kill();
        }
        else if (other.CompareTag("HillZone"))
        {
            isInsideHill = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("HillZone"))
        {
            isInsideHill = false;
            pointGivingCounter = 0f;
        }
    }

    private void Kill()
    {
        // Stop scoring when the player dies.
        isInsideHill = false;
        pointGivingCounter = 0f;

        spawnManager.SendToSpawnpoint(gameObject);
        StartCoroutine(InvincibilityFrames());
    }

    private IEnumerator InvincibilityFrames()
    {
        Debug.Log("Enabled InvincibilityFrames");
        isInvincible = true;

        float time = 0f;

        while (time < GlobalParameters.InvincibilityFramesSeconds)
        {
            time += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Disabled InvincibilityFrames");
        isInvincible = false;
    }
}