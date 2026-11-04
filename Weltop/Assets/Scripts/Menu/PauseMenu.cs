using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    private void Start()
    {
        pauseMenuPanel.SetActive(false);
        controllerGuidePanel.SetActive(false);
        isPaused = false;
    }

    private void Update()
    {
        if (!PausePressed())
            return;

        if (!isPaused)
        {
            OpenPauseMenu();
        }
        else if (showingGuide)
        {
            CloseControllerGuide();
        }
        else
        {
            ResumeGame();
        }
    }

    private bool PausePressed()
    {
        foreach (PlayerInput player in PlayerInput.all)
        {
            foreach (InputDevice device in player.devices)
            {
                if (device is Keyboard keyboard &&
                    keyboard.escapeKey.wasPressedThisFrame)
                {
                    return true;
                }

                if (device is Gamepad gamepad &&
                    gamepad.startButton.wasPressedThisFrame)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void OpenPauseMenu()
    {
        if (isPaused)
            return;

        isPaused = true;
        showingGuide = false;

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

        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void SelectButton(Button button)
    {
        if (EventSystem.current == null || button == null)
            return;

        EventSystem.current.SetSelectedGameObject(null);

        button.Select();
    }

    private void OnDisable()
    {
        if (isPaused)
        {
            Time.timeScale = previousTimeScale;
            isPaused = false;
        }
    }
}