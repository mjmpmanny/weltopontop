using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "3dPersonTest";

    public void PlayGame()
    {
        // Find the device that activated the menu.
        InputDevice device = null;

        if (Gamepad.current != null &&
            Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            device = Gamepad.current;
        }
        else if (Mouse.current != null &&
                 Mouse.current.leftButton.wasPressedThisFrame)
        {
            device = Keyboard.current;
        }
        else if (Keyboard.current != null)
        {
            device = Keyboard.current;
        }

        MenuPlayerSelection.SelectedDevice = device;

        SceneManager.LoadScene(gameplaySceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}