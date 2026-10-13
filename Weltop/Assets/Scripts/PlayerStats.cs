
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    private SpawnpointManager spawnManager;
    private UI ui;
    private PlayerInput playerInput;
    private PlayerController_3dPerson playerController;

    public Cam3dPerson playerCamera;

    public bool isInvincible;
    public int currentHillPoints;

    private float pointGivingCounter;
    private bool isInsideHill;
    private bool isDead;

    public int PlayerIndex =>
        playerInput != null ? playerInput.playerIndex : -1;

    private void Awake()
    {
        spawnManager = FindFirstObjectByType<SpawnpointManager>();
        ui = FindFirstObjectByType<UI>();
        playerInput = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController_3dPerson>();
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
        if (!isInsideHill || isDead)
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
        if (other.CompareTag("KillZone") && !isDead)
        {
            StartCoroutine(Kill());
        }
        else if (other.CompareTag("HillZone") && !isDead)
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

    private IEnumerator Kill()
    {
        isDead = true;

        // Stop scoring when the player dies.
        isInsideHill = false;
        pointGivingCounter = 0f;

        HideBody();

        yield return new WaitForSeconds(GlobalParameters.RespawnDelaySeconds);

        spawnManager.SendToSpawnpoint(gameObject);
        ShowBody();

        StartCoroutine(InvincibilityFrames());
        isDead = false;
    }

    private void HideBody()
    {
        playerController.canMove = false;
        playerCamera.canLook = false;

        // Hide the visible character model, including child renderers.
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
    }

    private void ShowBody()
    {
        playerController.canMove = true;
        playerCamera.canLook = true;

        // Restore the visible character model.
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = true;
        }
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
