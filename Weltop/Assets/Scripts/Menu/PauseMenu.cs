using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem.UI;
using System.Linq;

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject pauseMenuContainer;
    [SerializeField] private GameObject controllerGuidePanel;

    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button controllerGuideButton;
    [SerializeField] private Button backButton;

    [Header("Scene")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused;
    private bool showingGuide;
    private float previousTimeScale = 1f;
    private PlayerInput pauseOwner;
    
    [SerializeField] private InputSystemUIInputModule uiInputModule;

    private InputActionAsset pauseUIActions;
    private InputDevice[] originalUIDevices;
    private InputActionAsset originalUIActions;

    private void Start()
    {
        pauseMenuPanel.SetActive(false);
        controllerGuidePanel.SetActive(false);
        isPaused = false;
    }

    
    private void Update()
    {
        PlayerInput player = GetPlayerPressingPause();

        if (player == null)
            return;

        if (!isPaused)
        {
            pauseOwner = player;
            OpenPauseMenu();

            Debug.Log("Pause opened by Player " +
                      (pauseOwner.playerIndex + 1));
        }
        else if (player == pauseOwner)
        {
            if (showingGuide)
                CloseControllerGuide();
            else
                ResumeGame();
        }
    }

    private PlayerInput GetPlayerPressingPause()
    {
        foreach (PlayerInput player in PlayerInput.all)
        {
            foreach (InputDevice device in player.devices)
            {
                if (device is Keyboard keyboard &&
                    keyboard.escapeKey.wasPressedThisFrame)
                {
                    return player;
                }

                if (device is Gamepad gamepad &&
                    gamepad.startButton.wasPressedThisFrame)
                {
                    return player;
                }
            }
        }

        return null;
    }
    
    private void RestrictUIToPauseOwner()
    {
        if (uiInputModule == null || pauseOwner == null)
            return;
        
        if (pauseUIActions == null)
        {
            originalUIActions = uiInputModule.actionsAsset;
            pauseUIActions = Instantiate(uiInputModule.actionsAsset);
            originalUIDevices = pauseUIActions.devices?.ToArray();
        }

        pauseUIActions.devices = pauseOwner.devices.ToArray();

        uiInputModule.actionsAsset = pauseUIActions;
    }
    
    private void RestoreUIInput()
    {
        if (uiInputModule != null && originalUIActions != null)
            uiInputModule.actionsAsset = originalUIActions;

        if (pauseUIActions != null)
        {
            Destroy(pauseUIActions);
            pauseUIActions = null;
        }

        originalUIActions = null;
        pauseOwner = null;
    }

    public void OpenPauseMenu()
    {
        if (isPaused)
            return;

        isPaused = true;
        showingGuide = false;
        RestrictUIToPauseOwner();

        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        pauseMenuPanel.SetActive(true);
        pauseMenuContainer.SetActive(true);
        controllerGuidePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SelectButton(resumeButton);
    }

    public void ResumeGame()
    {
        if (!isPaused)
            return;

        isPaused = false;
        showingGuide = false;

        RestoreUIInput();
        pauseOwner = null;
        
        pauseMenuPanel.SetActive(false);
        controllerGuidePanel.SetActive(false);

        Time.timeScale = previousTimeScale;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenControllerGuide()
    {
        if (!isPaused)
            return;

        showingGuide = true;

        pauseMenuContainer.SetActive(false);
        controllerGuidePanel.SetActive(true);

        SelectButton(backButton);
    }

    public void CloseControllerGuide()
    {
        if (!isPaused)
            return;

        showingGuide = false;

        controllerGuidePanel.SetActive(false);
        pauseMenuContainer.SetActive(true);

        SelectButton(controllerGuideButton);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        MenuPlayerSelection.Clear();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        RestoreUIInput();
        pauseOwner = null;

        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void SelectButton(Button button)
    {
        if (EventSystem.current == null || button == null)
            return;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button.gameObject);
    }

    private void OnDisable()
    {
        if (isPaused)
        {
            Time.timeScale = previousTimeScale;
            isPaused = false;
        }
        
        RestoreUIInput();
    }
}