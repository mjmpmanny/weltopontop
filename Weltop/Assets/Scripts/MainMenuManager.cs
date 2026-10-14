using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "3dPersonTest";
    [SerializeField] private GameObject playerLobbyPanel;

    public void PlayGame()
    {
        // Open the player lobby instead of loading gameplay.
        playerLobbyPanel.SetActive(true);
    }

    public void BackToMenu()
    {
        // Return to the main menu.
        playerLobbyPanel.SetActive(false);
    }

    public void StartGameplay()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}